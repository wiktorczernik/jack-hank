using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    // This will be overridden by child classes
    public virtual void OnPickup(PlayerVehicleAbilities abilitiesController)
    {
        // Base implementation does nothing
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the bus
        if (other.CompareTag("Vehicle"))
        {
            // Try to get the ability controller from the bus
            PlayerVehicleAbilities abilitiesController = other.GetComponent<PlayerVehicleAbilities>();

            if (abilitiesController != null)
            {
                // Call the pickup method
                OnPickup(abilitiesController);

                // Destroy the pickup object
                Destroy(gameObject);
            }
        }
    }
}