using UnityEngine;
public class JumpPickup : AbilityPickup
{
    public int chargeAmount = 1;

    public override void OnPickup(AbilitiesController abilityController)
    {
        // Increment jump charges
        abilityController.jumpCharges += chargeAmount;
        Debug.Log("Jump charge added: " + chargeAmount);
    }
}