using System;
using System.Collections.Generic;
using System.Linq;
using LevelManagement;

namespace AccountManagement
{
    public class PlayerAccount
    {
        private readonly PlayerAccountData _playerAccountData;

        public PlayerAccount(PlayerAccountData data)
        {
            _playerAccountData = data.Clone() as PlayerAccountData;
        }

        public PlayerAccount(string accountName)
        {
            _playerAccountData = new PlayerAccountData { AccountName = accountName, openedLevels = Array.Empty<LevelStatistics>()};
        }

        public LevelStatistics GetLevelStatistics(int levelID)
        {
            return _playerAccountData.openedLevels.First(level => level.LevelID == levelID).Clone() as LevelStatistics;
        }

        public bool HasWatchedIntoCutscene()
        {
            return _playerAccountData.hasWatchedIntroCutscene;
        }
        
        public void SetPlayerWatchedIntroCutscene()
        {
            _playerAccountData.hasWatchedIntroCutscene = true;
        }

        public PlayerAccountData GetData()
        {
            return _playerAccountData.Clone() as PlayerAccountData;
        }

        public string GetAccountName()
        {
            return _playerAccountData.AccountName;
        }

        public void SetLevelAsCompleted(LevelDefinition completedLevel, Dictionary<PlayerBonusTypes, int> bonuses)
        {
            if (_playerAccountData.openedLevels.FirstOrDefault(level => level.LevelID == completedLevel.LevelID) ==
                null) return;

            var playerLevelStats = _playerAccountData.openedLevels;
            
            var newLevelStats = new LevelStatistics[playerLevelStats.Length + 1].Concat(playerLevelStats).ToArray();
            var completedLevelStats = new LevelStatistics
            {
                LevelID = completedLevel.LevelID,
                IsPassed = true,
                Bonuses = bonuses
            };

            _playerAccountData.bouncy += bonuses.Sum(pair => pair.Value);
            newLevelStats[playerLevelStats.Length] = completedLevelStats;
            _playerAccountData.openedLevels = newLevelStats;
        }
    }
}