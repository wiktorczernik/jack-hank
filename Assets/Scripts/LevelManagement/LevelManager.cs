using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LevelManagement
{
    public static class LevelManager
    {
        private static bool _isInitializedWithLevels;
        private static List<LevelInfo> _levels;

        public static void ValidateAndLoadLevelsSave()
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

                var levelSave = AccountManager.LoggedInPlayerAccount.GetLevelStatistics(node.LevelID);
                var lastLevelsInfo = _levels.Where(level =>
                    node.LastLevels.FirstOrDefault(last => last.LevelID == level.LevelID) != null).ToList();
                var areLastLevelsPassed = lastLevelsInfo.All(level => level.Status == LevelStatus.Passed);

                var levelStatus = levelSave switch
                {
                    { isPassed: true } when !areLastLevelsPassed => throw new Exception("Level validation failed!"),
                    null => areLastLevelsPassed ? LevelStatus.Available : LevelStatus.Unavailable,
                    _ => levelSave.isPassed ? LevelStatus.Passed : LevelStatus.Available
                };

                _levels.Add(new LevelInfo(levelStatus, node, levelSave));
            }

            _isInitializedWithLevels = true;
        }

        private static LevelDefinition[] GetLevelTreeLeafs()
        {
            var guids = AssetDatabase.FindAssets("t:LevelDefinition");

            if (guids.Length == 0) return Array.Empty<LevelDefinition>();

            var leafs = new List<LevelDefinition>();

            foreach (var guid in guids)
            {
                var definition = AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(guid));

                leafs.Add(definition);

                foreach (var lastLevel in definition.LastLevels)
                    if (leafs.Contains(lastLevel))
                        leafs.Remove(lastLevel);
            }

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