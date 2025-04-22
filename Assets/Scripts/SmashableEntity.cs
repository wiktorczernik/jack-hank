using UnityEngine;
using UnityEngine.Events;

public class SmashableEntity : GameEntity
{
    [Header("State")]
    public bool wasHit = false;

    [Header("Main")]
    public bool hittable = true;
    public int bountyPointsReward;
    public float destroyTime = 10f;
    public GameObject model;
    [SerializeField] protected SmashableType smashableType;

    [Header("Debris")]
    /// <summary>
    /// Tells if should delete main model and spawn debris on hit
    /// </summary>
    public bool shouldBecomeDebris = false;
    public GameObject debrisPrefab;

    [Header("Physics")]
    [SerializeField] protected Rigidbody usedRigidbody;
    [SerializeField] protected Collider[] usedColliders = new Collider[0];
    [SerializeField] protected CollisionEventEmitter collisionEvents;

    [Header("Physics On Hit")]
    public UnityEvent<SmashableEntity> OnHit;
    public bool hitFreezeRotation = false;
    public RigidbodyConstraints hitConstrains = RigidbodyConstraints.None;
    public bool hitIsKinematic = false;

    [Header("Audio")]
    [SerializeField] protected AudioClip[] impactAudios = new AudioClip[0];
    [SerializeField] protected AudioSource audioSource;

    public SmashableType SmashableType => smashableType;



    #region Event subscribing
    private void OnEnable()
    {
        collisionEvents.OnEnter?.AddListener(OnColliderHit);
    }
    private void OnDisable()
    {
        collisionEvents.OnEnter?.RemoveListener(OnColliderHit);
    }
    #endregion

    #region Events
    private void OnColliderHit(Collision collision)
    {
        if (collision.collider == null) return;

        SmashableEntity otherSmashable;
        bool isVehicleOrPass = collision.gameObject.tag == "Vehicle" || collision.gameObject.tag == "Passenger";
        bool isSmashable = collision.gameObject.TryGetComponent(out otherSmashable);

        if (!isVehicleOrPass && !isSmashable) return;
        
        if (!wasHit)
        {
            if (!hittable) return;
            if (isSmashable && !otherSmashable.wasHit) return;

            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 contactNormal = collision.contacts[0].normal;
            float relativeSpeed = collision.relativeVelocity.magnitude;

            usedRigidbody.freezeRotation = hitFreezeRotation;
            usedRigidbody.constraints = hitConstrains;
            usedRigidbody.isKinematic = hitIsKinematic;

            PlayImpactAudio();

            OnHitEvent();
            OnHit?.Invoke(this);
            wasHit = true;

            if (shouldBecomeDebris)
            {
                // Model's transform
                var modTrans = model.transform;
                var debrisInstance = Instantiate(debrisPrefab, modTrans.position, modTrans.rotation, gameObject.transform);
                debrisInstance.transform.localScale = modTrans.localScale;
                Destroy(model);
            }

            Invoke(nameof(SelfDestroy), destroyTime);
        }
    }

    protected virtual void OnHitEvent()
    {

    }
    protected override void InternalExplode(ExplosionProperties explosionProps)
    {
        usedRigidbody.freezeRotation = hitFreezeRotation;
        usedRigidbody.constraints = hitConstrains;
        usedRigidbody.isKinematic = hitIsKinematic;

        OnHitEvent();
        OnHit?.Invoke(this);
        wasHit = true;

        if (shouldBecomeDebris)
        {
            // Model's transform
            var modTrans = model.transform;
            var debrisInstance = Instantiate(debrisPrefab, modTrans.position, modTrans.rotation, gameObject.transform);
            debrisInstance.transform.localScale = modTrans.localScale;
            Destroy(model);
        }

        usedRigidbody.AddExplosionForce(explosionProps.force, explosionProps.epicenterPosition, explosionProps.epicenterRadius);

        Invoke(nameof(SelfDestroy), destroyTime);
    }

    public void SetRigidbodyKinematic(bool flag)
    {
        usedRigidbody.isKinematic = flag;
    }
    public void EnableColliders()
    {
        foreach(Collider c in usedColliders)
        {
            c.enabled = true;
        }
    }
    public void DisableColliders()
    {
        foreach (Collider c in usedColliders)
        {
            c.enabled = false;
        }
    }
    public void PlayImpactAudio()
    {
        if (impactAudios.Length == 0) return;

        int clipIndex = Random.Range(0, impactAudios.Length);
        var clip = impactAudios[clipIndex];

        if (!clip)
        {
            Debug.LogError("Smashable entity has null audio clip!", this);
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }
    
    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!model)
        {
            Debug.LogError("Smashable model game object wasn't filled in!", this);
            return;
        }
        if (!usedRigidbody)
        {
            usedRigidbody = model.GetComponentInChildren<Rigidbody>();
            if (!usedRigidbody)
            {
                Debug.LogError("Smashable entity doesn't have attached rigidbody to it!", this);
            }
        }
        if (!audioSource)
        {
            audioSource = GetComponentInChildren<AudioSource>();
            if (!audioSource)
            {
                Debug.LogError("Smashable entity doesn't have attached audio source to it!", this);
            }
        }
        if (usedColliders.Length == 0)
        {
            usedColliders = model.GetComponentsInChildren<Collider>();
            if (usedColliders.Length == 0)
            {
                Debug.LogError("Smashable entity doesn't have any colliders attached to it!", this);
            }
        }
        if (!collisionEvents)
        {
            collisionEvents = model.GetComponentInChildren<CollisionEventEmitter>();
            if (!collisionEvents)
            {
                Debug.LogError("Smashable entity doesn't have collision event emitter attached to it!", this);
            }
        }
    }
#endif
    #endregion
}

public enum SmashableType
{
    Cow, 
    RoadSign, 
    LightingPole, 
    TrafficBarrier, 
    TrafficLight, 
    TrafficCone, 
    OutdoorTable, 
    Stall, 
    Crate, 
    Cart, 
    Bench, 
    Trash, 
    Hydrant, 
    SmallSign, 
    Barrel
}