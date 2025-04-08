using System.Collections.Generic;
using System.Security.Cryptography;
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

    [Header("Folllow")]
    [SerializeField] SplineContainer _destinationQueueSpline;

    [Header("Destination")]
    public float destinationArriveMaxDistance = 15f;
    public float destinationArriveMaxSpeed = 20f;

    [Header("Hard turning")]
    public float hardTurnThresholdAngle = 90f;
    public float hardTurnMaxSpeed = 10f;


    private void Awake()
    {
        
        if (_destinationQueueSpline != null && _destinationQueueSpline.Spline.Count > 0)
        {
            foreach (var knot in _destinationQueueSpline.Spline.Knots)
            {
                Vector3 localPos = knot.Position;
                Vector3 worldPos = _destinationQueueSpline.transform.TransformPoint(localPos);
                destinationQueue.Enqueue(worldPos);
            }
            destinationPoint = destinationQueue.Dequeue();
        }
    }

    #region Helpers
    private bool InArriveZone() => DistanceToDest() < destinationArriveMaxDistance;
    private bool IsArriveSpeedCorrect() => physics.speedKmhForward < destinationArriveMaxSpeed;
    private float DistanceToDest() => Vector3.Distance(transform.position, destinationPoint);
    private Vector3 DirectionToDest() => destinationPoint - transform.position;
    private bool IsQueueEmpty() => destinationQueue.Count == 0;
    private bool IsFollowingSingle() => followMode == FollowMode.Single;
    private bool IsFollowingQueue() => followMode == FollowMode.Queue;
    #endregion

    private void Arrive()
    {
        physics.input = Vector3.zero;
        arrived = true;
        isFollowing = false;
    }

    private void FollowTick()
    {
        if (arrived) return;

        float distance = DistanceToDest();
        Vector3 destDir = DirectionToDest();
        Vector3 curAng = transform.eulerAngles;
        Vector3 destAng = Quaternion.LookRotation(destDir).eulerAngles;
        // Difference between destination Y angle and current Y angle;
        float angYdiff = destAng.y - curAng.y;

        bool doAccelerate = true;
        bool doBreak = false;
        bool doTurn = true;

        if (InArriveZone()) {
            doAccelerate = false;
            doBreak = true;
            doTurn = false;

            if (
                (IsArriveSpeedCorrect() && IsFollowingSingle()) 
                ||
                (IsArriveSpeedCorrect() && IsFollowingQueue() && IsQueueEmpty())
            ) {
                Arrive();
                return;
            }
            if (IsFollowingQueue() && !IsQueueEmpty()) {
                if (loopQueue) destinationQueue.Enqueue(destinationPoint);
                destinationPoint = destinationQueue.Dequeue();
            }
        }
        if (Mathf.Abs(angYdiff) > hardTurnThresholdAngle)
        {
            if (physics.speedKmh > hardTurnMaxSpeed)
            {
                doAccelerate = false && doAccelerate;
                doBreak = true && doBreak;
            }
            else
            {
                doAccelerate = true && doAccelerate;
                doBreak = false && doBreak;
            }
        }
        if (physics.speedKmh > followMaxSpeed) {
            doAccelerate = false;
            doBreak = true;
        }

        if (doAccelerate)
        {
            physics.input.y = 1f;
        }
        if (doBreak)
        {
            physics.input.y = -1f;
        }
        if (doTurn)
        {
            physics.input.x = curAng.y > destAng.y ? -1f : 1f;
        }
    }
    private void FixedUpdate()
    {
        if (isFollowing) FollowTick();
    }


    public enum FollowMode
    {
        Single,
        Queue
    }
}
