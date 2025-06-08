using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountButton_GUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private InputTip_GUI inputTip;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (nicknameInput.text.Trim().Length == 0)
        {
            inputTip.ShowText("Nickname is required");
            return;
        }
        
        var status = AccountManager.LogInNewAccount(nicknameInput.text);

        if (status == AccountManager.LogInStatus.AccountAlreadyExist)
        {
            inputTip.ShowText("Account already exist");
        }
        else
        {
            GameSceneManager.LoadFirstLevel();
        }
    }
}