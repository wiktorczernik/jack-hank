using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class AccountManager
{
    public static AccountData CurrentAccountData { get; private set; }

    public static List<string> GetSavedAccountsNames()
    {
        ProcessSaveDirectory();

        return new DirectoryInfo(GetDestination())
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

        var accountData = new AccountData
        {
            AccountName = accountName
        };
        
        JsonUtility.FromJsonOverwrite(File.ReadAllText(savePath), accountData);

        CurrentAccountData = accountData;
    }

    public static void LogOutCurrentAccount()
    {
        CurrentAccountData = null;
    }

    public static void SaveCurrentAccount()
    {
        ProcessSaveDirectory();
        
        File.WriteAllText(GetAccountSavePath(CurrentAccountData.AccountName), JsonUtility.ToJson(CurrentAccountData));
    }

    public static void LogInNewAccount(string accountName)
    {
        if (ExistsSavedAccount(accountName)) throw new Exception("Account already exists");
        if (CurrentAccountData != null) throw new Exception("There is logged in another account");
        
        ProcessSaveDirectory();

        var newAccount = new AccountData
        {
            AccountName = accountName
        };

        CurrentAccountData = newAccount;
        
        File.WriteAllText(GetAccountSavePath(accountName), JsonUtility.ToJson(newAccount));
    }

    private static string GetAccountSavePath(string accountName)
    {
        return $"{GetDestination()}/{accountName}.json";
    }

    private static void ProcessSaveDirectory()
    {
        if (Directory.Exists(GetDestination())) return;
        
        Directory.CreateDirectory(GetDestination());
    }

    private static string GetDestination()
    {
        return Application.persistentDataPath + "/saves";
    }
}
