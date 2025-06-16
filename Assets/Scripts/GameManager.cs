using System;
using JackHank.Cinematics;
using LevelManagement;
using LevelTask;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Local;

    public static Action<GameRunInfo> OnRunBegin;
    public static Action<GameRunInfo> OnRunFinish;
    public static Action OnDeath;

    private static LevelDefinition _definition;
    private static LevelTaskTracker[] _levelTaskTrackers;
    private static bool _isInitialized;
    [SerializeField] private Bonus_GUI bonusGUI;
    [SerializeField] private LevelDefinition debugLevelDefinition;
    public BossFightManager bossFightManager;
    [SerializeField] private SceneEnter sceneEnter;
    [SerializeField] private SceneExit sceneExit;

    [Header("Optional dependency")] [SerializeField]
    private IntroCutscenePlayer introCutscenePlayer;

    public static LevelTaskDefinition[] LevelTasks => _definition.LevelTasks;
    public static GameRunInfo RunInfo { get; private set; }
    public static bool IsDuringRun { get; private set; }
    public static PlayerVehicle PlayerVehicle { get; private set; }

    private void Update()
    {
        if (IsDuringRun) GameRunFrameTick();
    }

    private void Start()
    {
        if (Debug.isDebugBuild && !_isInitialized)
            Initialize(debugLevelDefinition);

        if (!_isInitialized) Debug.LogError("Game Manager was not initialized");

        SetupReferences();

        if (PlayerPrefs.HasKey("StartFromBossFight"))
        {
            sceneEnter.Disable();
            ClearPlayerPrefs();
            Local.bossFightManager.Begin();
            introCutscenePlayer.showOnDebug = false;
            BeginRun();
        }
        else if (introCutscenePlayer != null && introCutscenePlayer.WillPlay() && introCutscenePlayer.enabled)
        {
            sceneEnter.Disable();
            introCutscenePlayer.OnceOnSceneFinished += () =>
            {
                if (sceneEnter.UseEnter) sceneEnter.TeleportPlayerAtEnter();

                BeginRun();
            };
        }
        else
        {
            BeginRun();
        }
    }

    public void SetupReferences()
    {
        PlayerVehicle = FindFirstObjectByType<PlayerVehicle>();
        Local = this;
        RunInfo = new GameRunInfo();
        IsDuringRun = true;
    }

    public void Initialize(LevelDefinition definition)
    {
        if (_isInitialized) return;
        sceneExit.OnExit += OnLevelEnds;
        Application.quitting += ClearPlayerPrefs;

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
        if (CinematicPlayer.isPlaying) return;
        RunInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        Local.bonusGUI.ShowBonus(bonusPool, bonusType);
    }

    public static void UpdateBonus(int bonusValue, PlayerBonusTypes bonusType)
    {
        if (CinematicPlayer.isPlaying) return;
        RunInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        Local.bonusGUI.ShowBonus(bonusValue, bonusType);
    }

    public static void UpdateCombo(PlayerBonusTypes bonusType, int bonusValue, int combo, int bonusPool)
    {
        if (CinematicPlayer.isPlaying) return;
        RunInfo.ChangeBonusBountyBy(bonusValue, bonusType);
        Local.bonusGUI.ShowComboBonus(bonusType, bonusPool, combo);
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

    public static void PlayerDeathRestart()
    {
        PlayerVehicle.playerTurret.DisallowFire();
        PlayerVehicle._botDirect.enabled = false;
        PlayerVehicle.physics.enabled = false;
        PlayerVehicle.enabled = false;

        OnDeath?.Invoke();

        ScreenFade.onAfterIn += AfterFadeIn;
        ScreenFade.In(2, ScreenFadeType.Skull);

        void AfterFadeIn()
        {
            Debug.Log("AfterFadeIn");
            ScreenFade.onAfterIn -= AfterFadeIn;
            if (Local.bossFightManager.duringFight)
            {
                RestartBossFight();
            }
            else
            {
                FinishRun();
                RestartLevel();
            }
        }
    }

    public static void RestartLevel()
    {
        GameSceneManager.ReloadLevel();
    }

    public static void RestartBossFight()
    {
        PlayerPrefs.SetInt("StartFromBossFight", 1);
        var runStats = RunInfo.GetPointsByBonusTypes();
        foreach (var key in runStats.Keys) PlayerPrefs.SetInt(key.ToString(), runStats[key]);
        PlayerPrefs.Save();

        GameSceneManager.ReloadLevel();
    }

    private void BeginRun()
    {
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


        if (CinematicPlayer.isPlaying) return;

        RunInfo.Time += Time.deltaTime;
    }

    private void ClearPlayerPrefs()
    {
        foreach (var bonusType in RunInfo.GetBonusTypes())
        {
            var key = bonusType.ToString();
            RunInfo.ChangeBonusBountyBy(PlayerPrefs.GetInt(key), bonusType);

            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.DeleteKey("StartFromBossFight");
        PlayerPrefs.Save();
    }

    public void OnLevelEnds()
    {
        LevelManager.SetLevelAsCompleted(_definition);
    }
}