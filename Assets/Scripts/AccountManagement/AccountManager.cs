using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AccountManagement;
using LevelManagement;
using UnityEngine;

public static class AccountManager
{
    private static readonly string SaveFolderPath = Application.persistentDataPath + "/saves";
    public static PlayerAccount LoggedInPlayerAccount { get; private set; }

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
        ProcessSaveDirectory();

        var savePath = GetAccountSavePath(accountName);

        if (!File.Exists(savePath)) throw new Exception("Account not found");

        var accountData = new PlayerAccountData
        {
            AccountName = accountName
        };

        JsonUtility.FromJsonOverwrite(File.ReadAllText(savePath), accountData);

        LevelManager.InitializeAndValidateLevelsTree(accountData.openedLevels.ToList());

        LoggedInPlayerAccount = new PlayerAccount(accountData);
    }

    public static void LogOutCurrentAccount()
    {
        LoggedInPlayerAccount = null;
    }

    public static bool IsLoggedIn()
    {
        return LoggedInPlayerAccount != null;
    }

    public static void SaveCurrentAccount()
    {
        ProcessSaveDirectory();

        File.WriteAllText(
            GetAccountSavePath(LoggedInPlayerAccount.GetAccountName()),
            JsonUtility.ToJson(LoggedInPlayerAccount.GetData()));
    }

    public static void LogInNewAccount(string accountName)
    {
        if (ExistsSavedAccount(accountName)) throw new Exception("Account already exists");
        if (LoggedInPlayerAccount != null) throw new Exception("There is logged in another account");

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
}