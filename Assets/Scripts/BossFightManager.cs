using System;
using System.Collections;
using UnityEngine;

using JHCinematics;

public abstract class BossFightManager : MonoBehaviour
{
    [SerializeField] protected Boss bossPrefab;
    [SerializeField] protected Transform bossSpawnPoint;
    [SerializeField] private TriggerEventEmitter beginTrigger;
    [SerializeField] private PlayerVehicle player;

    [SerializeField] private CinematicSequence beginCutscene;
    [SerializeField] private Transform beginCutsceneTransform;
    [SerializeField] private Transform playerSpawnAfterBeginCutscene;

    [SerializeField] private CinematicSequence endCutscene;
    [SerializeField] private Transform endCutsceneTransform;
    [SerializeField] private Transform playerSpawnAfterEndCutscene;

    private Boss _bossInstance;

    public bool duringFight { get; private set; }
    public bool wasTriggered { get; private set; }

    private void Awake()
    {
        var errorMessageStart = $"component [{GetType().Name}]; game object [{name}]:";

        if (!beginTrigger)
            Debug.LogError($"{errorMessageStart} No trigger for boss fight!");
        if (!bossPrefab)
            Debug.LogError($"{errorMessageStart} No boss prefab!");
        if (!bossSpawnPoint)
            Debug.LogError($"{errorMessageStart} No boss spawn point!");
        if (!beginCutscene)
            Debug.LogError($"{errorMessageStart} No begin cutscene!");
        if (!beginCutsceneTransform)
            Debug.LogError($"{errorMessageStart} No begin cutscene transform!");
        if (!endCutscene)
            Debug.LogError($"{errorMessageStart} No end cutscene!");
        if (!endCutsceneTransform)
            Debug.LogError($"{errorMessageStart} No end cutscene transform!");

        _bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

        beginTrigger.OnEnter.AddListener(OnTriggerEnter);
        _bossInstance.onDeath += OnBossDeath;
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        if (wasTriggered) return;

        wasTriggered = true;
        HandleTriggerEnter();
    }

    private void OnBossDeath()
    {
        OnBossDeathInterval();

        CinematicPlayer.PlaySequence(endCutscene, endCutsceneTransform.position, endCutsceneTransform.rotation,
            endCutsceneTransform.localScale);

        CinematicPlayer.onEndPlay += OnCutsceneEnd;
        return;

        void OnCutsceneEnd()
        {
            OnEnd?.Invoke();
            AfterEndCutscene();
            duringFight = true;
            CinematicPlayer.onEndPlay -= OnCutsceneEnd;
        }
    }

    public event Action OnBegin;
    public event Action OnRestart;
    public event Action OnEnd;

    public void Begin()
    {
        StartCoroutine(BeginCo());
    }

    public void Restart()
    {
        OnRestart?.Invoke();
        OnRestartInterval();
    }

    private IEnumerator BeginCo()
    {
        yield return PrepareCo();

        CinematicPlayer.PlaySequence(beginCutscene, beginCutsceneTransform.position, beginCutsceneTransform.rotation,
            beginCutsceneTransform.localScale);

        CinematicPlayer.onEndPlay += OnCutsceneEnd;

        yield break;

        void OnCutsceneEnd()
        {
            player.Teleport(playerSpawnAfterBeginCutscene.position, playerSpawnAfterBeginCutscene.rotation);
            OnBeginInterval();
            _bossInstance.Activate();
            duringFight = true;
            OnBegin?.Invoke();
            CinematicPlayer.onEndPlay -= OnCutsceneEnd;
        }
    }

    protected abstract void HandleTriggerEnter();

    protected abstract IEnumerator PrepareCo();

    protected abstract void OnBeginInterval();
    protected abstract void OnRestartInterval();

    protected abstract void OnBossDeathInterval();
    protected abstract void AfterEndCutscene();
}