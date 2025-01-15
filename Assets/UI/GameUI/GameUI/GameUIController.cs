using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    private UIDocument _document;
    private Button _goUpgradeButton;

    [SerializeField] private string nameOfUpgradeScene;
    
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _goUpgradeButton = _document.rootVisualElement.Q<Button>("GoUpgradeButton");
        _goUpgradeButton.clicked += GoUpgradeButtonClicked;
    }

    private void GoUpgradeButtonClicked()
    {
        SceneManager.LoadScene(nameOfUpgradeScene);
    }
}
