using System;
using UnityEngine;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] private PlayerVehicle player;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private Transform botDestination;
    [SerializeField] private FadeTransition_GUI fadeTransition;
    [SerializeField] private bool useEnter = true;

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

        fadeTransition.PrepareFadeOut();
        fadeTransition.StartFadeOut();
        fadeTransition.OnFadeOutEnded += AfterFadeOut;
    }

    private void AfterFadeOut()
    {
        var playersBot = player.GetComponent<BotVehicle>();
        playersBot.isFollowing = true;
        playersBot.followMode = BotVehicle.FollowMode.Single;
        playersBot.destinationPoint = botDestination.position;
        playersBot.followMaxSpeed = 100;
        playersBot.destinationArriveMaxSpeed = 200;

        playersBot.onArrived += () => OnPlayerEndEntering?.Invoke();
        fadeTransition.OnFadeOutEnded -= AfterFadeOut;
    }
}