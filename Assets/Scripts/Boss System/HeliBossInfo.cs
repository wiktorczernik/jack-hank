using UnityEngine;

[RequireComponent(typeof(HeliBoss))]
public class HeliBossInfo : Boss
{
    private HeliBoss instance;
    [SerializeField] LayerMask groundMask;

    private void Awake()
    {
        instance = GetComponent<HeliBoss>();
        instance.enabled = false;
        instance.agent.enabled = false;
        instance.agent.isStopped = true;
        instance.agent.updatePosition = false;
        //instance.physics.enabled = false;
        instance.onDeath += Die;
    }

    protected override void OnActivate()
    {
        instance.target = FindFirstObjectByType<PlayerVehicle>();
        instance.enabled = true;
        if (Physics.Raycast(instance.transform.position, Vector3.down, out RaycastHit hitInfo, 50, groundMask))
        {
            Debug.Log("Placed on ground");
            instance.transform.position = hitInfo.point;
        }
        instance.agent.updatePosition = true;
        instance.agent.enabled = true;
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