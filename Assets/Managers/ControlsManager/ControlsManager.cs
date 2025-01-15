using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] private PauseManager _pauseManager;
    [SerializeField] private BusSystems _busSystems;

    public Controls Controls;

    private InputAction _escapePressedAction;
    private InputAction _EpressedAction;

    private void Awake()
    {
        Controls = new Controls();
    }

    private void OnEnable()
    {
        _escapePressedAction = Controls.UI.Cancel;
        _escapePressedAction.Enable();
        _escapePressedAction.performed += EscapePressed;

        _EpressedAction = Controls.Player.DoorControl;
        _EpressedAction.Enable();
        _EpressedAction.performed += _Epressed;
    }
    
    private void OnDisable()
    {
        _escapePressedAction.Disable();
        _EpressedAction.Disable();
    }
    
    private void EscapePressed(InputAction.CallbackContext context)
    {
        _pauseManager.PauseButtonPressed();
    }

    private void _Epressed(InputAction.CallbackContext obj)
    {
        _busSystems.DoorsAction();
    }
}
