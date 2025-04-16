using UnityEngine;

public class ManaPickup : AbilityPickup
{
    public int ManaPickupAmount;

    // Mana pickup implementation
    public override void OnPickup(PlayerVehicleAbilities abilitiesController)
    {
        abilitiesController.availableMana += ManaPickupAmount;
        print(abilitiesController.availableMana);
    }
}
