using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMenuController
{
    void SettingsButtonClicked();
    void BackButtonClicked();
    
    void MusicSliderChanged(ChangeEvent<float> evt);
    void SetMusicSliderValue(float value);

    void SfxSliderChanged(ChangeEvent<float> evt);
    void SetSfxSliderValue(float value);

    void ExitButtonClicked();
}
