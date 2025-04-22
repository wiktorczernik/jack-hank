using System;
using System.Collections;
using UnityEngine;

public abstract class BossFightManager : MonoBehaviour
{
    [SerializeField] protected Boss bossPrefab;
    [SerializeField] protected Transform bossSpawnPoint;
    [SerializeField] private TriggerEventEmitter beginTrigger;
    [SerializeField] private CinematicSequence beginCutscene;
    [SerializeField] private Transform beginCutsceneTransform;
    [SerializeField] private CinematicSequence endCutscene;
    [SerializeField] private Transform endCutsceneTransform;

    public bool duringFight { get; private set; }
    public bool wasTriggered { get; private set; }

    private void Awake()
    {
        if (!beginTrigger)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No trigger for boss fight!");
        if (!bossPrefab)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No boss prefab!");
        if (!bossSpawnPoint)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No boss spawn point!");
        if (!beginCutscene)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No begin cutscene!");
        if (!beginCutsceneTransform)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No begin cutscene transform!");
        if (!endCutscene)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No end cutscene!");
        if (!endCutsceneTransform)
            Debug.LogError($"component [{GetType().Name}]; game object [{name}]: No end cutscene transform!");

        beginTrigger.OnEnter.AddListener(OnTriggerEnter);
        bossPrefab.onDeath += OnBossDeath;
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        if (wasTriggered) return;

        wasTriggered = true;
        HandleTriggerEnter();
    }

    private void OnBossDeath()
    {
        CinematicPlayer.PlaySequence(endCutscene, endCutsceneTransform.position, endCutsceneTransform.rotation,
            endCutsceneTransform.localScale);

        CinematicPlayer.onEndPlay += OnCutsceneEnd;
        return;

        void OnCutsceneEnd()
        {
            HandleBossDeath();
            duringFight = true;
            CinematicPlayer.onEndPlay -= OnCutsceneEnd;
        }
    }

    public event Action OnBegin;

    public void Begin()
    {
        StartCoroutine(BeginCo());
    }

    public void Restart()
    {
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
            OnBeginInterval();
            duringFight = true;
            OnBegin?.Invoke();
            CinematicPlayer.onEndPlay -= OnCutsceneEnd;
        }
    }

    protected abstract void HandleTriggerEnter();

    protected abstract IEnumerator PrepareCo();

    protected abstract void OnBeginInterval();

    protected abstract void HandleBossDeath();

    protected abstract void OnRestartInterval();
}