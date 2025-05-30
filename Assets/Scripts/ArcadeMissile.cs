using UnityEngine;
using UnityEngine.Events;

public class ArcadeMissile : GameEntity
{
    [Header("Behaviour")]
    public Transform target;
    public float startFlySpeed = 10f;
    public float maxFlySpeed = 50f;
    public AnimationCurve speedIncreaseGraph;
    public float speedIncreaseDuration;
    public bool bounceOffPlayer = false;
    public float targetVerticalOffset = 0;
    public UnityEvent onSpawn;
    public UnityEvent onBounceOff;
    [Header("State")]
    public float currentFlySpeed;
    public float distance = Mathf.Infinity;
    public float minDistance = Mathf.Infinity;
    public Vector3 direction = Vector3.zero;
    public Vector3 lastDirection = Vector3.zero;
    public Quaternion lookRotation = Quaternion.identity;
    public Quaternion lastLookRotation = Quaternion.identity;
    public float lastDistance = Mathf.Infinity;
    public float startFlyTime;
    public float timeSinceStart;
    [Header("Components")]
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] CollisionEventEmitter collisionEvents;

    private void OnEnable()
    {
        onSpawn?.Invoke();
        collisionEvents.OnEnter?.AddListener(OnCollision);
        currentFlySpeed = startFlySpeed;
        startFlyTime = Time.time;
    }
    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 targetPos = target.position + Vector3.up * targetVerticalOffset;

            distance = Vector3.Distance(transform.position, targetPos);
            minDistance = Mathf.Min(minDistance, lastDistance, distance);

            direction = (targetPos - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);
        }

        rigidbody.MoveRotation(lookRotation);
        rigidbody.linearVelocity = direction * currentFlySpeed;

        lastDistance = distance;

        timeSinceStart = Time.time - startFlyTime;
        if (timeSinceStart < speedIncreaseDuration)
        {
            float speedIncrease = maxFlySpeed - startFlySpeed;
            speedIncrease *= speedIncreaseGraph.Evaluate(timeSinceStart);
            currentFlySpeed = startFlySpeed + speedIncrease;
        }
        else
            currentFlySpeed = maxFlySpeed;

        lastDirection = direction;
        lastLookRotation = lookRotation;
    }

    private void OnCollision(Collision collision)
    {
        if (bounceOffPlayer)
        {
            var player = collision.gameObject.GetComponentInParent<PlayerVehicle>();
            if (player)
            {
                onBounceOff?.Invoke();
                return;
            }
        }
        SelfExplode(1);
    }
    protected override void InternalSelfExplode()
    {
        base.InternalSelfExplode();
        Kill();
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (speedIncreaseGraph.keys.Length == 0) return;
        speedIncreaseDuration = speedIncreaseGraph.keys[speedIncreaseGraph.length - 1].time;
    }
#endif
}
