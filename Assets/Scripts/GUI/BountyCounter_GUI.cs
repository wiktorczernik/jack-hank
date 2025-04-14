using TMPro;
using UnityEngine;

public class BountyCounter_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    private void LateUpdate()
    {
        if (GameManager.IsDuringRun)
        {
            label.text = GameManager.RunInfo.AllBountyPoints.ToString();
        }
    }
}
