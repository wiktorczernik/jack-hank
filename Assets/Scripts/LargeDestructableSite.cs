using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using JHCinematics;


public class LargeDestructableSite : GameEntity
{
    [Header("State")]
    /// <summary>
    /// Tells is this site being destroyed
    /// </summary>
    public bool wasDestroyed = false;
    public bool wasChainDestroyed = false;
    public bool isBeingDestroyed = false;
    /// <summary>
    /// Tells how many supports already have been destroyed
    /// </summary>
    public int hitSupportsCount = 0;

    [Header("Destruction")]
    public LargeDestructableSite[] chainedDestructables;
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
    /// <summary>
    /// True - if destruction is chained
    /// </summary>
    public UnityEvent<bool> onDestructionBegin;
    /// <summary>
    /// True - if destruction is chained
    /// </summary>
    public UnityEvent<bool> onDestructionEnd;


    public void StartDestruction()
    {
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke(false);

        ProceedDestruction();
        StartCoroutine(FinishDestruction());
    }
    public void StartChainedDestruction()
    {
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke(true);

        ProceedDestruction();

        wasDestroyed = true;
        wasChainDestroyed = true;
        isBeingDestroyed = false;
        onDestructionEnd?.Invoke(true);

        foreach (var chain in chainedDestructables)
        {
            if (chain.wasDestroyed) continue;
            chain.StartChainedDestruction();
        }
    }
    #region Event handlers
    private void OnSupportSmashed(SmashableEntity support)
    {
        hitSupportsCount++;
        if (hitSupportsCount >= maxSupportsDestroyed)
        {
            StartDestruction();
        }
    }
    #endregion
    #region Helpers
    private void ProceedDestruction()
    {
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
    }
    private IEnumerator FinishDestruction()
    {
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
        wasChainDestroyed = false;
        isBeingDestroyed = false;
        onDestructionEnd?.Invoke(false);
        
        foreach(var chain in chainedDestructables)
        {
            chain.StartChainedDestruction();
        }
    }
    #endregion

    #region Unity Messages
    private void Awake()
    {
        foreach (var entity in supports)
        {
            if (entity == null) continue;
            entity.OnHit.AddListener(OnSupportSmashed);
        }
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
    #endregion
}
