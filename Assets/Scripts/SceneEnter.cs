using System;
using UnityEngine;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] private PlayerVehicle player;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private Transform botDestination;
    [SerializeField] private FadeTransition_GUI fadeTransition;
    [SerializeField] private bool useEnter = true;

    private Rigidbody _rigidbody;

    private void Start()
    {
        if (!useEnter) return;

        player.GetComponent<Rigidbody>().position = startingPosition.position;

        fadeTransition.PrepareFadeOut();
        fadeTransition.StartFadeOut();
        fadeTransition.OnFadeOutEnded += AfterFadeOut;
    }

    public event Action OnPlayerEndEntering;

    public void Disable()
    {
        useEnter = false;
    }

    private void AfterFadeOut()
    {
        var playersBot = player.GetComponent<BotVehicle>();
        playersBot.isFollowing = true;
        playersBot.followMode = BotVehicle.FollowMode.Single;
        playersBot.destinationPoint = botDestination.position;
        playersBot.followMaxSpeed = 100;

        playersBot.onArrived += () => OnPlayerEndEntering?.Invoke();
        fadeTransition.OnFadeOutEnded -= AfterFadeOut;
    }
}