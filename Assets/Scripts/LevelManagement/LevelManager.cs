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

        private void Awake()
        {
            Debug.Log("LevelManager: initialization");
            _levels = new List<LevelInfo>();
            LoadLevelDefinitions();
            AccountManager.OnLoggedIn += (accountData) => IncrementAccountData(accountData.openedLevels.ToList());
            AccountManager.OnLoggedOut += (accountData) => DecrementAccountData();
        }

        public static List<LevelInfo> GetLevelsList()
        {
            return _levels.ToList();
        }

        public static List<LevelInfo> GetAvailableLevels()
        {
            return _levels.Where(level => level.Status == LevelStatus.Available).ToList();
        }

        public static void SetLevelAsCompleted(LevelDefinition definition)
        {
            if (_levels.FindIndex(level => level.LevelID == definition.LevelID) == -1) return;
            
            _levels.Find(value => value.LevelID == definition.LevelID).SetStatus(LevelStatus.Passed);

            foreach (var level in _levels)
            {
                var isNextLevel = false;
                var isAvailable = true;

                foreach (var lastLevelId in level.LastLevelsIDs)
                {
                    var lastLevel = _levels.First(level => level.LevelID == lastLevelId);
                    isNextLevel = lastLevel.LevelID == definition.LevelID;
                    if (lastLevel.Status != LevelStatus.Passed) isAvailable = false;
                }
                
                if (isAvailable && isNextLevel) level.SetStatus(LevelStatus.Available);
            }
            
            Debug.Log("Level set as completed log");
            foreach (var level in _levels)
            {
                Debug.Log($"{level.LevelID}: {level.LevelSceneName}, {level.Status}: {level.IsIncrementedWithStatistics}");
            }
        }

        [CanBeNull]
        public static LevelInfo GetLevelByName(string levelName)
        {
            return _levels.Find(info => info.LevelSceneName == levelName);
        }

        public static LevelStatistics[] GetLevelsStatistics()
        {
            return _levels.Where(level => level.Status == LevelStatus.Passed && level.Status == LevelStatus.Available)
                .Select(level => level.GetLevelStatistics())
                .ToArray();
        }
        
        private static void LoadLevelDefinitions()
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

            Debug.Log("Statistic Log");
            foreach (var statistic in levelsStatistics)
            {
                Debug.Log(statistic);
            }
 
            var nodes = new Stack<LevelInfo>();

            foreach (var leaf in leafs)
                nodes.Push(leaf);

            while (nodes.Count > 0)
            {
                var node = nodes.Peek();
                var notProcessedNodes = GetNotProcessedLevels(node.LastLevelsIDs);

                if (notProcessedNodes.Count > 0)
                {
                    foreach (var notProcessedNode in notProcessedNodes)
                        nodes.Push(notProcessedNode);

                    continue;
                }

                nodes.Pop();

                var levelSave = levelsStatistics.Find(level => level.LevelID == node.LevelID);
                
                var lastLevelsInfo = _levels.Where(level =>
                    node.LastLevelsIDs.ToList().FindIndex(id => id == level.LevelID) != -1).ToList();
                 
                var areLastLevelsPassed = lastLevelsInfo.All(level => level.Status == LevelStatus.Passed);

                var levelStatus = levelSave switch
                {
                    { IsPassed: true } when !areLastLevelsPassed => throw new Exception("Level validation failed!"),
                    null => areLastLevelsPassed ? LevelStatus.Available : LevelStatus.Unavailable,
                    _ => levelSave.IsPassed ? LevelStatus.Passed : LevelStatus.Available
                };
                
                node.IncrementLevelStatistics(levelSave ?? new LevelStatistics());
                node.SetStatus(levelStatus);
            }

            IsIncrementedWithAccountData = true;
            
            Debug.Log("Level processed log");
            foreach (var level in _levels)
            {
                Debug.Log($"{level.LevelID}: {level.LevelSceneName}, {level.Status}: {level.IsIncrementedWithStatistics}");
            }
        }
        
        private static void DecrementAccountData()
        {
            foreach (var levelInfo in _levels)
            {
                levelInfo.DecrementLevelStatistics();
            }
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
    }
}