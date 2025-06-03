using UnityEngine;
using UnityEngine.Events;

public class SlowRidePenalty : MonoBehaviour
{
    public PenaltyState penaltyState;
    [Tooltip("In km/h")] public float maxPenaltyVelocity;
    public float eventTimer;

    public float chargeDuration;
    public float warnDuration;

    VehiclePhysics physics;

    public UnityEvent onWarningBegin;
    public UnityEvent onWarningTick;
    public UnityEvent onPenaltyReceived;

    private void Awake()
    {
        physics = GetComponent<PlayerVehicle>().physics;
    }

    private void Update()
    {
        if (physics.bodyRigidbody.linearVelocity.magnitude * 3.6f >= maxPenaltyVelocity) 
        {
            penaltyState = PenaltyState.Waiting;
            eventTimer = 0f;
            return;
        }

        eventTimer += Time.deltaTime;
        if (penaltyState == PenaltyState.Waiting) penaltyState = PenaltyState.Charging;
        if (penaltyState == PenaltyState.Charging && eventTimer >= chargeDuration)
        {
            penaltyState = PenaltyState.Warning;
            eventTimer = 0f;
            onWarningBegin?.Invoke();
            return;
        }
        if (penaltyState == PenaltyState.Warning)
        {
            onWarningTick?.Invoke();
            if (eventTimer >= warnDuration)
            {
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
        Charging,
        Warning,
        Penalty
    }
}
