using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAccountList : MonoBehaviour
{
    [SerializeField] private SelectAccount buttonLoadAccount;
    [SerializeField] private TextMeshProUGUI noAccountsText;
    private bool _isAccountSelected;
    
    private void Start()
    {
        var names = AccountLoader.GetSavedAccountsNames();

        if (names.Count == 0)
        {
            Instantiate(noAccountsText, gameObject.transform);
            return;
        }

        for (var i = 0; i < names.Count; i++)
        {
            Instantiate(buttonLoadAccount, transform).Initialize(i, names[i], OnAccountSelected, OnAccountNotExist);
        }
    }

    private void OnAccountSelected(string accountName)
    {
        if (_isAccountSelected) return;

        AccountLoader.LogInAccount(accountName);
        _isAccountSelected = true;
        SceneManager.LoadScene("Levels");
    }

    private void OnAccountNotExist()
    {
        
    }
}
