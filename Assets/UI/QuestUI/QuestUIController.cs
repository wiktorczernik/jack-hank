using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class QuestUIController : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;

    [SerializeField] private string nameOfGameScene;
    
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _playButton = _document.rootVisualElement.Q<Button>("PlayButton");
        _playButton.clicked += PlayButtonClicked;
    }

    private void PlayButtonClicked()
    {
        SceneManager.LoadScene(nameOfGameScene);
    }
    

}
