using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;


    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;
    
    
    public void BeginRun()
    {
        runInfo = new GameRunInfo();
        isDuringRun = true;
        OnRunBegin?.Invoke(runInfo);
        PrepareLevelEntities();
    }

    public static void FinishRun()
    {
        isDuringRun = false;
        OnRunFinish?.Invoke(runInfo);
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

    private void OnPassengerHit(SmashableEntity smashable) => OnPassengerHit((PickupablePassenger)smashable);
    private void OnPassengerHit(PickupablePassenger passenger)
    {
        runInfo.AddBountyPenalty(passenger.bountyPointsPenalty);
        Debug.Log("Passenger was hit! Oh no!");
    }
    private void OnPassengerPickup(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        runInfo.AddPassenger();
        runInfo.AddBountyPoints(passenger.bountyPointsReward);
    }
    private void OnHitSmashable(SmashableEntity smashable)
    {
        runInfo.AddBountyPoints(smashable.bountyPointsReward);
    }
    private void GameRunFrameTick()
    {
        if (!isDuringRun) return;
        
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
