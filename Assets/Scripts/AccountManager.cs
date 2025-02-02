using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountData CurrentAccountData { get; private set; }

    public static List<string> GetSavedAccountsNames()
    {
        ProcessSaveDirectory();

        return new DirectoryInfo(GetDestination())
            .GetFiles("*.json")
            .ToList()
            .ConvertAll(save => save.Name);
    }

    public static bool ExistsSavedAccount(string name)
    {
        return File.Exists(GetAccountSavePath(name));
    }

    public static void LogInAccount(string name)
    {
        ProcessSaveDirectory();

        var savePath = GetAccountSavePath(name);

        CurrentAccountData = File.Exists(savePath)
            ? JsonUtility.FromJson<AccountData>(new FileInfo(savePath).ToString())
            : new AccountData();
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

        var newAccount = new AccountData();

        CurrentAccountData = newAccount;
        
        File.WriteAllText(GetAccountSavePath(accountName), JsonUtility.ToJson(newAccount));
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
