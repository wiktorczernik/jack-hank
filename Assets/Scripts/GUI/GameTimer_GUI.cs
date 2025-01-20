using TMPro;
using UnityEngine;

public class GameTimer_GUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] GameRunInfo runInfo;
 
    void Update()
    {
        label.text = runInfo.GetTimeFormatted();
    }
}
