using TMPro;
using UnityEngine;

public class BountyCounter_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    private void LateUpdate()
    {
        if (GameManager.isDuringRun)
        {
            label.text = GameManager.runInfo.AllBountyPoints.ToString();
        }
    }
}
