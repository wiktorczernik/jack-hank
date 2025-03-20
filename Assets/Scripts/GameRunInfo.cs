using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRunInfo
{
    public float Time = 0;
    public int AllBountyPoints => _pointsByBonusTypes.Sum(pair => pair.Value) ;
    public int PassengersOnBoard = 0;
    
    private const string FormatTimeTemplate = "{0}:{1}.{2}";
    private readonly Dictionary<PlayerBonusTypes, int> _pointsByBonusTypes = new()
    {
        { PlayerBonusTypes.DestructionCombo , 0},
        { PlayerBonusTypes.Drift , 0},
        { PlayerBonusTypes.Flying, 0},
        { PlayerBonusTypes.Passenger, 0},
    };

    public string GetTimeFormatted()
    {
        var seconds = Time % 60;
        var minutes = (int)((Time - seconds) / 60);
        var millisecond = seconds % 1;

        var secondsPadded = Mathf.RoundToInt(seconds).ToString().PadLeft(2, '0');
        var millisecondPadded = millisecond.ToString().Substring(2, 2).PadLeft(2, '0');
        
        return string.Format(FormatTimeTemplate, minutes, secondsPadded, millisecondPadded);
    }

    public void ChangeBonusBountyBy(int value, PlayerBonusTypes bonusType)
    {
        if (!_pointsByBonusTypes.ContainsKey(bonusType))
            throw new NotImplementedException(
                "Bonus type has not implemented in GameRunInfo!!! Please add a BonusType!");
        
        if (bonusType == PlayerBonusTypes.Passenger) AddPassenger();
        
        _pointsByBonusTypes[bonusType] += value;
    }
    
    public void AddPassenger()
    {
        PassengersOnBoard++;
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
