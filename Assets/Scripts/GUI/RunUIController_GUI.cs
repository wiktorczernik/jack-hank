using UnityEngine;
using JackHank.Cinematics;

public class RunUIController_GUI : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        CinematicPlayer.onBeginPlay += Disable;
        CinematicPlayer.onEndPlay += Enable;
    }
    private void OnDestroy()
    {
        CinematicPlayer.onBeginPlay -= Disable;
        CinematicPlayer.onEndPlay -= Enable;
    }

    private void Update()
    {
        _canvasGroup.alpha = Time.timeScale;
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
