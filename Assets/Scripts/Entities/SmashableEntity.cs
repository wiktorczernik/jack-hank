using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Jest to podstawowa klasa dla wszystkich zniszczalnych drobnych obiekt�w. Zarz�dza procesem niszczenia, 
/// przechowuje stan, informacje czy mo�e aktualnie zosta� zniszczone (hittable) lub popchni�te (bumpable), 
/// odtwarza d�wi�k po zderzeniu i manipuluje stanami collider�w i rigidbody. Opcjonalnie utworzy tak zwane 
/// debris (cz�stki), czyli  wersje tego smashable podzielonego na mniejsze obiekty aby zsymulowa� rozsypanie 
/// si�. SmashableEntity oraz jego debris musz� by� przechowywane w oddzielnych prefabach.
/// </summary>
public class SmashableEntity : GameEntity
{
    /// <summary>
    /// Czy zosta�o potr�cone?
    /// </summary>
    [Header("State")]
    public bool wasHit = false;

    /// <summary>
    /// Czy jest w stanie zosta� potr�cone i popchni�te poprzez bumper?
    /// </summary>
    [Header("Main")]
    public bool bumpable = true;
    /// <summary>
    /// Czy jest w stanie zosta� potr�cone?
    /// </summary>
    public bool hittable = true;
    /// <summary>
    /// Nagroda za zniszczenie
    /// </summary>
    public int bountyPointsReward;
    /// <summary>
    /// Czas po kt�rym obiekt zostanie zniszczony (destroy) je�li zosta� potr�cony
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
    /// Czy to smashable powinno rozbi� si� na ma�e kawa�ki po potr�ceniu
    /// </summary>
    public bool shouldBecomeDebris = false;
    /// <summary>
    /// Prefab kawa�k�w
    /// </summary>
    public GameObject debrisPrefab;

    /// <summary>
    /// G��wne rigidbody wykorzystywane przez to smashable
    /// </summary>
    [Header("Physics")]
    public Rigidbody usedRigidbody;
    /// <summary>
    /// Lista collider�w wykorzystywanych przez to smashable
    /// </summary>
    public Collider[] usedColliders = new Collider[0];
    [SerializeField] protected CollisionEventEmitter collisionEvents;

    [Header("Physics On Hit")]
    public UnityEvent<SmashableEntity> OnHit;
    /// <summary>
    /// Czy rotacja powinna by� zamro�ona po zderzeniu
    /// </summary>
    public bool hitFreezeRotation = false;
    /// <summary>
    /// Ograniczenia pozycyjne rigidbody po zderzeniu
    /// </summary>
    public RigidbodyConstraints hitConstrains = RigidbodyConstraints.None;
    /// <summary>
    /// Czy rigidbody powinno by� kinematic po zderzeniu
    /// </summary>
    public bool hitIsKinematic = false;

    /// <summary>
    /// Zestaw d�wi�k�w kt�re mog� by� zagrane po zderzeniu
    /// </summary>
    [Header("Audio")]
    [SerializeField] protected AudioClip[] impactAudios = new AudioClip[0];
    /// <summary>
    /// �r�d�o d�wi�ku
    /// </summary>
    public AudioSource audioSource;
    /// <summary>
    /// Minimalny pitch d�wi�ku zderzenia
    /// </summary>
    [SerializeField] protected float impactMinPitch = 0.75f;
    /// <summary>
    /// Maksymalny pitch d�wi�ku zderzenia
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
    /// Powoduje znieszczenie smashable na si��
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
    /// <param name="explosionProps">W�a�ciwo�ci wybuchu</param>
    protected override void InternalExplode(ExplosionProperties explosionProps)
    {
        if (wasHit) return;
        
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
    /// Zagra losowy d�wi�k po zderzeniu
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