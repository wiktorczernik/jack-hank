using System;
using System.Linq;
using UnityEngine;

namespace AccountManagement
{
    // Jest to po prostu opakowanie enkapsulujące dla AccountData.
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

        public void SetPlayTime(int timestamp)
        {
            if (timestamp < 0)
            {
                Debug.LogError("PlayerAccount: SetPlayTime called with negative timestamp.");
                return;
            }
            
            _playerAccountData.playTimeTimestamp = timestamp;
        }

        public void IncrementPlayTime(int deltaTimestamp)
        {
            if (deltaTimestamp < 0)
            {
                Debug.LogError("PlayerAccount: IncrementPlayTime called with negative deltaTimestamp.");
                return;
            }
            
            _playerAccountData.playTimeTimestamp += deltaTimestamp;
        }

        public int GetPlayTime()
        {
            return _playerAccountData.playTimeTimestamp;
        }

    

        public void SetPassengersAmount(int amount)
        {
            if (amount < 0)
            {
                Debug.LogError("PlayerAccount: SetPassengersAmount called with negative amount.");
                return;
            }

            _playerAccountData.savedPassengers = amount;
        }

        public void IncrementPassengersAmount(int deltaAmount)
        {
            if (deltaAmount < 0)
            {
                Debug.LogError("PlayerAccount: IncrementPassengersAmount called with negative deltaAmount.");
            }
            
            _playerAccountData.savedPassengers += deltaAmount;
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

        public string ToJson()
        {
            return JsonUtility.ToJson(GetData());
        }
    }
}