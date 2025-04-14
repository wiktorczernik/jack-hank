using System.Linq;
using LevelManagement;

namespace AccountManagement
{
    public class PlayerAccount
    {
        private PlayerAccountData _playerAccountData;

        public PlayerAccount(PlayerAccountData data)
        {
            _playerAccountData = data.Clone() as PlayerAccountData;
        }

        public PlayerAccount(string accountName)
        {
            _playerAccountData = new PlayerAccountData() { AccountName = accountName };
        }

        public LevelStatistics GetLevelStatistics(int levelID)
        {
            return _playerAccountData.openedLevels.First(level => level.LevelID == levelID).Clone() as LevelStatistics;
        }



        public PlayerAccountData GetData()
        {
            return _playerAccountData.Clone() as PlayerAccountData;
        }

        public string GetAccountName()
        {
            return _playerAccountData.AccountName;
        }
    }
}