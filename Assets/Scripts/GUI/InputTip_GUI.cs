using TMPro;
using UnityEngine;

public class InputTip_GUI : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    public void ShowText(string text)
    {
        gameObject.SetActive(true);
        _text.text = text;
    }
    
    private void Awake()
    {
        gameObject.SetActive(false);
        _text = GetComponent<TextMeshProUGUI>();
    }
}