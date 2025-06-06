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
        
        
        public bool IsIncrementedWithStatistics => _statistics != null;
        public int LevelID => _definition.LevelID;
        public string LevelSceneName => _definition.SceneName;

        public LevelStatus Status { get; private set; } = LevelStatus.None;
        
        
        public Dictionary<PlayerBonusTypes, int> BountyPointsPerBonusType => 
            _statistics == null ? new Dictionary<PlayerBonusTypes, int>() : _statistics.Bonuses;
        
        public int TotalBountyPoints => _statistics == null ? 0 : _statistics.Bonuses.Sum(pair => pair.Value);

        public int[] LastLevelsIDs {
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
            Status = LevelStatus.None;
        }

        public void SetStatus(LevelStatus status)
        {
            if (_statistics == null)
            {
                Debug.LogError("LevelInfo: levelInfo has not incremented with level statistics.");
                return;
            }
            
            Status = status;
            _statistics.IsPassed = status == LevelStatus.Passed;
        }

        public void SetBountyPoints(Dictionary<PlayerBonusTypes, int> bountyPoints)
        {
            if (_statistics == null)
            {
                Debug.LogError("LevelInfo: levelInfo has not incremented with level statistics.");
                return;
            }
            
            _statistics.Bonuses = bountyPoints.ToList().ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}

