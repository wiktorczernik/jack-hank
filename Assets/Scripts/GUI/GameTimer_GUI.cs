using TMPro;
using UnityEngine;

public class GameTimer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    void LateUpdate()
    {
        if (GameManager.IsDuringRun)
        {
            label.text = GameManager.RunInfo.GetTimeFormatted();
        }
    }
}
