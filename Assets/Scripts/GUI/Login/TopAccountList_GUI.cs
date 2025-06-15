using System;
using System.Collections.Generic;
using System.Linq;
using AccountManagement;
using TMPro;
using UnityEngine;

public class TopAccountList_GUI : MonoBehaviour
{
    [SerializeField] private RectTransform listItemPrefab;
    [SerializeField] private TextMeshProUGUI  noAccountTextPrefab;
    [SerializeField] private int amountOfTopAccountsToDisplay = 5;
    [SerializeField] private float gapBetweenListItems = 5;
    
    private List<RectTransform> _listItems;
    private TextMeshProUGUI _noAccountTextInstance;
    
    private void Start()
    {
        InitializeItemList();
        
        AccountManager.onCreateAccount += accountData => UpdateList();
        AccountManager.onDeleteAccount += accountData => UpdateList();
        
        UpdateList();
    }

    private void UpdateList()
    {
        var accounts = AccountManager
            .GetSavedAccounts()
            .OrderByDescending(data => data.bountyPoints)
            .Take(amountOfTopAccountsToDisplay)
            .ToList();
        
        for (var i = 0; i < amountOfTopAccountsToDisplay; i++)
        {
            var textComponent = _listItems[i].gameObject.GetComponent<TextMeshProUGUI>();

            textComponent.text = "";

            if (i >= accounts.Count) continue;
            
            textComponent.text = $"{i + 1}. {accounts[i].accountName}";
        }
        
        if (accounts.Count == 0)
        {
            Instantiate(noAccountTextPrefab, transform);
        }else if (_noAccountTextInstance != null)
        {
            Destroy(_noAccountTextInstance);
        }
    }

    private void InitializeItemList()
    {
        _listItems = new List<RectTransform>();

        for (var i = 0; i < amountOfTopAccountsToDisplay; i++)
        {
            var item = Instantiate(listItemPrefab, transform);
            item.anchoredPosition = new Vector2(0, -(i * item.rect.height + i * gapBetweenListItems));
            _listItems.Add(item);
        }
    }
}