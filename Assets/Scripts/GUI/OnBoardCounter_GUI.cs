using UnityEngine;
using TMPro;

public class OnBoardCounter_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    private void LateUpdate()
    {
        if (GameManager.IsDuringRun)
        {
            label.text = GameManager.RunInfo.PassengersOnBoard.ToString();
        }
    }
}
