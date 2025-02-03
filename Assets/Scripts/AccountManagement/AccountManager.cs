using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using AccountManagement;
using UnityEngine;

public static class AccountManager
{
    public static PlayerAccount LoggedInPlayerAccount { get; private set; }

    private static readonly string SaveFolderPath = Application.persistentDataPath + "/saves";

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

        LoggedInPlayerAccount = new PlayerAccount(accountData);
    }

    public static void LogOutCurrentAccount()
    {
        LoggedInPlayerAccount = null;
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
