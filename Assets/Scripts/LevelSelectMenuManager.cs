using System.Collections;
using JackHank.Dialogs;
using LevelManagement;
using UnityEngine;

public class LevelSelectMenuManager : MonoBehaviour
{
    [SerializeField] private Transform botDestination;
    [SerializeField] private SceneExit _sceneExit;
    [SerializeField] private Dialog dialog;
    [SerializeField] private LevelComplete_GUI levelCompleteGUI;
    
    private LevelSelectMenuScroller _levelScroller;
    private BotVehicle _botVehicle;
    
    public static LevelSelectMenuState State { get; private set; }
    public static LevelInfo[] NextLevels { get; private set; }
    
    private static LevelSelectMenuManager _instance;
    private static bool _initialized;

    public static void SetLevelSelectedState(LevelInfo selectedLevel)
    {
        if (State == LevelSelectMenuState.LevelSelected) return;

        _instance._sceneExit.SetNextLevel(selectedLevel.levelID);
        
        _instance._levelScroller.StopScrolling();
        _instance._botVehicle.isFollowing = true;
        _instance._botVehicle.followMode = BotVehicle.FollowMode.Single; 
        _instance._botVehicle.destinationPoint = _instance.botDestination.position;
        _instance._botVehicle.followMaxSpeed = 100;
    }

    private static void Initialize()
    {
        if (_initialized) return;

        Debug.Log("init");

        State = LevelSelectMenuState.Idle;
        //NextLevels = LevelManager.GetAvailableLevels().ToArray();
        //var nextLevelsUI = FindFirstObjectByType<NextAccessibleLevels_GUI>();
        //nextLevelsUI.InitializeWithNextLevels(NextLevels);
        //nextLevelsUI.OnLevelSelected += SetLevelSelectedState;

        _instance._sceneExit = FindFirstObjectByType<SceneExit>();
        _instance._sceneExit.OnExit += ResetState;

        _instance._levelScroller = FindFirstObjectByType<LevelSelectMenuScroller>();
        _instance._levelScroller.StartScrolling();

        _instance._botVehicle = FindFirstObjectByType<BotVehicle>();
        _instance._botVehicle.isFollowing = false;

        var slowRidePenalty = _instance._botVehicle.GetComponent<SlowRidePenalty>();
        if (slowRidePenalty != null) slowRidePenalty.enabled = false;

        _initialized = true;
    }

    private void Start()
    {
        Debug.Log("Start");
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        ScreenFade.Out(1f);

        yield return new WaitWhile(() => ScreenFade.isBusy);
        
        DialogPlayer.Request(_instance.dialog);

        yield return new WaitUntil(() => DialogPlayer.playbackState == null);
        
        _instance.levelCompleteGUI.Appear();

        yield return new WaitWhile(() => _instance.levelCompleteGUI.isBusy);
        
        ScreenFade.In(1f);
        
        yield return new WaitWhile(() => ScreenFade.isBusy);
        
        GameSceneManager.LoadLogin();
    }

    private static void ResetState()
    {
        _initialized = false;
        State = LevelSelectMenuState.Idle;
    }

    private void Awake()
    {
        _instance = this;

        Debug.Log(Debug.isDebugBuild);
        Debug.Log(GameSceneManager.isLoading);

        if (Debug.isDebugBuild && !GameSceneManager.isLoading)
        {
            Initialize();
        }

        if (!Debug.isDebugBuild)
        {
            GameSceneManager.onMenuLoadEnd += Initialize;
        }
    }
}

public enum LevelSelectMenuState
{
    Cutscene, Idle, LevelSelected
}