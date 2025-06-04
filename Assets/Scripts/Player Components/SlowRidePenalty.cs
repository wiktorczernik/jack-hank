using UnityEngine;
using UnityEngine.Events;


// Checks if the player bus moves too slow. If so, the player receives a warning, then a penalty.
public class SlowRidePenalty : MonoBehaviour
{
    [Tooltip("Current script state")] public PenaltyState penaltyState;
    [Tooltip("The threshold velocity when the script gets triggered. In km/h")] public float maxPenaltyVelocity;
    public float eventTimer;

    [Tooltip("How long will the excusing period last")] public float excusingDuration;
    [Tooltip("How long will the warning period last")] public float warningDuration;

    VehiclePhysics physics;

    public UnityEvent onWarningBegin;
    public UnityEvent onWarningTick;
    public UnityEvent onWarningEnd;
    public UnityEvent onPenaltyReceived;

    private void Awake()
    {
        physics = GetComponent<PlayerVehicle>().physics;
    }

    private void FixedUpdate()
    {
        if (physics.speedKmh >= maxPenaltyVelocity)
        {
            penaltyState = PenaltyState.Waiting;
            eventTimer = 0f;
            return;
        }

        eventTimer += Time.fixedDeltaTime;
        if (penaltyState == PenaltyState.Waiting) penaltyState = PenaltyState.Excusing;
        if (penaltyState == PenaltyState.Excusing && eventTimer >= excusingDuration)
        {
            penaltyState = PenaltyState.Warning;
            eventTimer = 0f;
            onWarningBegin?.Invoke();
            return;
        }
        if (penaltyState == PenaltyState.Warning)
        {
            onWarningTick?.Invoke();
            if (eventTimer >= warningDuration)
            {
                onWarningEnd?.Invoke();
                penaltyState = PenaltyState.Penalty;
                eventTimer = 0f;
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
