using UnityEngine;
using JackHank.Cinematics;

public class RunUIController_GUI : MonoBehaviour
{
    private void Awake()
    {
        CinematicPlayer.onBeginPlay += Disable;
        CinematicPlayer.onEndPlay += Enable;
    }
    private void OnDestroy()
    {
        CinematicPlayer.onBeginPlay -= Disable;
        CinematicPlayer.onEndPlay -= Enable;
    }

    void Enable()
    {
        gameObject.SetActive(true);
    }
    void Disable()
    {
        gameObject.SetActive(false);
    }
}
