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
    private static string SaveFolderPath => Application.persistentDataPath + "/saves";
    public static PlayerAccount LoggedInPlayerAccount { get; private set; }

    public static bool UseDebugAccount { get; set; }
    private static AccountManager _instance;
    
    public static event Action<PlayerAccountData> OnLoggedIn;
    public static event Action<PlayerAccountData> OnLoggedOut;

    private void Awake()
    {
        _instance = this;
    }

    public static List<string> GetSavedAccountsNames()
    {
        ProcessSaveDirectory();

        return new DirectoryInfo(SaveFolderPath)
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
        accountData.AccountName = accountName;

        LoggedInPlayerAccount = new PlayerAccount(accountData);
        
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
        if (LoggedInPlayerAccount != null) return;
        LoggedInPlayerAccount = new PlayerAccount(_instance.debugAccountSettings);
        UseDebugAccount = true;
        OnLoggedIn?.Invoke(LoggedInPlayerAccount.GetData().Clone() as PlayerAccountData);
    }

    public static void LogOutCurrentAccount()
    {
        LoggedInPlayerAccount = null;
        OnLoggedOut?.Invoke(GetUpdatedAccountData());
    }

    public static bool IsLoggedIn()
    {
        return LoggedInPlayerAccount != null;
    }

    public static void SaveCurrentAccount()
    {
        if (UseDebugAccount) return;
        
        ProcessSaveDirectory();

        File.WriteAllText(
            GetAccountSavePath(LoggedInPlayerAccount.GetAccountName()),
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
        LoggedInPlayerAccount = new PlayerAccount(accountName);
        File.WriteAllText(GetAccountSavePath(accountName), JsonUtility.ToJson(LoggedInPlayerAccount.GetData()));

        LogInAccount(accountName);
    }

    private static string GetAccountSavePath(string accountName)
    {
        return $"{SaveFolderPath}/{accountName}.json";
    }

    private static void ProcessSaveDirectory()
    {
        if (Directory.Exists(SaveFolderPath)) return;

        Directory.CreateDirectory(SaveFolderPath);
    }

    private static PlayerAccountData GetUpdatedAccountData()
    {
        var dataToSave = LoggedInPlayerAccount.GetData();

        dataToSave.openedLevels = LevelManager.GetLevelsStatistics();

        dataToSave.bouncy = dataToSave.openedLevels.Sum(level => level.Bonuses.Sum(pair => pair.Value));

        return dataToSave;
    }
}