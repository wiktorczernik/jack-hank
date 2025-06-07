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
            _playerAccountData = new PlayerAccountData { accountName = accountName, openedLevels = Array.Empty<LevelStatistics>()};
        }

        public LevelStatistics GetLevelStatistics(int levelID)
        {
            return _playerAccountData.openedLevels.First(level => level.levelID == levelID).Clone() as LevelStatistics;
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
            return _playerAccountData.accountName;
        }
    }
}