using UnityEngine;

public class PlayerNitroAbility : PlayerVehicleAbility
{
    [Header("Nitro Settings")]
    public float maxSpeedBonus = 100f;
    public float accelerationBonus = 2500f;

    protected override void OnWorkBegin()
    {
        physics.maxForwardSpeed += maxSpeedBonus;
        physics.forwardAcceleration += accelerationBonus;
    }
    protected override void OnWorkTick()
    {
        physics.Accelerate(1);
    }
    protected override void OnWorkEnd()
    {
        physics.maxForwardSpeed -= maxSpeedBonus;
        physics.forwardAcceleration -= accelerationBonus;
    }
}
