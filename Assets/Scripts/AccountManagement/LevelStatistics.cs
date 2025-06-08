using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace AccountManagement
{
    // LevelStatistics przechowuje dane gracza dotyczące danego poziomu. Klasa jest Serializable dla tego żeby zapisywać
    // dane do plika JSON z kontem gracz
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