using UnityEngine;

[RequireComponent(typeof(HeliBoss))]
public class HeliBossInfo : Boss
{
    HeliBoss instance;

    private void Awake()
    {
        instance = GetComponent<HeliBoss>();
        instance.enabled = false;
        instance.physics.enabled = false;
    }

    protected override void OnActivate()
    {
        instance.target = FindFirstObjectByType<PlayerVehicle>();
        instance.enabled = true;
        instance.physics.enabled = true;
    }

    protected override void PrepareDie()
    {

    }
}
