using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class BotVehicle : Vehicle
{
    [Header("State")]
    public bool arrived = false;
    public bool isFollowing = false;
    public Vector3 destinationPoint = Vector3.zero;
    public Queue<Vector3> destinationQueue = new Queue<Vector3>();
    public bool loopQueue = false;
    public FollowMode followMode = FollowMode.Single;
    public float followMaxSpeed = 60f;

    [Header("Follow")]
    public float startFollowDelay = 0;
    private float _startFollowTimer = 0;
    [SerializeField] private SplineContainer _destinationQueueSpline;

    [Header("Destination")]
    public float destinationArriveMaxDistance = 15f;
    public float destinationArriveMaxSpeed = 20f;

    [Header("Hard turning")]
    public float hardTurnThresholdAngle = 90f;
    public float hardTurnMaxSpeed = 10f;

    [Header("Behaviour")]
    [Tooltip("In km/h")]
    public float speedDamageThreshold = 40f;
    public float dmgReceivedPerSpeedUnit = 3f;

    public event Action onArrived;


    protected virtual void Awake()
    {
        if (_destinationQueueSpline != null && _destinationQueueSpline.Spline.Count > 0)
        {
            foreach (var knot in _destinationQueueSpline.Spline.Knots)
            {
                Vector3 worldPos = _destinationQueueSpline.transform.TransformPoint(knot.Position);
                destinationQueue.Enqueue(worldPos);
            }

            if (destinationQueue.Count > 0)
            {
                destinationPoint = destinationQueue.Dequeue();
            }
        }
    }

    #region Helpers

    private bool InArriveZone()
    {
        return Vector3.Distance(transform.position, destinationPoint) < destinationArriveMaxDistance;
    }

    private bool IsArriveSpeedCorrect()
    {
        return physics.speedKmh < destinationArriveMaxSpeed;
    }

    private bool IsQueueEmpty()
    {
        return destinationQueue.Count == 0;
    }

    private bool IsFollowingSingle()
    {
        return followMode == FollowMode.Single;
    }

    private bool IsFollowingQueue()
    {
        return followMode == FollowMode.Queue;
    }

    #endregion


    private void Arrive()
    {
        onArrived?.Invoke();
        physics.input = Vector3.zero;
        arrived = true;
        isFollowing = false;
    }


    private void FollowTick()
    {
        if (_startFollowTimer < startFollowDelay)
        {
            _startFollowTimer += Time.fixedDeltaTime;
            return;
        }

        Vector3 rawDir = destinationPoint - transform.position;
        rawDir.y = 0f;
        if (rawDir.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector3 curAng = transform.rotation.eulerAngles;
        Vector3 destAng = Quaternion.LookRotation(rawDir).eulerAngles;
        float angYdiff = Mathf.DeltaAngle(curAng.y, destAng.y);
        float angYabs = Mathf.Abs(angYdiff);

        bool doAccelerate = true;
        bool doBreak = false;
        bool doTurn = true;

        float distance = rawDir.magnitude;

        if (distance < destinationArriveMaxDistance)
        {
            doAccelerate = false;
            doTurn = false;

            if ((IsFollowingQueue() && IsQueueEmpty() || IsFollowingSingle()) &&
                physics.speedKmh > destinationArriveMaxSpeed)
            {
                doBreak = true;
            }

            if ((physics.speedKmh <= destinationArriveMaxSpeed && IsFollowingSingle()) ||
                (physics.speedKmh <= destinationArriveMaxSpeed && IsFollowingQueue() && IsQueueEmpty()))
            {
                Arrive();
                return;
            }

            if (IsFollowingQueue() && !IsQueueEmpty())
            {
                if (loopQueue)
                {
                    destinationQueue.Enqueue(destinationPoint);
                }

                destinationPoint = destinationQueue.Dequeue();
            }
        }

        if (angYabs > hardTurnThresholdAngle)
        {
            if (physics.speedKmh > hardTurnMaxSpeed)
            {
                doAccelerate = false;
                doBreak = true;
                doTurn = true;
            }
            else
            {
                doAccelerate = true;
                doBreak = false;
                doTurn = true;
            }
        }

        if (physics.speedKmh > followMaxSpeed)
        {
            doAccelerate = false;
            doBreak = true;
        }

        if (doAccelerate)
        {
            physics.input.y = 1f;
        }
        else if (doBreak)
        {
            physics.input.y = -1f;
        }
        else
        {
            physics.input.y = 0f;
        }

        if (doTurn)
        {
            float steeringAmount = Mathf.Min(angYabs / 90f, 1f);
            physics.input.x = Mathf.Sign(angYdiff) * steeringAmount;
        }
        else
        {
            physics.input.x = 0f;
        }
    }


    protected virtual void FixedUpdate()
    {
        if (isFollowing)
        {
            FollowTick();
        }
    }


    public void ReceivePhysicalDamage(Collision collision)
    {
        Transform current = collision.collider.transform;
        while (current != null)
        {
            if (current.TryGetComponent<Vehicle>(out Vehicle v))
            {
                float impactSpeedKmh = collision.relativeVelocity.magnitude * 3.6f;
                if (impactSpeedKmh < speedDamageThreshold) return;

                float damage = (impactSpeedKmh - speedDamageThreshold) * dmgReceivedPerSpeedUnit;
                Hurt(damage);
                return;
            }
            current = current.parent;
        }
    }


    protected override void OnDeathInternal()
    {
        physics.enabled = false;
        enabled = false;
        SelfExplode();
    }


    public enum FollowMode
    {
        Single,
        Queue
    }
}
