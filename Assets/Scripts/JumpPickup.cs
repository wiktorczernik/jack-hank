using UnityEngine;
public class JumpPickup : AbilityPickup
{
    public int chargeAmount = 1;

    public override void OnPickup(AbilitiesController abilityController)
    {
        abilityController.addJumpCharge();
    }
}