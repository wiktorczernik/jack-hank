using TMPro;
using UnityEngine;

public class AmmoMonitorGUI : MonoBehaviour
{
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

        Hide();
    }
    private void OnDisable()
    {
        PlayerTurret.onAllowFire -= Show;
        PlayerTurret.onDisallowFire -= Hide;
        PlayerTurret.onFire -= OnFire;
    }

    private void OnFire()
    {
        UpdateCounter();
    }
    private void UpdateCounter()
    {
        Counter.text = PlayerTurret.ammo.ToString();
    }
    private void Show()
    {
        Visuals.SetActive(true);
        UpdateCounter();
    }
    private void Hide()
    {
        Visuals.SetActive(false);
    }
}
