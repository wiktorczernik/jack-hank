using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UpgradeUIController : MonoBehaviour
{
    private UIDocument _document;
    private Button _goQuestButton;

    [SerializeField] private string nameOfQuestScene;
    
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _goQuestButton = _document.rootVisualElement.Q<Button>("NextDayButton");
        _goQuestButton.clicked += GoQuestButtonClicked;
    }

    private void GoQuestButtonClicked()
    {
        SceneManager.LoadScene(nameOfQuestScene);
    }
}
