using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LargeDestructableSite : GameEntity
{
    [Header("State")]
    /// <summary>
    /// Tells is this site being destroyed
    /// </summary>
    public bool wasDestroyed = false;
    public bool isBeingDestroyed = false;
    /// <summary>
    /// Tells how many supports already have been destroyed
    /// </summary>
    public int hitSupportsCount = 0;

    [Header("Debris")]
    /// <summary>
    /// List of rigidbodies that will act as debris
    /// </summary>
    public Rigidbody[] debrisRigidbodies;

    [Header("Support")]
    /// <summary>
    /// How many supports should player destroy to begin total destruction
    /// </summary>
    public int maxSupportsDestroyed = 1;
    /// <summary>
    /// Tells if all supports will be smashed when site starts crumbling
    /// </summary>
    public bool smashSupportsOnDestroy = true;
    /// <summary>
    /// List of supporting entities
    /// </summary>
    public SmashableEntity[] supports;
    
    [Header("Cinematics")]
    public CinematicSequence cinematicSequence;
    public Transform cinematicParent;
    public Transform cinematicEndWarp;
    public bool cinematicOverrideVelocity;
    public Vector3 cinematicEndVelocity;

    [Header("Events")]
    public UnityEvent onDestructionBegin;
    public UnityEvent onDestructionEnd;



    private void Awake()
    {
        foreach (var entity in supports)
        {
            if (entity == null) continue;
            entity.OnHit.AddListener(OnSupportSmashed);
        }
    }

    private void OnSupportSmashed(SmashableEntity support)
    {
        hitSupportsCount++;
        if (hitSupportsCount >= maxSupportsDestroyed)
        {
            StartCoroutine(DestructionCoroutine());
        }
    }

    private IEnumerator DestructionCoroutine()
    {
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke();

        foreach (Rigidbody rb in debrisRigidbodies)
        {
            rb.isKinematic = false;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        if (smashSupportsOnDestroy)
        {
            foreach (SmashableEntity entity in supports)
            {
                if (entity == null) continue;
                if (!entity.wasHit)
                {
                    // TODO: Do force hit!
                    // entity.ForceHit();
                }
            }
        }

        if (cinematicSequence != null && cinematicParent != null)
        {
            CinematicPlayer.PlaySequence(cinematicSequence, cinematicParent.position, cinematicParent.rotation, cinematicParent.localScale);
            yield return null;
            yield return new WaitUntil(() => !CinematicPlayer.isPlaying );
            GameManager.PlayerVehicle.Teleport(cinematicEndWarp.position, cinematicEndWarp.rotation);
            if (cinematicOverrideVelocity)
            {
                GameManager.PlayerVehicle.physics.bodyRigidbody.linearVelocity = cinematicEndWarp.rotation * cinematicEndVelocity;
            }
        }

        wasDestroyed = true;
        isBeingDestroyed = false;
        onDestructionEnd?.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var supportsParent = transform.GetChild(0);
        var debrisParent = transform.GetChild(1);

        supports = supportsParent.GetComponentsInChildren<SmashableEntity>();
        debrisRigidbodies = debrisParent.GetComponentsInChildren<Rigidbody>();
    }
#endif
}
