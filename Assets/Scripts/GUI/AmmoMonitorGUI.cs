using TMPro;
using UnityEngine;

public class AmmoMonitorGUI : MonoBehaviour
{
    [SerializeField] TMP_Text Counter;
    [SerializeField] Color LowColor;
    [SerializeField] Color Color;
    [SerializeField] int LowColorThreshold;
    [SerializeField] PlayerTurret TargetTurret;

    public bool isTurnedOn = true;

    private void Update()
    {
        Counter.gameObject.SetActive(isTurnedOn);
        if (!isTurnedOn) return;

        Counter.text = TargetTurret.ammo.ToString();
        Counter.color = TargetTurret.ammo <= LowColorThreshold ? LowColor : Color;
    }
}
