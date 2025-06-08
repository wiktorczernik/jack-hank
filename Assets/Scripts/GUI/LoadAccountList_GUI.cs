using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AccountList_GUI : MonoBehaviour
{
    [SerializeField] private AccountListItem_GUI listItemPrefab;
    [SerializeField] private TextMeshProUGUI  noAccountTextPrefab;
    [SerializeField] private ModalWindow_GUI modalWindow;

    private List<AccountListItem_GUI> _items;
    private bool _wasAnyItemSelected;

    private void Start()
    {
        _items = new List<AccountListItem_GUI>();
        var names = AccountManager.GetSavedAccountsNames();

        if (names.Count == 0)
        {
            Instantiate(noAccountTextPrefab, gameObject.transform);
            return;
        }

        for (var i = 0; i < names.Count; i++)
        {
            var listItem = Instantiate(listItemPrefab, transform); 
            listItem.Initialize(names[i], OnItemClick, OnItemDelete);
            _items.Add(listItem);
        }
        
        UpdateList();
    }

    private void UpdateList()
    {
        var offset = 10;
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            item.SetPosition(new Vector2(0, -(item.rectTransform.rect.height + offset) * i));
        }
    }

    private void DeleteItem(AccountListItem_GUI listItem)
    {
        _items.Remove(listItem);
        Destroy(listItem.gameObject);
        AccountManager.RemoveAccount(listItem.accountName);
        UpdateList();
    }

    private void OnItemClick(AccountListItem_GUI listItem)
    {
        if (_wasAnyItemSelected) return;

        var status = AccountManager.LogInAccount(listItem.accountName);

        if (status == AccountManager.LogInStatus.AccountNotFound)
        {
            modalWindow.Show("Something went wrong. Account not fount.");
            DeleteItem(listItem);
        }else if (status == AccountManager.LogInStatus.AccountSaveCorrupted)
        {
            modalWindow.Show("Something went wrong. Account save is corrupted.");
            DeleteItem(listItem);
        }else if (status == AccountManager.LogInStatus.Success)
        {
            _wasAnyItemSelected = true;
            void LoadGame()
            {
                ScreenFade.onAfterIn -= LoadGame;
                AccountManager.LogInAccount(accountName);
                GameSceneManager.LoadFirstLevel();
            }

            ScreenFade.onAfterIn += LoadGame;
            ScreenFade.In(2, ScreenFadeType.Circle);
        }
    }

    private void OnItemDelete(AccountListItem_GUI listItem)
    {
        DeleteItem(listItem);
    }
}