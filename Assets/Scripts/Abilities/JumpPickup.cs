using UnityEngine;
public class JumpPickup : AbilityPickup
{
    public int chargeAmount = 1;

    public override void OnPickup(PlayerVehicleAbilities abilityController)
    {
        //abilityController.addJumpCharge();
    }
}