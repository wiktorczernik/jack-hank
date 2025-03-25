using System;
using System.Linq;
using LevelTask;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Bonus_GUI bonusGUI;
    [SerializeField] private LevelTaskDefinition _levelTaskTample;
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;
    public static PlayerVehicle PlayerVehicle { get; private set; }

    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;
    
    public static LevelTaskDefinition[] LevelTasks; 
    private static LevelTaskTracker[] _levelTaskTrackers; // this value is taking from level definition when level scene is loading
    private static GameManager _local;

    public void BeginRun()
    {
        LevelTasks = new[] { _levelTaskTample };
        _local = this;
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

    public static void UpdateBonus(int bonusValue, PlayerBonusTypes bonusType, int bonusPool)
    {
        runInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        _local.bonusGUI.ShowBonus(bonusPool, bonusType);
    }
    
    public static void UpdateBonus(int bonusValue,  PlayerBonusTypes bonusType)
    {
        runInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        _local.bonusGUI.ShowBonus(bonusValue, bonusType);
    }
    
    public static void UpdateDestructionCombo(int bonusValue, int combo, int bonusPool)
    {
        runInfo.ChangeBonusBountyBy(bonusValue, PlayerBonusTypes.DestructionCombo);
        _local.bonusGUI.ShowDestructionComboBonus(bonusPool, combo);
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
