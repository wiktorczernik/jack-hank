using UnityEngine;

public class SpeedBoostPickup : AbilityPickup
{
    public int chargeAmount = 1;

    public override void OnPickup(PlayerVehicleAbilities abilityController)
    {
        // Increment speed boost charges
        //abilityController.addSpeedBoostCharge();
    }
}