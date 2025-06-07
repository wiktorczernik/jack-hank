using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace AccountManagement
{
    [Serializable]
    public class LevelStatistics : ICloneable
    {
        public int levelID;
        public bool isPassed;
        public Dictionary<PlayerBonusTypes, int> bonuses;
        
        public object Clone()
        {
            return new LevelStatistics()
            {
                levelID = levelID, 
                isPassed = isPassed, 
                bonuses = bonuses.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}