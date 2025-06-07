using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoadAccountList_GUI : MonoBehaviour
{
    [FormerlySerializedAs("buttonLoadAccount")] [SerializeField]
    private SelectAccountButton_GUI buttonLoadAccountButtonGUI;

    [SerializeField] private TextMeshProUGUI noAccountsText;
    private bool _isAccountSelected;

    private void Start()
    {
        var names = AccountManager.GetSavedAccountsNames();

        if (names.Count == 0)
        {
            Instantiate(noAccountsText, gameObject.transform);
            return;
        }

        for (var i = 0; i < names.Count; i++)
            Instantiate(buttonLoadAccountButtonGUI, transform)
                .Initialize(i, names[i], OnAccountSelected, OnAccountNotExist);
    }

    private void OnAccountSelected(string accountName)
    {
        if (_isAccountSelected) return;

        void LoadGame()
        {
            ScreenFade.onAfterIn -= LoadGame;
            AccountManager.LogInAccount(accountName);
            _isAccountSelected = true;
            GameSceneManager.LoadFirstLevel();
        }

        ScreenFade.onAfterIn += LoadGame;

        ScreenFade.In(2, ScreenFadeType.Circle);
    }

    private void OnAccountNotExist()
    {
    }
}