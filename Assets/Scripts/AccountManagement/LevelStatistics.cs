using System;
using System.Linq;
using System.Collections.Generic;

namespace AccountManagement
{
    [Serializable]
    public class LevelStatistics : ICloneable
    {
        public int LevelID;
        public bool IsPassed;
        public Dictionary<PlayerBonusTypes, int> Bonuses;
        
        public object Clone()
        {
            return new LevelStatistics()
            {
                LevelID = LevelID, 
                IsPassed = IsPassed, 
                Bonuses = Bonuses.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}