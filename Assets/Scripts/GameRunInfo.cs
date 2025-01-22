using UnityEngine;

public class GameRunInfo
{
    public float time = 0;
    public int bountyPoints = 0;
    public int passengersOnBoard = 0;
    
    const string _formatTimeTemplate = "{0}:{1}.{2}";
    
    
    public string GetTimeFormatted()
    {
        float seconds = time % 60;
        int minutes = (int)((time - seconds) / 60);
        float millisec = seconds % 1;

        string secondsPadded = Mathf.RoundToInt(seconds).ToString().PadLeft(2, '0');
        string millisecPadded = millisec.ToString().Substring(2, 2).PadLeft(2, '0');
        
        return string.Format(_formatTimeTemplate, minutes, secondsPadded, millisecPadded);
    }
    public void AddBountyPoints(int amount)
    {
        bountyPoints += amount;
    }
    public void AddPassenger()
    {
        passengersOnBoard++;
    }
}

public enum GameRunPhase
{ 
    Loading,
    Preparation,
    Introduction,
    Countdown,
    Play,
    Ending,
    Aftermath
}
