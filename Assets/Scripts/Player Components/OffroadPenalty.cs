using Unity.Collections;
using UnityEngine;

public class OffroadPenalty : MonoBehaviour
{
    bool regIn = false; // registered inside
    bool penalted = false;
    public float eventTimer;
    public float maxOffroadTime;

    [SerializeField, ReadOnly] OffroadState state;

    PlayerVehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<PlayerVehicle>();
    }

    public void RegisterInside()
    {
        regIn = true;
    }

    private void Update() // actually normal update is used for game logic and fixed update for physics
    {
        if (regIn) eventTimer += Time.deltaTime;
        else eventTimer = 0;
        if (eventTimer > maxOffroadTime) { penalted = true; vehicle.Kill(); }
        regIn = false;

        state = GetState();
    }

    public OffroadState GetState()
    {
        if (penalted) return OffroadState.Penalty;
        else if (eventTimer == 0) return OffroadState.Waiting;
        return OffroadState.Warning;
    }

    public enum OffroadState
    {
        Waiting,
        Warning,
        Penalty
    }
}
