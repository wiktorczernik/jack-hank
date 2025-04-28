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
        instance.onDeath += Die;
    }

    protected override void OnActivate()
    {
        instance.target = FindFirstObjectByType<PlayerVehicle>();
        instance.enabled = true;
        instance.physics.enabled = true;
        instance.onDeath -= Die;
    }

    protected override void PrepareDie()
    {

    }
}
