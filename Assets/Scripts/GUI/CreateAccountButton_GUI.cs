using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateAccountButton_GUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        AccountManager.LogInNewAccount(nicknameText.text);
        SceneManager.LoadScene("SelectLevels");
    }
}
