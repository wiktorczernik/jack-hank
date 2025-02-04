using System;

namespace AccountManagement
{
    [Serializable]
    public class PlayerAccountData : ICloneable
    {
        [NonSerialized] public string AccountName;
        public int bouncy;
        public LevelStatistics[] openedLevels;
        
        public object Clone()
        {
            var clone = new PlayerAccountData()
            {
                AccountName = AccountName,
                bouncy = bouncy,
                openedLevels = new LevelStatistics[openedLevels ?.Length ?? 0]
            };

            if (openedLevels == null) return clone;

            for (var i = 0; i < openedLevels.Length; i++)
            {
                clone.openedLevels[i] = openedLevels[i].Clone() as LevelStatistics;
            }

            return clone;
        }
    }
}

