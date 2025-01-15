using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour, IMenuController
{

    [SerializeField] private VisualTreeAsset settingsMenuTemplate;
    [SerializeField] private VisualElement _settingsMenu;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PauseManager pauseManager;

    [SerializeField] private string nameOfMainMenuScene;
    
    private VisualElement _buttonsWrapper;
    
    private UIDocument _document;

    public VisualElement _display;

    private Button _resumeButton;
    private Button _settingsButton;
    private Button _returnToMainMenuButton;
    private Button _quitButton;

    private Slider _musicSlider;
    private Slider _sfxSlider;
    
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        
        _buttonsWrapper = _document.rootVisualElement.Q<VisualElement>("Menu");
        
        _display = _document.rootVisualElement.Q<VisualElement>("Display");

        _resumeButton = _document.rootVisualElement.Q<Button>("ResumeButton");
        _settingsButton = _document.rootVisualElement.Q<Button>("OptionsButton"); 
        _returnToMainMenuButton = _document.rootVisualElement.Q<Button>("ReturnToMainMenuButton"); 
        _quitButton = _document.rootVisualElement.Q<Button>("QuitButton");

        _resumeButton.clicked += ResumeButtonClicked;
        _settingsButton.clicked += SettingsButtonClicked;
        _returnToMainMenuButton.clicked += ReturnToMainMenuButtonClicked;
        _quitButton.clicked += ExitButtonClicked;

        _settingsMenu = settingsMenuTemplate.CloneTree();

        var backButton = _settingsMenu.Q<Button>("BackButton");
        backButton.clicked += BackButtonClicked;

        _musicSlider = _settingsMenu.Q<Slider>("MusicSlider");
        _musicSlider.RegisterValueChangedCallback(MusicSliderChanged);

        _sfxSlider = _settingsMenu.Q<Slider>("SFXSlider");
        _sfxSlider.RegisterValueChangedCallback(SfxSliderChanged);

    }

    //Resume Button
    private void ResumeButtonClicked()
    {
        pauseManager.PauseButtonPressed();
    }
    
    //Return To Main Menu Button
    private void ReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene(nameOfMainMenuScene);
    }

    //Exit Button
    public void ExitButtonClicked()
    {
        Application.Quit();
    }

    //Settings
    public void SettingsButtonClicked()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_settingsMenu);
    }

    public void BackButtonClicked()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_resumeButton);
        _buttonsWrapper.Add(_settingsButton);
        _buttonsWrapper.Add(_returnToMainMenuButton);
        _buttonsWrapper.Add(_quitButton);
    }
    
    //Music Slider
    public void MusicSliderChanged(ChangeEvent<float> evt)
    {
        audioManager.SetMusiCVolume(_musicSlider.value);
    }

    public void SetMusicSliderValue(float value)
    {
        _musicSlider.value = value;
    }
    
    //SFX Slider
    public void SfxSliderChanged(ChangeEvent<float> evt)
    {
        audioManager.SetSfxVolume(_sfxSlider.value);
    }

    public void SetSfxSliderValue(float value)
    {
        _sfxSlider.value = value;
    }
    
}
