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
    /// <summary>
    /// Pozycja, do której próbuje dotrzeæ rakieta.
    /// W przypadku gdy jest null, leci wed³ug ostatnio zapisanego kierunku.
    /// </summary>
    public Transform target;
    /// <summary>
    /// Pocz¹tkowa prêdkoœæ z jak¹ poleci rakieta po spawnie
    /// </summary>
    public float startFlySpeed = 10f;
    /// <summary>
    /// Maksymalna prêdkoœæ jak¹ mo¿e uzyskaæ rakieta
    /// </summary>
    public float maxFlySpeed = 50f;
    /// <summary>
    /// Krzywa, której oœ X decyduje o d³ugoœci zmiany prêdkoœci od startFlySpeed do maxFlySpeed, a oœ Y decyduje o tempie zmiany
    /// </summary>
    public AnimationCurve speedIncreaseGraph;
    /// <summary>
    /// Ile trwa zmiana z pocz¹tkowej prêdkoœci do maksymalnej. Nie zmieniaæ tego bezpoœrednio, do tego jest speedIncreaseGraph
    /// </summary>
    public float speedIncreaseDuration;
    /// <summary>
    /// Czy wywo³a zdarzenie odbicia siê po zderzeniu z graczem, nie powoduj¹c wybuchu
    /// </summary>
    public bool bounceOffPlayer = false;
    /// <summary>
    /// Pionowy offset (world up) wobec targeta do którego d¹¿y rakieta
    /// </summary>
    public float targetVerticalOffset = 0;
    public UnityEvent onSpawn;
    public UnityEvent onBounceOff;
    [Header("State")]
    public float currentFlySpeed;
    /// <summary>
    /// Aktualna odleg³oœæ do celu
    /// </summary>
    public float distance = Mathf.Infinity;
    /// <summary>
    /// Najmniejsza odleg³oœæ do celu jak¹ mia³a rakieta podczas lotu
    /// </summary>
    public float minDistance = Mathf.Infinity;
    /// <summary>
    /// Kierunek lotu rakiety
    /// </summary>
    public Vector3 direction = Vector3.zero;
    /// <summary>
    /// Poprzedni kierunek lotu rakiety
    /// </summary>
    public Vector3 lastDirection = Vector3.zero;
    /// <summary>
    /// Rotacja rakiety
    /// </summary>
    public Quaternion lookRotation = Quaternion.identity;
    /// <summary>
    /// Poprzednia rotacja rakiety
    /// </summary>
    public Quaternion lastLookRotation = Quaternion.identity;
    /// <summary>
    /// Poprzednia odleg³oœæ do celu
    /// </summary>
    public float lastDistance = Mathf.Infinity;
    /// <summary>
    /// Czas od którego zaczê³a lecieæ rakieta (Time.time)
    /// </summary>
    public float startFlyTime;
    /// <summary>
    /// Ile czasu minê³o od pocz¹tku lotu
    /// </summary>
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
