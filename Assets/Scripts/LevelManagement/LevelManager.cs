using System;
using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using UnityEngine;

namespace LevelManagement
{
    public static class LevelManager
    {
        private static bool _isInitializedWithLevels;
        private static List<LevelInfo> _levels;

        public static void InitializeAndValidateLevelsTree(List<LevelStatistics> levelsStatistics)
        {
            if (_isInitializedWithLevels) throw new Exception("LevelManager has already been initialized!");

            var leafs = GetLevelTreeLeafs();
 
            var levelDefinitionsNodes = new Stack<LevelDefinition>();
            _levels = new List<LevelInfo>();

            foreach (var leaf in leafs)
                levelDefinitionsNodes.Push(leaf);

            while (levelDefinitionsNodes.Count > 0)
            {
                var node = levelDefinitionsNodes.Peek();
                var notProcessedNodes = GetNotProcessedLevels(node.LastLevels);

                if (notProcessedNodes.Count > 0)
                {
                    foreach (var notProcessedNode in notProcessedNodes)
                        levelDefinitionsNodes.Push(notProcessedNode);

                    continue;
                }

                levelDefinitionsNodes.Pop();

                var levelSave = levelsStatistics.Find(level => level.LevelID == node.LevelID);
                var lastLevelsInfo = _levels.Where(level =>
                    node.LastLevels.FirstOrDefault(last => last.LevelID == level.LevelID) != null).ToList();
                var areLastLevelsPassed = lastLevelsInfo.All(level => level.Status == LevelStatus.Passed);

                var levelStatus = levelSave switch
                {
                    { IsPassed: true } when !areLastLevelsPassed => throw new Exception("Level validation failed!"),
                    null => areLastLevelsPassed ? LevelStatus.Available : LevelStatus.Unavailable,
                    _ => levelSave.IsPassed ? LevelStatus.Passed : LevelStatus.Available
                };

                _levels.Add(new LevelInfo(levelStatus, node, levelSave));
            }

            _isInitializedWithLevels = true;
        }

        public static void DeinitializeLevelsTree()
        {
            if (!_isInitializedWithLevels) throw new Exception("LevelManager has not been initialized!");
            
            _levels.Clear();
            _isInitializedWithLevels = false;
        }

        public static List<LevelInfo> GetLevelsList()
        {
            if (!_isInitializedWithLevels) throw new Exception("LevelManager has not been initialized!");

            return _levels.ToList();
        }

        public static List<LevelInfo> GetAvailableLevels()
        {
            return _levels.Where(level => level.Status == LevelStatus.Available).ToList();
        }

        private static LevelDefinition[] GetLevelTreeLeafs()
        {
            var definitions = Resources.LoadAll<LevelDefinition>("Level Definitions/");
  
            if (definitions.Length == 0) return Array.Empty<LevelDefinition>();

            var leafs = new List<LevelDefinition>();
            leafs.AddRange(definitions);

            foreach (var definition in definitions)
                foreach (var lastLevel in definition.LastLevels)
                    if (leafs.Contains(lastLevel))
                        leafs.Remove(lastLevel);
            

            return leafs.ToArray();
        }

        private static List<LevelDefinition> GetNotProcessedLevels(LevelDefinition[] lastLevels)
        {
            return lastLevels
                .Where(last => _levels.FindIndex(info => info.LevelID == last.LevelID) == -1)
                .ToList();
        }
    }
}