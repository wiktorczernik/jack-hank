using System;
using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using JetBrains.Annotations;
using UnityEngine;

namespace LevelManagement
{
    // To statyczna klasa odpowiedzialna za zarządzanie poziomami – pozwala na odczyt i modyfikację danych związanych
    // z poziomami.
    public class LevelManager : MonoBehaviour
    {
        public static bool containPlayerProgressData { private set; get; }
        private static List<LevelInfo> _levels;
        
        private static bool _isInitialized;
        private void Awake()
        {
            _levels = new List<LevelInfo>();
            LoadLevelsDefinitions();
            AccountManager.onLoggedIn += (accountData) => AddPlayerProgressData(accountData.openedLevels.ToList());
            AccountManager.onLoggedOut += (accountData) => DecrementAccountData();
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
            return _levels.Where(level => level.status == LevelStatus.Available).ToList();
        }

        public static void SetLevelAsCompleted(LevelDefinition definition)
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load  'Essentials' scene.");
                return;
            }
            if (!containPlayerProgressData)
            {
                Debug.LogError(
                    "LevelManager: level manager is not incremented with account data. Probably you forgot to use debug account");
                return;
            }
            if (_levels.FindIndex(level => level.levelID == definition.LevelID) == -1) return;
            
            _levels.Find(value => value.levelID == definition.LevelID).SetStatus(LevelStatus.Passed);

            foreach (var level in _levels)
            {
                var isNextLevel = false;
                var canBeAvailable = true;

                foreach (var lastLevel in GetLastLevels(level))
                {
                    isNextLevel = lastLevel.levelID == definition.LevelID;
                    if (lastLevel.status != LevelStatus.Passed) canBeAvailable = false;
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
            return _levels.Find(info => info.levelSceneName == levelName);
        }

        [CanBeNull]
        public static LevelInfo GetLevelByID(int id)
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load 'Essentials' scene.");
                return null;
            }
            return _levels.Find(level => level.levelID == id);
        }

        public static LevelStatistics[] GetLevelsStatistics()
        {
            if (!_isInitialized)
            {
                Debug.LogError("LevelManager: not initialized. Probably you forgot to load  'Essentials' scene.");
                return Array.Empty<LevelStatistics>();
            }
            return _levels.Where(level => level.status == LevelStatus.Passed && level.status == LevelStatus.Available)
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

        private static void AddPlayerProgressData(List<LevelStatistics> levelsStatistics)
        {
            var leafs = GetLevelTreeLeafs();
 
            var levelsWithoutSaveData = new Stack<LevelInfo>();

            foreach (var leaf in leafs)
                levelsWithoutSaveData.Push(leaf);

            while (levelsWithoutSaveData.Count > 0)
            {
                var level = levelsWithoutSaveData.Peek();
                var lastLevelsWithoutSaveData = GetNotProcessedLevels(level.lastLevelsIDs);

                if (lastLevelsWithoutSaveData.Count > 0)
                {
                    foreach (var notProcessedNode in lastLevelsWithoutSaveData)
                        levelsWithoutSaveData.Push(notProcessedNode);

                    continue;
                }

                levelsWithoutSaveData.Pop();

                var levelSave = levelsStatistics.Find(save => save.levelID == level.levelID);
                
                var areLastLevelsPassed = GetLastLevels(level)
                    .All(l => l.status == LevelStatus.Passed);

                LevelStatus levelStatus;

                if (levelSave != null && levelSave.isPassed && !areLastLevelsPassed)
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
                    levelStatus = levelSave.isPassed ? LevelStatus.Passed : LevelStatus.Available;
                }
                
                level.AddPlayerProgressData(levelSave);
                level.SetStatus(levelStatus);
            }

            containPlayerProgressData = true;
        }
        
        private static void DecrementAccountData()
        {
            foreach (var levelInfo in _levels)
            {
                levelInfo.RemovePlayerProgressData();
            }

            containPlayerProgressData = false;
        }
        
        private static List<LevelInfo> GetLevelTreeLeafs()
        {
            if (_levels.Count == 0) return new List<LevelInfo>();

            var leafs = _levels.ToList();

            foreach (var level in _levels)
                foreach (var lastLevel in level.lastLevelsIDs)
                {
                    var leaf = leafs.Find(leaf => leaf.levelID == lastLevel);
                    
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
                var lastLevel = _levels.Find(level => level.levelID == id);
                
                if (lastLevel != null && !lastLevel.containPlayerProgressData) result.Add(lastLevel);
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

            foreach (var id in level.lastLevelsIDs)
            {
                var lastLevel = _levels.Find(l => l.levelID == id);

                if (lastLevel == null)
                {
                    Debug.LogError($"LevelManager: level with name {level.levelSceneName} has nonexistent last level with id {id} and name {level.levelSceneName}");
                }
                
                result.Add(lastLevel);
            }

            return result;
        }
    }
}