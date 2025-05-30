using TMPro;
using UnityEngine;

public class Speedometer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] VehiclePhysics carController;
    [SerializeField] string _monoSpace = "0.8em";

    private void LateUpdate()
    {
        text.text = $"<mspace={_monoSpace}>{carController.speedKmhForward.ToString()}";
    }
}
