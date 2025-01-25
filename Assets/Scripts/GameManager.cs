using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;


    public static Action<GameRunInfo> OnRunBegin;

    public void BeginRun()
    {
        runInfo = new GameRunInfo();
        isDuringRun = true;
        OnRunBegin?.Invoke(runInfo);
        PrepareLevelEntities();
    }

    private void PrepareLevelEntities()
    {
        PlayerVehicle playerVehicle = FindFirstObjectByType<PlayerVehicle>();

        playerVehicle.onPickupPassenger.AddListener(OnPassengerPickup);

        SmashableEntity[] smashables = FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None);
        foreach (SmashableEntity smashable in smashables)
        {
            UnityAction<SmashableEntity> onHit;
            if (smashable is PickupablePassenger)
            {
                PickupablePassenger passenger = smashable as PickupablePassenger;
                passenger.StartLookingForPlayerVehicle(playerVehicle);
                onHit = OnPassengerHit;
            }
            else
            {
                onHit = OnHitSmashable;
            }

            smashable.OnHit.AddListener(onHit);
        }
    }

    private void OnPassengerHit(SmashableEntity smashable)
    {
        Debug.Log("Passenger was hit! Oh no!");
    }
    private void OnPassengerPickup(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        runInfo.AddPassenger();
    }
    private void OnHitSmashable(SmashableEntity smashable)
    {
        runInfo.AddBountyPoints(smashable.bountyPointsReward);
    }
    private void GameRunFrameTick()
    {
        runInfo.time += Time.deltaTime;   
    }


    private void Awake()
    {
        BeginRun();
    }
    private void Update()
    {
        if (isDuringRun)
        {
            GameRunFrameTick();
        }
    }
}
