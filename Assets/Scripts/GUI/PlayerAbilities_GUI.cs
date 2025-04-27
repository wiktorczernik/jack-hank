using UnityEngine;

public class PlayerAbilities_GUI : MonoBehaviour
{
    public PlayerVehicleAbilities abilities;

    public PlayerAbility_GUI nitro;
    public PlayerAbility_GUI jump;

    private void Awake()
    {
        nitro.ability = abilities.nitro;
        jump.ability = abilities.jump;
    }
    private void OnEnable()
    {
        CinematicPlayer.onBeginPlay += OnBeginCinematic;
        CinematicPlayer.onEndPlay += OnEndCinematic;
    }
    private void OnDisable()
    {
        CinematicPlayer.onBeginPlay -= OnBeginCinematic;
        CinematicPlayer.onEndPlay -= OnEndCinematic;
    }

    private void OnBeginCinematic()
    {
        nitro.gameObject.SetActive(false);
        jump.gameObject.SetActive(false);
    }
    private void OnEndCinematic()
    {
        nitro.gameObject.SetActive(true);
        jump.gameObject.SetActive(true);
    }
}
