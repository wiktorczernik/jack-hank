using UnityEngine;
using UnityEngine.Serialization;

namespace LevelTask
{
    public class CounterLevelTask : LevelTaskDefinition
    {
        [SerializeField] private int amountToCollect;
        [SerializeField] private SmashableType smashableType;

        public int AmountToCollect => amountToCollect;
        public SmashableType SmashableType=> smashableType;
    }
}


