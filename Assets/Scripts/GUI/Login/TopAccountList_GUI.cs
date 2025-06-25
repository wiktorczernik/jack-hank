using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TopAccountList_GUI : MonoBehaviour
{
    [SerializeField] private LeaderListItem listItemPrefab;
    [SerializeField] private TextMeshProUGUI  noAccountTextPrefab;
    [SerializeField] private int amountOfTopAccountsToDisplay = 5;
    [SerializeField] private float gapBetweenListItems = 5;
    
    private List<LeaderListItem> _listItems;
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
            .ThenByDescending(data => data.savedPassengers)
            .ThenBy(data => data.playTimeTimestamp)
            .ToList();
        
        for (var i = 0; i < accounts.Count; i++)
        {
            var acc = accounts[i];
            var item = Instantiate(listItemPrefab, transform.GetChild(0));
            _listItems.Add(item);

            
            item.SetName($"#{i + 1} - {acc.accountName}");
            item.SetBounty(acc.bountyPoints);
            item.SetPassengers(acc.savedPassengers);
            item.SetPlayTime(acc.playTimeTimestamp);
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
        if (_listItems != null)
        {
            foreach (var item in _listItems)
            {
                Destroy(item);
            }
        }

        _listItems = new List<LeaderListItem>();

        for (var i = 0; i < amountOfTopAccountsToDisplay; i++)
        {
        }
    }
}