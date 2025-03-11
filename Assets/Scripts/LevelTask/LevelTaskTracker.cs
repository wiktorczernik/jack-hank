using System;
using UnityEngine;

namespace LevelTask
{
    public abstract class LevelTaskTracker : MonoBehaviour
    {
        public abstract bool IsComplete();
        protected abstract LevelTaskTracker Initialize(LevelTaskDefinition levelTaskDefinition);

        public static LevelTaskTracker CreateTracker(GameObject parent, LevelTaskDefinition levelTask) => levelTask switch
        { 
            CounterLevelTask => parent.AddComponent<CounterTracker>().Initialize(levelTask),
            TimerLevelTask => parent.AddComponent<TimerTracker>().Initialize(levelTask),
            _ => throw new NotSupportedException(),
        };

    }
}

