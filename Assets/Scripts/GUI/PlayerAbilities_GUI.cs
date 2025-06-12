using UnityEngine;

using JackHank.Cinematics;

public class PlayerAbilities_GUI : MonoBehaviour
{
    public PlayerVehicleAbilities abilities;

    public PlayerAbility_GUI nitro;
    public PlayerAbility_GUI jump;
    
    private void OnEnable()
    {
        nitro.Initialize(abilities.nitro);
        jump.Initialize(abilities.jump);
        nitro.gameObject.SetActive(true);
        jump.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        nitro.Free();
        jump.Free();
        nitro.gameObject.SetActive(false);
        jump.gameObject.SetActive(false);
    }
    private void Update()
    {
        nitro.gameObject.SetActive(nitro.ability.state != PlayerVehicleAbility.AbilityState.TurnOff);
        jump.gameObject.SetActive(jump.ability.state != PlayerVehicleAbility.AbilityState.TurnOff);
    }
}
