using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using JetBrains.Annotations;
using UnityEngine;

namespace LevelManagement
{
    //LevelInfo to runtime'owa reprezentacja poziomu w grze.
    public class LevelInfo
    {
        private readonly LevelDefinition _definition;
        [CanBeNull] private LevelStatistics _statistics;
        
        public bool containPlayerProgressData => _statistics != null;
        public int levelID => _definition.LevelID;
        public string levelSceneName => _definition.SceneName;

        public LevelStatus status { get; private set; } = LevelStatus.None;
        
        public Dictionary<PlayerBonusTypes, int> bountyPointsPerBonusType => 
            _statistics == null ? new Dictionary<PlayerBonusTypes, int>() : _statistics.bonuses;
        
        public int totalBountyPoints => _statistics == null ? 0 : _statistics.bonuses.Sum(pair => pair.Value);

        public int[] lastLevelsIDs {
            get
            {
                return _definition.LastLevels.Select(def => def.LevelID).ToArray();
            }
            
        }

        public LevelInfo(LevelDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogError(("LevelInfo: definition is null."));
                return;
            }
            
            _definition = definition;
        }

        [CanBeNull]
        public LevelStatistics GetLevelStatistics()
        {
            return _statistics?.Clone() as LevelStatistics;
        }

        public void AddPlayerProgressData(LevelStatistics levelStatistics)
        {
            if (levelStatistics == null)
            {
                Debug.LogError("LevelInfo: levelStatistics is null.");
                return;
            }

            if (levelStatistics.levelID != _definition.LevelID)
            {
                Debug.LogError("LevelInfo: levelStatistics and levelDefinition has different levelID.");
                return;
            }
            
            _statistics = levelStatistics;
            if (_statistics.bonuses == null)
                _statistics.bonuses = new Dictionary<PlayerBonusTypes, int>();
        }
        
        public void RemovePlayerProgressData()
        {
            _statistics = null;
            status = LevelStatus.None;
        }

        public void SetStatus(LevelStatus status)
        {
            if (_statistics == null)
            {
                Debug.LogError("LevelInfo: levelInfo has not incremented with level statistics.");
                return;
            }
            
            this.status = status;
            _statistics.isPassed = status == LevelStatus.Passed;
        }

        public void SetBountyPoints(Dictionary<PlayerBonusTypes, int> bountyPoints)
        {
            if (_statistics == null)
            {
                Debug.LogError("LevelInfo: levelInfo has not incremented with level statistics.");
                return;
            }
            
            _statistics.bonuses = bountyPoints.ToList().ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void IncrementBountyPoints(Dictionary<PlayerBonusTypes, int> bonusPoints)
        {
            if (_statistics == null)
            {
                Debug.LogError("LevelInfo: levelInfo has not incremented with level statistics.");
                return;
            }

            foreach ((var bonusType, var bonusValue) in bonusPoints)
            {
                if (!_statistics.bonuses.ContainsKey(bonusType)) _statistics.bonuses.Add(bonusType, bonusValue);
                else _statistics.bonuses[bonusType] += bonusValue;
            }
        }
    }
}

