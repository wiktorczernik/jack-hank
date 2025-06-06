using System;
using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using JetBrains.Annotations;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManager : MonoBehaviour
    {
        public static bool IsIncrementedWithAccountData { private set; get; }
        private static List<LevelInfo> _levels;
        
        private static bool _isInitialized;
        private void Awake()
        {
            _levels = new List<LevelInfo>();
            LoadLevelsDefinitions();
            AccountManager.OnLoggedIn += (accountData) => IncrementAccountData(accountData.openedLevels.ToList());
            AccountManager.OnLoggedOut += (accountData) => DecrementAccountData();
            _isInitialized = true;
        }

        public static List<LevelInfo> GetLevelsList()
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load 'Essentials' scene.");
                return new List<LevelInfo>();
            }
            return _levels.ToList();
        }

        public static List<LevelInfo> GetAvailableLevels()
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load  'Essentials' scene.");
                return new List<LevelInfo>();
            }
            return _levels.Where(level => level.Status == LevelStatus.Available).ToList();
        }

        public static void SetLevelAsCompleted(LevelDefinition definition)
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load  'Essentials' scene.");
                return;
            }
            if (!IsIncrementedWithAccountData)
            {
                Debug.LogError(
                    "LevelManager: level manager is not incremented with account data. Probably you forgot to use debug account");
                return;
            }
            if (_levels.FindIndex(level => level.LevelID == definition.LevelID) == -1) return;
            
            _levels.Find(value => value.LevelID == definition.LevelID).SetStatus(LevelStatus.Passed);

            foreach (var level in _levels)
            {
                var isNextLevel = false;
                var canBeAvailable = true;

                foreach (var lastLevel in GetLastLevels(level))
                {
                    isNextLevel = lastLevel.LevelID == definition.LevelID;
                    if (lastLevel.Status != LevelStatus.Passed) canBeAvailable = false;
                }
                
                if (canBeAvailable && isNextLevel) level.SetStatus(LevelStatus.Available);
            }
        }

        [CanBeNull]
        public static LevelInfo GetLevelByName(string levelName)
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load 'Essentials' scene.");
                return null;
            }
            return _levels.Find(info => info.LevelSceneName == levelName);
        }

        [CanBeNull]
        public static LevelInfo GetLevelByID(int id)
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load 'Essentials' scene.");
                return null;
            }
            return _levels.Find(level => level.LevelID == id);
        }

        public static LevelStatistics[] GetLevelsStatistics()
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load  'Essentials' scene.");
                return Array.Empty<LevelStatistics>();
            }
            return _levels.Where(level => level.Status == LevelStatus.Passed && level.Status == LevelStatus.Available)
                .Select(level => level.GetLevelStatistics())
                .ToArray();
        }
        
        private static void LoadLevelsDefinitions()
        {
            var definitions = Resources.LoadAll<LevelDefinition>("Level Definitions/");

            foreach (var definition in definitions)
            {
                _levels.Add(new LevelInfo(definition));
            }
        }

        private static void IncrementAccountData(List<LevelStatistics> levelsStatistics)
        {
            var leafs = GetLevelTreeLeafs();
 
            var levelsWithoutSaveData = new Stack<LevelInfo>();

            foreach (var leaf in leafs)
                levelsWithoutSaveData.Push(leaf);

            while (levelsWithoutSaveData.Count > 0)
            {
                var level = levelsWithoutSaveData.Peek();
                var lastLevelsWithoutSaveData = GetNotProcessedLevels(level.LastLevelsIDs);

                if (lastLevelsWithoutSaveData.Count > 0)
                {
                    foreach (var notProcessedNode in lastLevelsWithoutSaveData)
                        levelsWithoutSaveData.Push(notProcessedNode);

                    continue;
                }

                levelsWithoutSaveData.Pop();

                var levelSave = levelsStatistics.Find(save => save.LevelID == level.LevelID);
                
                var areLastLevelsPassed = GetLastLevels(level)
                    .All(l => l.Status == LevelStatus.Passed);

                LevelStatus levelStatus;

                if (levelSave != null && levelSave.IsPassed && !areLastLevelsPassed)
                {
                    Debug.LogError("LevelManager: level save validation failed. There are passed levels with not passed last levels");
                    return;
                }

                if (levelSave == null)
                {
                    levelSave = new LevelStatistics();
                    levelStatus = areLastLevelsPassed ? LevelStatus.Available : LevelStatus.Unavailable;
                }else
                {
                    levelStatus = levelSave.IsPassed ? LevelStatus.Passed : LevelStatus.Available;
                }
                
                level.IncrementLevelStatistics(levelSave);
                level.SetStatus(levelStatus);
            }

            IsIncrementedWithAccountData = true;
        }
        
        private static void DecrementAccountData()
        {
            foreach (var levelInfo in _levels)
            {
                levelInfo.DecrementLevelStatistics();
            }

            IsIncrementedWithAccountData = false;
        }
        
        private static List<LevelInfo> GetLevelTreeLeafs()
        {
            if (_levels.Count == 0) return new List<LevelInfo>();

            var leafs = _levels.ToList();

            foreach (var level in _levels)
                foreach (var lastLevel in level.LastLevelsIDs)
                {
                    var leaf = leafs.Find(leaf => leaf.LevelID == lastLevel);
                    
                    if (leaf == null) continue;
                    
                    leafs.Remove(leaf);
                }
            
            return leafs;
        }

        private static List<LevelInfo> GetNotProcessedLevels(int[] lastLevelsIDs)
        {
            var result = new  List<LevelInfo>();

            foreach (var id in lastLevelsIDs)
            {
                var lastLevel = _levels.Find(level => level.LevelID == id);
                
                if (lastLevel != null && !lastLevel.IsIncrementedWithStatistics) result.Add(lastLevel);
            }

            return result;
        }

        private static List<LevelInfo> GetLastLevels(LevelInfo level)
        {
            if (level == null)
            {
                Debug.LogError("LevelManager: level is null");
                return new List<LevelInfo>();
            }
            
            var result = new List<LevelInfo>();

            foreach (var id in level.LastLevelsIDs)
            {
                var lastLevel = _levels.Find(l => l.LevelID == id);

                if (lastLevel == null)
                {
                    Debug.LogError($"LevelManager: level with name {level.LevelSceneName} has nonexistent last level with id {id} and name {level.LevelSceneName}");
                }
                
                result.Add(lastLevel);
            }

            return result;
        }
    }
}