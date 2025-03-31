using UnityEngine;

public class SpeedBoostPickup : AbilityPickup
{
    public int chargeAmount = 1;

    public override void OnPickup(AbilitiesController abilityController)
    {
        // Increment speed boost charges
        abilityController.speedBoostCharges += chargeAmount;
        Debug.Log("Speed boost charge added: " + chargeAmount);
    }
}