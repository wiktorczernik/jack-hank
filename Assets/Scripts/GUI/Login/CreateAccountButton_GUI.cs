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
            inputTip.ShowText("Account with that nickname already exists");
        }
        else
        {
            GetComponent<Button>().interactable = false;

            void LoadFirstLevel()
            {
                ScreenFade.onAfterIn -= LoadFirstLevel;
                GameSceneManager.LoadFirstLevel();
            }
            ScreenFade.onAfterIn += LoadFirstLevel;

            ScreenFade.In(1f, ScreenFadeType.Circle);
        }
    }
}