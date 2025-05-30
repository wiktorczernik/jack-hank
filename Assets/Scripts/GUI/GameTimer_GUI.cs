using TMPro;
using UnityEngine;

public class GameTimer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] string _monoSpace;

    void LateUpdate()
    {
        if (GameManager.IsDuringRun)
        {
            label.text = $"<mspace={_monoSpace}>{GameManager.RunInfo.GetTimeFormatted()}";
        }
    }
}
