using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AccountManagement;
using LevelManagement;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    [SerializeField] private PlayerAccountData debugAccountSettings;
    private static string _saveFolderPath => Application.persistentDataPath + "/saves";
    private static AccountManager _instance;
    
    public static PlayerAccount loggedInPlayerAccount { get; private set; }
    public static bool useDebugAccount { get; set; }
    
    public static event Action<PlayerAccountData> OnLoggedIn;
    public static event Action<PlayerAccountData> OnLoggedOut;

    private void Awake()
    {
        _instance = this;
    }

    public static List<string> GetSavedAccountsNames()
    {
        ProcessSaveDirectory();

        return new DirectoryInfo(_saveFolderPath)
            .GetFiles("*.json")
            .ToList()
            .ConvertAll(save => save.Name.Replace(".json", ""));
    }

    public static bool ExistsSavedAccount(string accountName)
    {
        return File.Exists(GetAccountSavePath(accountName));
    }

    public static void LogInAccount(string accountName)
    {
        if (IsLoggedIn())
        {
            Debug.LogError("AccountManager: There's already logged in account.");
            return;
        }
        ProcessSaveDirectory();

        var savePath = GetAccountSavePath(accountName);

        if (!File.Exists(savePath))
        {
            Debug.LogError($"AccountManager: Account with name '{accountName}' not found.");
            return;
        }

        var accountData = JsonUtility.FromJson<PlayerAccountData>(File.ReadAllText(savePath));
        accountData.accountName = accountName;

        loggedInPlayerAccount = new PlayerAccount(accountData);
        
        OnLoggedIn?.Invoke(accountData.Clone() as PlayerAccountData);
    }

    public static void LogInDebugAccount()
    {
        if (_instance == null)
        {
            Debug.LogError(
                "AccountManager: No instance of AccountManager. Probably you forgot to load 'Essentials' scene");
            return;
        }
        if (loggedInPlayerAccount != null) return;
        loggedInPlayerAccount = new PlayerAccount(_instance.debugAccountSettings);
        useDebugAccount = true;
        OnLoggedIn?.Invoke(loggedInPlayerAccount.GetData().Clone() as PlayerAccountData);
    }

    public static void LogOutCurrentAccount()
    {
        loggedInPlayerAccount = null;
        OnLoggedOut?.Invoke(GetUpdatedAccountData());
    }

    public static bool IsLoggedIn()
    {
        return loggedInPlayerAccount != null;
    }

    public static void SaveCurrentAccount()
    {
        if (useDebugAccount) return;
        
        ProcessSaveDirectory();

        File.WriteAllText(
            GetAccountSavePath(loggedInPlayerAccount.GetAccountName()),
            JsonUtility.ToJson(GetUpdatedAccountData()));
    }

    public static void LogInNewAccount(string accountName)
    {
        if (ExistsSavedAccount(accountName))
        {
            Debug.LogError($"AccountManager: account with name '{accountName}' already exists.");
            return;
        }
        if (IsLoggedIn())
        {
            Debug.LogError("AccountManager: There's already logged in account.");
            return;
        }

        ProcessSaveDirectory();
        loggedInPlayerAccount = new PlayerAccount(accountName);
        File.WriteAllText(GetAccountSavePath(accountName), JsonUtility.ToJson(loggedInPlayerAccount.GetData()));

        LogInAccount(accountName);
    }

    private static string GetAccountSavePath(string accountName)
    {
        return $"{_saveFolderPath}/{accountName}.json";
    }

    private static void ProcessSaveDirectory()
    {
        if (Directory.Exists(_saveFolderPath)) return;

        Directory.CreateDirectory(_saveFolderPath);
    }

    private static PlayerAccountData GetUpdatedAccountData()
    {
        var dataToSave = loggedInPlayerAccount.GetData();

        dataToSave.openedLevels = LevelManager.GetLevelsStatistics();

        dataToSave.bouncy = dataToSave.openedLevels.Sum(level => level.bonuses.Sum(pair => pair.Value));

        return dataToSave;
    }
}