using UnityEngine;

namespace LevelTask
{
    public class TimerLevelTask : LevelTaskDefinition
    {
        [SerializeField] private float timerDurationInSeconds;
        
        public float TimerDurationInSeconds => timerDurationInSeconds;
    }
}




