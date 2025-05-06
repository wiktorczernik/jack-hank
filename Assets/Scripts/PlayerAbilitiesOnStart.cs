using UnityEngine;

public class PlayerAbilitiesOnLevelStart : MonoBehaviour
{
    [SerializeField] private PlayerVehicleAbilities playerAbilities;
    [SerializeField] private bool hasNitro = true;
    [SerializeField] private bool hasJump = true;

    private void Start()
    {
        if (playerAbilities.jump)
        {
            if (hasJump) playerAbilities.jump.TurnOn();
            else playerAbilities.jump.TurnOff();
        }
        
        if (playerAbilities.nitro)
        {
            if (hasNitro) playerAbilities.nitro.TurnOn();
            else playerAbilities.nitro.TurnOff();
        }
    }
}