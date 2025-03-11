using System;
using LevelTask;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;
    public static PlayerVehicle PlayerVehicle { get; private set; }

    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;

    public static Action<string, int> OnBigBounty;
    public static LevelTaskDefinition[] LevelTasks;
    private static LevelTaskTracker[] _levelTaskTrackers; // this value is taking from level definition when level scene is loading

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
        PlayerVehicle = FindFirstObjectByType<PlayerVehicle>();
        PlayerVehicle.onPickupPassenger.AddListener(OnPassengerPickup);

        SmashableEntity[] smashables = FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None);
        foreach (SmashableEntity smashable in smashables)
        {
            UnityAction<SmashableEntity> onHit;
            if (smashable is PickupablePassenger)
            {
                PickupablePassenger passenger = smashable as PickupablePassenger;
                passenger.StartLookingForPlayerVehicle(PlayerVehicle);
                onHit = OnPassengerHit;
            }
            else
            {
                onHit = OnHitSmashable;
            }

            smashable.OnHit.AddListener(onHit);
        }
        
        _levelTaskTrackers = new LevelTaskTracker[LevelTasks.Length];
        for (var i = 0; i < _levelTaskTrackers.Length; i++)
        {
            _levelTaskTrackers[i] = LevelTaskTracker.CreateTracker(gameObject, LevelTasks[i]);
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
        OnBigBounty?.Invoke("FRIEND CAUGHT", passenger.bountyPointsPenalty);
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
