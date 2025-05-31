using System.Collections;
using JackHank.Cinematics;
using UnityEngine;
using UnityEngine.Events;
using Random = Unity.Mathematics.Random;

/// <summary>
/// Jest to podstawowa klasa dla wszystkich zniszczalnych budynków. Zarz¹dza procesem niszczenia, 
/// przechowuje stan i kondycje do wywo³ania znisczenia. Mo¿e spowodowaæ znisczenia innych budynków, 
/// wywo³aæ cinematic i teleportowaæ gracza po nim.
/// </summary>
public class LargeDestructableSite : GameEntity
{
    [Header("State")]
    /// <summary>
    /// Czy budynek zosta³ zniszczony?
    /// </summary>
    public bool wasDestroyed;
    /// <summary>
    /// Czy budynek zosta³ zniszczony poprzez znisczenie innego budynku?
    /// </summary>
    public bool wasChainDestroyed;
    /// <summary>
    /// Czy aktualnie jest w trakcie nisczenia?
    /// </summary>
    public bool isBeingDestroyed;
    /// <summary>
    /// Iloœæ smashable wspieraj¹cych ten budynek od zawalenia siê
    /// </summary>
    public int hitSupportsCount;
    /// <summary>
    /// Po jakim czasie od znisczenia budynku pojawi siê jego debris
    /// </summary>
    [Header("Destruction")] public float debrisDelay = 1f;
    /// <summary>
    /// Minimalny czas do wywo³ania znisczenia zale¿nego budynku
    /// </summary>
    public float chainMinDelay = 1;
    /// <summary>
    /// Maksymalny czas do wywo³ania znisczenia zale¿nego budynku
    /// </summary>
    public float chainMaxDelay = 3;
    /// <summary>
    /// Budynki zale¿ne od tego, które te¿ zostan¹ zniszczone bo zniszczeniu tego budynku
    /// </summary>
    public LargeDestructableSite[] chainedDestructables;
    // Lista rigidbody która bêdzie debris
    public Rigidbody[] debrisRigidbodies;

    /// <summary>
    /// Maksymalna liczba supportów która mo¿e byæ zniszczona bez wywo³ania znisczenia budynku
    /// </summary>
    [Header("Support")]
    public int maxSupportsDestroyed = 1;

    /// <summary>
    /// Czy wszystkie smashable zostan¹ potr¹cone po znisczoniu
    /// </summary>
    public bool smashSupportsOnDestroy = true;

    /// <summary>
    /// Lista wspieraj¹cych smashable
    /// </summary>
    public SmashableEntity[] supports;

    /// <summary>
    /// Cinematic który zostanie odtworzony po zniszczeniu
    /// </summary>
    [Header("Cinematics")] 
    public CinematicSequence cinematicSequence;
    /// <summary>
    /// Transform bêd¹cy rodzicem instancji cinematica
    /// </summary>
    public Transform cinematicParent;
    /// <summary>
    /// Gdzie ma teleportowaæ siê gracz po cinematicu
    /// </summary>
    public Transform cinematicEndWarp;
    /// <summary>
    /// Decyduje, czy powinna zostaæ nadana nowa prêdkoœæ autobusowi po cutscenie
    /// </summary>
    public bool cinematicOverrideVelocity;
    /// <summary>
    /// Prêdkoœæ autobusu po cutscenie
    /// </summary>
    public Vector3 cinematicEndVelocity;

    /// <summary>
    /// True - jeœli zosta³o wywo³ane przez inny budynek
    /// </summary>
    [Header("Events")]
    public UnityEvent<bool> onDestructionBegin;

    /// <summary>
    /// True - jeœli zosta³o wywo³ane przez inny budynek
    /// </summary>
    public UnityEvent<bool> onDestructionEnd;

    /// <summary>
    /// Ile punktów zdobêdzie gracz za zniszczenie tego budynku
    /// </summary>
    [Header("Bonus")] public int bountyPointsReward;


    /// <summary>
    /// Rozpocznie proces destrukcji tego budynku
    /// </summary>
    public void StartDestruction()
    {
        if (isBeingDestroyed || wasDestroyed || wasChainDestroyed) return;
        isBeingDestroyed = true;
        onDestructionBegin?.Invoke(false);

        StartCoroutine(CreateDebris());
        StartCoroutine(FinishDestruction());
    }
    /// <summary>
    /// Rozpocznie proces destrukcji tego budynku poprzez zale¿noœæ od innego budynku
    /// </summary>
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
    /// <summary>
    /// Tworzy debris dla tego budynku
    /// </summary>
    /// <returns></returns>
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
                    entity.ForceHit();
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