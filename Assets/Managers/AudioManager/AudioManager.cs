using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] private string menuSceneName;

    [SerializeField] private MenuController mainMenuController;
    [SerializeField] private PauseMenuController pauseMenuController;

    private IMenuController _menuController;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == menuSceneName)
        {
            _menuController = mainMenuController;
        }
        else
        {
            _menuController = pauseMenuController;
        }

        SetMusiCVolume(PlayerPrefs.GetFloat("SavedMusicVolume", 100));
        SetSfxVolume(PlayerPrefs.GetFloat("SavedSFXVolume", 100));
    }

    //Music
    public void SetMusiCVolume(float value)
    {
        if (value < 1) {
            value = 0.001f;
        }
        RefreshMusicSlider(value);
        PlayerPrefs.SetFloat("SavedMusicVolume", value);
        masterMixer.SetFloat("Music", Mathf.Log10(value / 100) * 20f);
    }
    
    public void RefreshMusicSlider(float value)
    {
        _menuController.SetMusicSliderValue(value);
    }
    
    //SFX
    public void SetSfxVolume(float value)
    {
        if (value < 1) {
            value = 0.001f;
        }
        RefreshSfxSlider(value);
        PlayerPrefs.SetFloat("SavedSFXVolume", value);
        masterMixer.SetFloat("SFX", Mathf.Log10(value / 100) * 20f);
    }
    
    public void RefreshSfxSlider(float value)
    {
        _menuController.SetSfxSliderValue(value);
    }
}
