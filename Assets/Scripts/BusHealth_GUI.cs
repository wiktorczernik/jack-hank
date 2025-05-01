using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusHealth_GUI : MonoBehaviour
{
    [SerializeField] Image Visual;
    [SerializeField] TMP_Text value;
    public PlayerVehicle Bus;

    private void Update()
    {
        float val = Bus.health / Bus.maxHealth;
        Visual.fillAmount = val;
        value.text = $"HP: {Mathf.Floor(val * 1000f) / 10f}";
    }
}
