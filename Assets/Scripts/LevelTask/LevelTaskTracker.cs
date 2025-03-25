using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace LevelTask
{
    public abstract class LevelTaskTracker : MonoBehaviour
    {
        public abstract bool IsComplete();
        protected abstract LevelTaskTracker Initialize(LevelTaskDefinition levelTaskDefinition);

        public static LevelTaskTracker CreateTracker(GameObject parent, LevelTaskDefinition levelTask)
        {
            LevelTaskTracker tracker = levelTask switch
            {
                CounterLevelTask => parent.AddComponent<CounterTracker>().Initialize(levelTask),
                TimerLevelTask => parent.AddComponent<TimerTracker>().Initialize(levelTask),
                _ => null,
            };
            if (tracker == null)
                Debug.LogError("Ty dolboev!?");
            return tracker;
        } 
    }
}

