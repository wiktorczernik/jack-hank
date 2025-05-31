using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Jest to podstawowa klasa dla wszystkich zniszczalnych drobnych obiektÛw. Zarzπdza procesem niszczenia, 
/// przechowuje stan, informacje czy moøe aktualnie zostaÊ zniszczone (hittable) lub popchniÍte (bumpable), 
/// odtwarza düwiÍk po zderzeniu i manipuluje stanami colliderÛw i rigidbody. Opcjonalnie utworzy tak zwane 
/// debris (czπstki), czyli  wersje tego smashable podzielonego na mniejsze obiekty aby zsymulowaÊ rozsypanie 
/// siÍ. SmashableEntity oraz jego debris muszπ byÊ przechowywane w oddzielnych prefabach.
/// </summary>
public class SmashableEntity : GameEntity
{
    /// <summary>
    /// Czy zosta≥o potrπcone?
    /// </summary>
    [Header("State")]
    public bool wasHit = false;

    /// <summary>
    /// Czy jest w stanie zostaÊ potrπcone i popchniÍte poprzez bumper?
    /// </summary>
    [Header("Main")]
    public bool bumpable = true;
    /// <summary>
    /// Czy jest w stanie zostaÊ potrπcone?
    /// </summary>
    public bool hittable = true;
    /// <summary>
    /// Nagroda za zniszczenie
    /// </summary>
    public int bountyPointsReward;
    /// <summary>
    /// Czas po ktÛrym obiekt zostanie zniszczony (destroy) jeúli zosta≥ potrπcony
    /// </summary>
    public float destroyTime = 10f;
    /// <summary>
    /// Referencja do obiektu modelu tego obiektu
    /// </summary>
    public GameObject model;
    /// <summary>
    /// Typ smashable
    /// </summary>
    [SerializeField] protected SmashableType smashableType;

    [Header("Debris")]
    /// <summary>
    /// Czy to smashable powinno rozbiÊ siÍ na ma≥e kawa≥ki po potrπceniu
    /// </summary>
    public bool shouldBecomeDebris = false;
    /// <summary>
    /// Prefab kawa≥kÛw
    /// </summary>
    public GameObject debrisPrefab;

    /// <summary>
    /// G≥Ûwne rigidbody wykorzystywane przez to smashable
    /// </summary>
    [Header("Physics")]
    public Rigidbody usedRigidbody;
    /// <summary>
    /// Lista colliderÛw wykorzystywanych przez to smashable
    /// </summary>
    [SerializeField] protected Collider[] usedColliders = new Collider[0];
    [SerializeField] protected CollisionEventEmitter collisionEvents;

    [Header("Physics On Hit")]
    public UnityEvent<SmashableEntity> OnHit;
    /// <summary>
    /// Czy rotacja powinna byÊ zamroøona po zderzeniu
    /// </summary>
    public bool hitFreezeRotation = false;
    /// <summary>
    /// Ograniczenia pozycyjne rigidbody po zderzeniu
    /// </summary>
    public RigidbodyConstraints hitConstrains = RigidbodyConstraints.None;
    /// <summary>
    /// Czy rigidbody powinno byÊ kinematic po zderzeniu
    /// </summary>
    public bool hitIsKinematic = false;

    /// <summary>
    /// Zestaw düwiÍkÛw ktÛre mogπ byÊ zagrane po zderzeniu
    /// </summary>
    [Header("Audio")]
    [SerializeField] protected AudioClip[] impactAudios = new AudioClip[0];
    /// <summary>
    /// èrÛd≥o düwiÍku
    /// </summary>
    [SerializeField] protected AudioSource audioSource;
    /// <summary>
    /// Minimalny pitch düwiÍku zderzenia
    /// </summary>
    [SerializeField] protected float impactMinPitch = 0.75f;
    /// <summary>
    /// Maksymalny pitch düwiÍku zderzenia
    /// </summary>
    [SerializeField] protected float impactMaxPitch = 1.25f;
    /// <summary>
    /// Typ smashable
    /// </summary>
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
        if (collision.relativeVelocity.magnitude < 5f) return;
        ForceHit();
    }
    /// <summary>
    /// Powoduje znieszczenie smashable na si≥Í
    /// </summary>
    public void ForceHit()
    {
        if (!wasHit)
        {
            if (!hittable) return;

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

    protected virtual void OnHitEvent() { }
    /// <summary>
    /// Reackja na wybuch poprzez znisczenie
    /// </summary>
    /// <param name="explosionProps">W≥aúciwoúci wybuchu</param>
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
    /// <summary>
    /// Zagra losowy düwiÍk po zderzeniu
    /// </summary>
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

        audioSource.pitch = Random.Range(impactMinPitch, impactMaxPitch);
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
    Barrel,
    WaterTower,
    Cardboard
}