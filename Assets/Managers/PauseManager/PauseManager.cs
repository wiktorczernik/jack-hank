using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    [SerializeField] private PauseMenuController pauseMenuController;

    public void PauseButtonPressed()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    
    private void Pause()
    {
        isPaused = true;
        pauseMenuController._display.style.display = DisplayStyle.Flex;
    }

    private void Resume()
    {
        isPaused = false;
        pauseMenuController._display.style.display = DisplayStyle.None;
    }
}
