using System;
using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using JetBrains.Annotations;
using UnityEngine;

namespace LevelManagement
{
    public class LevelInfo
    {
        private readonly LevelDefinition _definition;
        [CanBeNull] private LevelStatistics _statistics;
        
        
        public bool isIncrementedWithStatistics => _statistics != null;
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

        public void IncrementLevelStatistics(LevelStatistics levelStatistics)
        {
            if (levelStatistics == null)
            {
                Debug.LogError("LevelInfo: levelStatistics is null.");
                return;
            }
            
            _statistics = levelStatistics;
        }
        
        public void DecrementLevelStatistics()
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
    }
}

