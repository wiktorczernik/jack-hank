using System;
using LevelManagement;
using LevelTask;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Local;

    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;

    private static LevelDefinition _definition;
    private static LevelTaskTracker[] _levelTaskTrackers;
    private static bool _isInitialized;
    [SerializeField] private Bonus_GUI bonusGUI;
    [SerializeField] private LevelDefinition debugLevelDefinition;
    [SerializeField] private FadeTransition_GUI fadeTransition;
    public static LevelTaskDefinition[] LevelTasks => _definition.LevelTasks;
    public static GameRunInfo RunInfo { get; private set; }
    public static bool IsDuringRun { get; private set; }
    public static PlayerVehicle PlayerVehicle { get; private set; }

    private void Awake()
    {
        if (Debug.isDebugBuild && !_isInitialized)
            Initialize(debugLevelDefinition);

        if (!_isInitialized) Debug.LogError("Game Manager was not initialzied");

        BeginRun();
    }

    private void Update()
    {
        if (IsDuringRun) GameRunFrameTick();
    }

    public void Initialize(LevelDefinition definition)
    {
        if (_isInitialized) return;
        if (definition == null)
        {
            Debug.LogError("Game Manager can not be initialized with null Level Definition");
            return;
        }

        _definition = definition;
        CreateTaskTrackers();
        _isInitialized = true;
    }

    public static void FinishRun()
    {
        IsDuringRun = false;
        OnRunFinish?.Invoke(RunInfo);
    }

    public static void UpdateBonus(int bonusValue, PlayerBonusTypes bonusType, int bonusPool)
    {
        RunInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        Local.bonusGUI.ShowBonus(bonusPool, bonusType);
    }

    public static void UpdateBonus(int bonusValue, PlayerBonusTypes bonusType)
    {
        RunInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        Local.bonusGUI.ShowBonus(bonusValue, bonusType);
    }

    public static void UpdateDestructionCombo(int bonusValue, int combo, int bonusPool)
    {
        RunInfo.ChangeBonusBountyBy(bonusValue, PlayerBonusTypes.DestructionCombo);
        Local.bonusGUI.ShowDestructionComboBonus(bonusPool, combo);
    }

    public static LevelCompletenessMark GetMarkByBounty()
    {
        if (RunInfo.AllBountyPoints <= _definition.E) return LevelCompletenessMark.E;
        if (RunInfo.AllBountyPoints <= _definition.D) return LevelCompletenessMark.D;
        if (RunInfo.AllBountyPoints <= _definition.C) return LevelCompletenessMark.C;
        if (RunInfo.AllBountyPoints <= _definition.B) return LevelCompletenessMark.B;
        if (RunInfo.AllBountyPoints <= _definition.A) return LevelCompletenessMark.A;

        return LevelCompletenessMark.S;
    }

    public static void RestartLevel()
    {
        Local.fadeTransition.StartFadeIn();
        Local.fadeTransition.OnFadeInEnded += AfterFadeIn;

        return;

        void AfterFadeIn()
        {
            SceneManager.LoadScene(_definition.SceneName);
            Local.fadeTransition.OnFadeInEnded -= AfterFadeIn;
        }
    }

    public static void RestartBossFight()
    {
    }

    private void BeginRun()
    {
        PlayerVehicle = FindFirstObjectByType<PlayerVehicle>();
        Local = this;
        RunInfo = new GameRunInfo();
        IsDuringRun = true;
        OnRunBegin?.Invoke(RunInfo);
    }

    private void CreateTaskTrackers()
    {
        if (LevelTasks.Length == 0) return;

        _levelTaskTrackers = new LevelTaskTracker[LevelTasks.Length];
        for (var i = 0; i < _levelTaskTrackers.Length; i++)
            _levelTaskTrackers[i] = LevelTaskTracker.CreateTracker(gameObject, LevelTasks[i]);
    }

    private void GameRunFrameTick()
    {
        if (!IsDuringRun) return;

        RunInfo.Time += Time.deltaTime;
    }
}