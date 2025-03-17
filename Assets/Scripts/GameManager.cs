using System;
using LevelTask;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;
    public static PlayerVehicle PlayerVehicle { get; private set; }

    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;
    
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
        
        _levelTaskTrackers = new LevelTaskTracker[LevelTasks.Length];
        for (var i = 0; i < _levelTaskTrackers.Length; i++)
        {
            _levelTaskTrackers[i] = LevelTaskTracker.CreateTracker(gameObject, LevelTasks[i]);
        }
    }

    public static void ChangePlayerBonusBy(int value, PlayerBonusTypes bonusType)
    {
        runInfo.ChangeBonusBountyBy(value, bonusType);
    }
    
    public static void ChangePlayerBonusBy(int value, PlayerBonusTypes bonusType, int comboValue)
    {
        if (bonusType != PlayerBonusTypes.DestructionCombo)
            throw new ArgumentException("Parameter comboValue can be set only for destruction combo");
        
        runInfo.ChangeBonusBountyBy(value, bonusType);
        
    }
    
    private void GameRunFrameTick()
    {
        if (!isDuringRun) return;
        
        runInfo.Time += Time.deltaTime;   
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
