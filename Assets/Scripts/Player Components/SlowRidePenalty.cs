using System;
using JackHank.Cinematics;
using UnityEngine;


// Checks if the player bus moves too slow. If so, the player receives a warning, then a penalty.
public class SlowRidePenalty : MonoBehaviour
{
    [Tooltip("Current script state")]
    public static PenaltyState state;

    [Tooltip("The threshold velocity when the script gets triggered. In km/h")] public float maxPenaltyVelocity;
    public static float eventTimer;
    [Tooltip("How long will the excusing period last")]
    public float excusingDuration = 3;
    [Tooltip("How long will the warning period last")]
    public float warningDuration = 5;

    public static event Action onWarningBegin;
    public static event Action onWarningTick;
    public static event Action onWarningEnd;
    public static event Action onPenaltyReceived;

    PlayerVehicle player;


    private void Awake()
    {
        player = GetComponent<PlayerVehicle>();
    }
    private void OnEnable()
    {
        state = PenaltyState.Waiting;
        eventTimer = 0;
    }
    private void OnDisable()
    {
        state = PenaltyState.Waiting;
        eventTimer = 0;
    }

    private void FixedUpdate()
    {
        if (state == PenaltyState.Penalty) return;
        if (player.physics.speedKmh >= maxPenaltyVelocity || CinematicPlayer.isPlaying)
        {
            state = PenaltyState.Waiting;
            eventTimer = 0f;
            return;
        }

        eventTimer += Time.fixedDeltaTime;
        if (state == PenaltyState.Waiting) state = PenaltyState.Excusing;
        if (state == PenaltyState.Excusing && eventTimer >= excusingDuration)
        {
            state = PenaltyState.Warning;
            eventTimer = 0f;
            onWarningBegin?.Invoke();
            return;
        }
        if (state == PenaltyState.Warning)
        {
            onWarningTick?.Invoke();
            if (eventTimer >= warningDuration)
            {
                onWarningEnd?.Invoke();
                state = PenaltyState.Penalty;
                eventTimer = 0f;
                player.Kill();
                onPenaltyReceived?.Invoke();
                return;
            }
        }
    }

    public enum PenaltyState
    {
        Waiting,
        Excusing,
        Warning,
        Penalty
    }
}
