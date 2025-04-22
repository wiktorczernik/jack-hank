using System;
using System.Collections;
using UnityEngine;

public abstract class BossFightManager : MonoBehaviour
{
    public event Action OnBegin;
    public bool DuringFight { get; private set; }
    public bool IsTriggered { get; private set; }
    
    [SerializeField] protected GameObject bossPrefab;
    [SerializeField] protected Transform bossSpawnPoint;
    [SerializeField] private TriggerEventEmitter bossFightTrigger;

    private void Awake()
    {
        if (!bossFightTrigger) 
            Debug.LogError($"[{GetType().Name}] component in [{name}] game object: No trigger for boss fight!");
        if (!bossPrefab) 
            Debug.LogError($"[{GetType().Name}] component in [{name}] game object: No boss prefab!");
        if (!bossSpawnPoint)
            Debug.LogError($"[{GetType().Name}] component in [{name}] game object: No boss spawn point!");
        
        bossFightTrigger.OnEnter.AddListener(OnTriggerEnter);
    }

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
        OnBeginInterval();
        DuringFight = true;
        OnBegin?.Invoke();
       
    }

    private void OnTriggerEnter(Collider triggerCollider)
    {
        IsTriggered = true;
        HandleTriggerEnter();
    }

    protected abstract void HandleTriggerEnter();

    protected abstract IEnumerator PrepareCo();

    protected abstract void OnBeginInterval();

    protected abstract void OnRestartInterval();
}