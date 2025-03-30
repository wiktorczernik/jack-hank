using System;
using AccountManagement;

namespace LevelManagement
{
    public class LevelInfo
    {
        private readonly LevelDefinition _definition;
        private readonly LevelStatistics _statistics;
        private LevelStatus _status;
    
        public readonly int LevelID;
        public string LevelSceneName => _definition.SceneName;

        public LevelStatus Status
        {
            get => _status;
            set
            {
                _statistics.IsPassed = value == LevelStatus.Passed;

                _status = value;
            }
        }
    
        private readonly LevelInfo[] _lastLevels;

        public LevelInfo[] LastLevels => _lastLevels.Clone() as LevelInfo[];

        public LevelInfo(LevelStatus status, LevelDefinition definition, LevelStatistics statistics = null)
        {
            if (definition == null) throw new Exception("Level definition is null");
            if (statistics is not null && statistics.LevelID != definition.LevelID) throw new Exception("LevelID doesn't match");

            _statistics = statistics != null ? statistics.Clone() as LevelStatistics : new LevelStatistics();
            LevelID = definition.LevelID;
            Status = status;
            _definition = definition;
        }

        public LevelStatistics GetLevelStatistics()
        {
            return _statistics.Clone() as LevelStatistics;
        }
    }
}

