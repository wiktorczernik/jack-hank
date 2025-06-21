using System;
using System.Linq;
using System.Collections.Generic;

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
            var clone = new LevelStatistics();

            clone.levelID = levelID;
            clone.isPassed = isPassed;
            
            clone.bonuses = bonuses == null ? 
                new Dictionary<PlayerBonusTypes, int>() : 
                bonuses.ToDictionary(x => x.Key, x => x.Value);

            return clone;
        }
    }
}