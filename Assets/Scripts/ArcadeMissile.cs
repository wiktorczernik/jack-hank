using UnityEngine;
using UnityEngine.Events;

public class ArcadeMissile : GameEntity
{
    [Header("Behaviour")]
    public Transform target;
    public float flySpeed = 10f;
    public float closeMaxDistance = 10f;
    public float closeOvershootThreshold = 0.5f;
    public UnityEvent onSpawn;
    [Header("State")]
    public float distance = Mathf.Infinity;
    public float minDistance = Mathf.Infinity;
    public float lastDistance = Mathf.Infinity;
    public bool isClose = false;
    [Header("Components")]
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] CollisionEventEmitter collisionEvents;

    private void OnEnable()
    {
        onSpawn?.Invoke();
        collisionEvents.OnEnter?.AddListener(OnCollision);
    }
    private void FixedUpdate()
    {
        distance = Vector3.Distance(transform.position, target.position);
        minDistance = Mathf.Min(minDistance, lastDistance, distance);

        if (!isClose && distance < closeMaxDistance)
        {
            isClose = true;
        }

        Vector3 targetDirection = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);

        rigidbody.MoveRotation(lookRotation);
        rigidbody.linearVelocity = targetDirection * flySpeed;

        lastDistance = distance;
    }

    private void OnCollision(Collision collision)
    {
        SelfExplode(1);
    }
    protected override void InternalSelfExplode()
    {
        base.InternalSelfExplode();
        Kill();
        Destroy(gameObject);
    }
}
