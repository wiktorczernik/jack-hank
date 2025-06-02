using System;
using UnityEngine;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] private PlayerVehicle player;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private Transform botDestination;
    [SerializeField] private bool useEnter = true;
    [SerializeField] private ScreenFadeType fadeOutType = ScreenFadeType.Default;
    [SerializeField] private float fadeOutDuration = 1.2f;
    [Range(0, 200)][SerializeField] private float initialSpeedInKmH = 35f;
    private bool _isDisabled;

    private Rigidbody _rigidbody;

    public bool UseEnter => useEnter;

    private void Start()
    {
        if (!useEnter || _isDisabled) return;

        TeleportPlayerAtEnter();
    }

    public event Action OnPlayerEndEntering;

    public void Disable()
    {
        _isDisabled = true;
    }

    public void TeleportPlayerAtEnter()
    {
        player.Teleport(startingPosition.position, startingPosition.rotation);
        ScreenFade.onBeforeOut += AfterFadeOut;
        ScreenFade.Out(fadeOutDuration, fadeOutType);
    }

    private void AfterFadeOut()
    {
        ScreenFade.onBeforeOut -= AfterFadeOut;
        player.physics.bodyRigidbody.linearVelocity = player.transform.forward * (initialSpeedInKmH / 2); 
        //I do not know why linear velocity takes km/h and multiply it on 2, but it works
    }
}