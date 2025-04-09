using TMPro;
using UnityEngine;

public class Speedometer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] VehiclePhysics carController;

    private void LateUpdate()
    {
        text.text = carController.speedKmhForward.ToString();
    }
}
