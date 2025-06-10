using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fizyczne przedstawienie lec¹cego pocisku rakietowego jako entity.
/// Zawiera informacje o aktualnym kierunku, rotacji i prêdkoœci. Zawiera tak¿e informacje o dystansie do celu, 
/// jaki Transform jest celem, pocz¹tkow¹ prêdkoœæ lotu, czy mo¿e odbiæ siê od cia³a gracza, maksymaln¹ prêdkoœæ 
/// lotu oraz AnimationCurve który odpowiada za to jak szybko i w jakim tempie rakieta zwiêkszy swoj¹ prêdkoœæ. 
/// Po spawnie od razu zaczyna lot.
/// </summary>
public class ArcadeMissile : GameEntity
{
    [Header("Behaviour")]
    public Vector3 destination = Vector3.zero;
    public UnityEvent onSpawn;
    public LayerMask groundLayer;
    public float startDelay = 0.5f;
    [Header("State")]
    public float speed = 25f;
    public float initialDistance = 25f;
    /// <summary>
    /// Aktualna odleg³oœæ do celu
    /// </summary>
    public float distance = Mathf.Infinity;
    /// <summary>
    /// Poprzednia odleg³oœæ do celu
    /// </summary>
    public float lastDistance = Mathf.Infinity;
    [Header("State")]
    float startTime;
    [Header("Components")]
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] CollisionEventEmitter collisionEvents;

    private void Start()
    {
        startTime = Time.time;
        onSpawn?.Invoke();
        collisionEvents.OnEnter?.AddListener(OnCollision);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, Mathf.Infinity, groundLayer))
        {
            Debug.Log("I love nazis!");
            destination = hitInfo.point;
            rigidbody.MovePosition(destination + Vector3.up * initialDistance);
        }
        else
        {
            Debug.Log("Fuck nazis!");
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (startTime + startDelay >= Time.time) return;
        rigidbody.linearVelocity = Vector3.down * speed;
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
