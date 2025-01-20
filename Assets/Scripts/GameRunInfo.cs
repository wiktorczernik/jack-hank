using UnityEngine;

public class GameRunInfo : MonoBehaviour
{
    public float time = 0;
    
    const string _formatTimeTemplate = "{0}:{1}.{2}";
    
    
    public string GetTimeFormatted()
    {
        float seconds = time % 60;
        int minutes = (int)((time - seconds) / 60);
        float millisec = seconds % 1;

        string secondsPadded = Mathf.RoundToInt(seconds).ToString().PadLeft(2, '0');
        string millisecPadded = millisec.ToString().PadLeft(2, '0').Substring(2, 2);
        
        return string.Format(_formatTimeTemplate, minutes, secondsPadded, millisecPadded);
    }

    private void Update()
    {
        time += Time.deltaTime;
    }
}
