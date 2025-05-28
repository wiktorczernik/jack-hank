using UnityEngine;

using JackHank.Cinematics;

public class PlayerAbilities_GUI : MonoBehaviour
{
    public PlayerVehicleAbilities abilities;

    public PlayerAbility_GUI nitro;
    public PlayerAbility_GUI jump;
    
    private void OnEnable()
    {
        OnEndCinematic();
    }
    private void OnDisable()
    {
        OnBeginCinematic();
    }

    private void OnBeginCinematic()
    {
        nitro.gameObject.SetActive(false);
        jump.gameObject.SetActive(false);
    }
    private void OnEndCinematic()
    {
        Debug.Log("On End Cinematic");
        nitro.ability = abilities.nitro;
        jump.ability = abilities.jump;
        nitro.gameObject.SetActive(true);
        jump.gameObject.SetActive(true);
    }
}
