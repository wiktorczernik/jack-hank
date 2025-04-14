using System;
using UnityEngine;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] private PlayerVehicle player;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private Transform botDestination;

    public event Action OnPlayerEndEntering;

    private Rigidbody _rigidbody;
    private void Start()
    {
        player.GetComponent<Rigidbody>().position = startingPosition.position;
        var playersBot = player.GetComponent<BotVehicle>();
        
        playersBot.isFollowing = true;
        playersBot.followMode = BotVehicle.FollowMode.Single; 
        playersBot.destinationPoint = botDestination.position;
        playersBot.followMaxSpeed = 100;

        playersBot.OnArrived += () => OnPlayerEndEntering?.Invoke();
    }
}