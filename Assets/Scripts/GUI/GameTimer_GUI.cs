using TMPro;
using UnityEngine;

public class GameTimer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    void LateUpdate()
    {
        if (GameManager.isDuringRun)
        {
            label.text = GameManager.runInfo.GetTimeFormatted();
        }
    }
}
