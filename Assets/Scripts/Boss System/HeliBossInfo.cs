using UnityEngine;

[RequireComponent(typeof(HeliBoss))]
public class HeliBossInfo : Boss
{
    private HeliBoss instance;

    private void Awake()
    {
        instance = GetComponent<HeliBoss>();
        instance.enabled = false;
        instance.agent.isStopped = true;
        //instance.physics.enabled = false;
        instance.onDeath += Die;
    }

    protected override void OnActivate()
    {
        instance.target = FindFirstObjectByType<PlayerVehicle>();
        instance.enabled = true;
        instance.agent.isStopped = false;
        instance.StartTotalFlyingDestruction();
        
        BossHPManager.DisplayBoss(instance);
    }

    protected override void PrepareDie()
    {
        instance.enabled = false;
        instance.agent.isStopped = true;
        instance.onDeath -= Die;
        instance.gameObject.SetActive(false);
    }
}