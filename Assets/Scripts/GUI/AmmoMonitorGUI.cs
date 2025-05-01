using TMPro;
using UnityEngine;

using JackHank.Cinematics;

public class AmmoMonitorGUI : MonoBehaviour
{
    public bool isVisible = false;
    [SerializeField] GameObject Visuals;
    [SerializeField] TMP_Text Counter;
    [SerializeField] Color LowColor;
    [SerializeField] Color Color;
    [SerializeField] int LowColorThreshold;

    private void OnEnable()
    {
        PlayerTurret.onAllowFire += Show;
        PlayerTurret.onDisallowFire += Hide;
        PlayerTurret.onFire += OnFire;
        CinematicPlayer.onEndPlay += UpdateVisibility;
        CinematicPlayer.onFrameUpdate += OnCinematicFrame;

        Hide();
    }
    private void OnDisable()
    {
        PlayerTurret.onAllowFire -= Show;
        PlayerTurret.onDisallowFire -= Hide;
        PlayerTurret.onFire -= OnFire;
        CinematicPlayer.onEndPlay -= UpdateVisibility;
        CinematicPlayer.onFrameUpdate -= OnCinematicFrame;
    }

    private void OnFire()
    {
        Debug.Log(PlayerTurret.ammo);
        UpdateCounter();
    }
    private void UpdateCounter()
    {
        Counter.SetText(PlayerTurret.ammo.ToString());
    }
    private void Show()
    {
        isVisible = true;
        Visuals.SetActive(isVisible);
        UpdateCounter();
    }
    private void Hide()
    {
        isVisible = false;
        ForceHide();
    }
    private void ForceHide()
    {
        Visuals.SetActive(false);
    }
    private void OnCinematicFrame(CinematicSequence.CameraFrameState state)
    {
        ForceHide();
    }
    private void UpdateVisibility()
    {
        Visuals.SetActive(isVisible);
    }
}
