using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AccountManagement;
using JetBrains.Annotations;
using LevelManagement;
using UnityEngine;

// Jest to statyczna klasa zarządzająca kontem gracza. Umożliwia wejście do specjalnego konta debugowego, które można
// konfigurować za pomocą inspektora w scenie Essentials.
public class AccountManager : MonoBehaviour
{
    [SerializeField] private PlayerAccountData debugAccountSettings;
    private static string _saveFolderPath => Application.persistentDataPath + "/saves";
    private static AccountManager _instance;

    private static PlayerAccount _currentAccount;
    
    public static PlayerAccount currentAccount
    {
        get
        {
            if (_currentAccount != null) return _currentAccount;
            LogInDebugAccount();

            return _currentAccount;
        }
    }

    public static bool useDebugAccount { get; set; }
    
    public static event Action<PlayerAccountData> onLoggedIn;
    public static event Action<PlayerAccountData> onLoggedOut;

    public static event Action<PlayerAccountData> onDeleteAccount;

    public static event Action<PlayerAccountData> onCreateAccount;

    private void Awake()
    {
        _instance = this;
        Debug.Log($"AccountManager: Account saves on path {_saveFolderPath}");
    }

    public static void LogInDebugAccountOnAwake()
    {
        LogInDebugAccount();
    }

    public static List<string> GetSavedAccountsNames()
    {
        EnsureSaveDirectoryExists();

        return new DirectoryInfo(_saveFolderPath)
            .GetFiles("*.json")
            .ToList()
            .ConvertAll(save => save.Name.Replace(".json", ""));
    }

    public static List<PlayerAccountData> GetSavedAccounts()
    {
        EnsureSaveDirectoryExists();
        return new DirectoryInfo(_saveFolderPath).GetFiles("*.json")
            .ToList()
            .ConvertAll(save =>
            {
                var account =  JsonUtility.FromJson<PlayerAccountData>(File.ReadAllText(save.FullName));
                account.accountName = save.Name.Replace(".json", "");
                return account;
            });
    }

    public static bool ExistsSavedAccount(string accountName)
    {
        return File.Exists(GetAccountSavePath(accountName));
    }

    public static LogInStatus LogInAccount(string accountName)
    {
        EnsureSaveDirectoryExists();

        var savePath = GetAccountSavePath(accountName);
        
        if (!File.Exists(savePath))
        {
            Debug.LogError($"AccountManager: Account with name '{accountName}' not found.");
            return LogInStatus.AccountNotFound;
        }

        PlayerAccountData accountData;
        try
        {
            accountData = JsonUtility.FromJson<PlayerAccountData>(File.ReadAllText(savePath));
        }
        catch (Exception e)
        {
            Debug.LogError("AccountManager: something went wrong with serialization. Maybe save is corrupt or too much old.");
            Debug.LogError(e);
            return LogInStatus.AccountSaveCorrupted;
        }
         
        accountData.accountName = accountName;
        
        _currentAccount = new PlayerAccount(accountData);
        
        onLoggedIn?.Invoke(accountData.Clone() as PlayerAccountData);
        
        return LogInStatus.Success;
    }

    public static void LogInDebugAccount()
    {
        if (_instance == null)
        {
            Debug.LogError(
                "AccountManager: No instance of AccountManager. Probably you forgot to load 'Essentials' scene");
            return;
        }
        if (_currentAccount != null) return;
        _currentAccount = new PlayerAccount(_instance.debugAccountSettings);
        useDebugAccount = true;
        onLoggedIn?.Invoke(_currentAccount.GetData().Clone() as PlayerAccountData);
    }

    public static void LogOutCurrentAccount()
    {
        _currentAccount = null;
        onLoggedOut?.Invoke(GetUpdatedAccountData());
    }

    public static bool IsLoggedIn()
    {
        return _currentAccount != null;
    }

    public static void SaveCurrentAccount()
    {
        if (useDebugAccount) return;
        
        EnsureSaveDirectoryExists();

        File.WriteAllText(
            GetAccountSavePath(_currentAccount.GetAccountName()),
            JsonUtility.ToJson(GetUpdatedAccountData()));
    }

    public static LogInStatus LogInNewAccount(string accountName)
    {
        if (ExistsSavedAccount(accountName))
        {
            Debug.LogError($"AccountManager: account with name '{accountName}' already exists.");
            return LogInStatus.AccountAlreadyExist;
        }
        
        EnsureSaveDirectoryExists();
        _currentAccount = new PlayerAccount(accountName);
        File.WriteAllText(GetAccountSavePath(accountName), _currentAccount.ToJson());
        onCreateAccount?.Invoke(_currentAccount.GetData());
        
        return LogInAccount(accountName);
    }

    public static void RemoveAccount(string accountName)
    {
        EnsureSaveDirectoryExists();

        var accountData = GetLocalAccountData(accountName);
        if (accountData == null) return;
        
        File.Delete(GetAccountSavePath(accountName));
        onDeleteAccount?.Invoke(accountData);
    }

    private static string GetAccountSavePath(string accountName)
    {
        return $"{_saveFolderPath}/{accountName}.json";
    }

    private static void EnsureSaveDirectoryExists()
    {
        if (Directory.Exists(_saveFolderPath)) return;

        Directory.CreateDirectory(_saveFolderPath);
    }

    private static PlayerAccountData GetUpdatedAccountData()
    {
        var dataToSave = _currentAccount.GetData();

        dataToSave.openedLevels = LevelManager.GetLevelsStatistics();

        dataToSave.bountyPoints = dataToSave.openedLevels.Sum(level => level.bonuses.Sum(pair => pair.Value));

        return dataToSave;
    }

    [CanBeNull]
    private static PlayerAccountData GetLocalAccountData(string accountName)
    {
        if (!ExistsSavedAccount(accountName)) return null;

        var data = JsonUtility.FromJson<PlayerAccountData>(File.ReadAllText(GetAccountSavePath(accountName)));
        data.accountName = accountName;
        return data;
    }

    public enum LogInStatus
    {
        AccountNotFound,
        AccountSaveCorrupted,
        AccountAlreadyExist,
        Success
    }
}