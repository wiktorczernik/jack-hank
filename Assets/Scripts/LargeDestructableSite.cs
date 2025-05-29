using System.Collections;
using JackHank.Cinematics;
using UnityEngine;
using UnityEngine.Events;
using Random = Unity.Mathematics.Random;

public class LargeDestructableSite : GameEntity
{
    [Header("State")]
    /// <summary>
    /// Tells is this site being destroyed
    /// </summary>
    public bool wasDestroyed;

    public bool wasChainDestroyed;
    public bool isBeingDestroyed;

    /// <summary>
    ///     Tells how many supports already have been destroyed
    /// </summary>
    public int hitSupportsCount;

    [Header("Destruction")] public float debrisDelay = 1f;

    public float chainMinDelay = 1;
    public float chainMaxDelay = 3;
    public LargeDestructableSite[] chainedDestructables;

    /// <summary>
    ///     List of rigidbodies that will act as debris
    /// </summary>
    public Rigidbody[] debrisRigidbodies;

    [Header("Support")]
    /// <summary>
    /// How many supports should player destroy to begin total destruction
    /// </summary>
    public int maxSupportsDestroyed = 1;

    /// <summary>
    ///     Tells if all supports will be smashed when site starts crumbling
    /// </summary>
    public bool smashSupportsOnDestroy = true;

    /// <summary>
    ///     List of supporting entities
    /// </summary>
    public SmashableEntity[] supports;

    [Header("Cinematics")] public CinematicSequence cinematicSequence;

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
    ///     True - if destruction is chained
    /// </summary>
    public UnityEvent<bool> onDestructionEnd;

    [Header("Bonus")] public int bountyPointsReward;


    public void StartDestruction()
    {
        if (isBeingDestroyed || wasDestroyed || wasChainDestroyed) return;
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke(false);

        StartCoroutine(CreateDebris());
        StartCoroutine(FinishDestruction());
    }

    public void StartChainedDestruction()
    {
        if (isBeingDestroyed || wasDestroyed || wasChainDestroyed) return;
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke(true);

        StartCoroutine(CreateDebris());

        wasDestroyed = true;
        wasChainDestroyed = true;
        isBeingDestroyed = false;
        onDestructionEnd?.Invoke(true);

        var random = new Random((uint)Time.time);
        foreach (var chain in chainedDestructables)
        {
            var time = random.NextFloat(chainMinDelay, chainMaxDelay);

            IEnumerator DelayedChain()
            {
                yield return new WaitForSeconds(time);
                if (chain.isBeingDestroyed) yield break;
                if (chain.wasDestroyed || chain.wasChainDestroyed) yield break;
                chain.StartChainedDestruction();
            }

            StartCoroutine(DelayedChain());
        }
    }

    #region Event handlers

    private void OnSupportSmashed(SmashableEntity support)
    {
        hitSupportsCount++;
        if (hitSupportsCount >= maxSupportsDestroyed) StartDestruction();
    }

    #endregion

    #region Helpers

    private IEnumerator CreateDebris()
    {
        yield return new WaitForSeconds(debrisDelay);

        foreach (var rb in debrisRigidbodies)
        {
            rb.isKinematic = false;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        if (smashSupportsOnDestroy)
            foreach (var entity in supports)
            {
                if (entity == null) continue;
                if (!entity.wasHit)
                {
                    // TODO: Do force hit!
                    // entity.ForceHit();
                }
            }
    }

    private IEnumerator FinishDestruction()
    {
        if (cinematicSequence != null && cinematicParent != null)
        {
            CinematicPlayer.PlaySequence(cinematicSequence, cinematicParent.position, cinematicParent.rotation,
                cinematicParent.localScale);
            yield return null;
            yield return new WaitUntil(() => !CinematicPlayer.isPlaying);
            GameManager.PlayerVehicle.Teleport(cinematicEndWarp.position, cinematicEndWarp.rotation);
            if (cinematicOverrideVelocity)
                GameManager.PlayerVehicle.physics.bodyRigidbody.linearVelocity =
                    cinematicEndWarp.rotation * cinematicEndVelocity;
        }

        wasDestroyed = true;
        wasChainDestroyed = false;
        isBeingDestroyed = false;
        onDestructionEnd?.Invoke(false);

        GameManager.UpdateBonus(bountyPointsReward, PlayerBonusTypes.LargeDestruction);

        var random = new Random((uint)Time.time);
        foreach (var chain in chainedDestructables)
        {
            var time = random.NextFloat(chainMinDelay, chainMaxDelay);

            IEnumerator DelayedChain()
            {
                yield return new WaitForSeconds(time);
                if (chain.isBeingDestroyed) yield break;
                if (chain.wasDestroyed || chain.wasChainDestroyed) yield break;
                chain.StartChainedDestruction();
            }

            StartCoroutine(DelayedChain());
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

        if (supports == null || supports.Length == 0)
            supports = supportsParent.GetComponentsInChildren<SmashableEntity>();
        if (debrisRigidbodies == null || debrisRigidbodies.Length == 0)
            debrisRigidbodies = debrisParent.GetComponentsInChildren<Rigidbody>();
    }
#endif

    #endregion
}