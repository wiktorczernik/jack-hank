using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindow_GUI : MonoBehaviour
{
    private TextMeshProUGUI _textMassage;
    private Button _okBtn;
    
    public void Show(string text)
    {
        gameObject.SetActive(true);
        _textMassage.text = text;
    }
    
    private void Awake()
    {
        _textMassage = GetComponentInChildren<TextMeshProUGUI>();
        _okBtn = GetComponentInChildren<Button>();
        gameObject.SetActive(false);
        _okBtn.onClick.AddListener(OkButtonClick);
    }

    private void OkButtonClick()
    {
        gameObject.SetActive(false);
    }
}