using System;

namespace AccountManagement
{
    [Serializable]
    public class LevelStatistics : ICloneable
    {
        public int levelID;
        public bool isPassed;
        
        public object Clone()
        {
            return new LevelStatistics() { levelID = levelID, isPassed = isPassed };
        }
    }
}