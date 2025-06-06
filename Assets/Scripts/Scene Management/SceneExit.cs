using System;
using System.Collections;
using UnityEngine;
using LevelManagement;

public class SceneExit : MonoBehaviour
{
    [SerializeField] private SceneExitMode mode = SceneExitMode.LevelMode;
    [Tooltip("Delay AFTER finish text animation")]
    [Range(0, 60)][SerializeField] private float delayBeforeNewScene = 5f;
    
    [Header("Triggler settings")]
    [SerializeField] private TriggerEventEmitter exitTrigger;
    [SerializeField] private bool useAutoDrivingToExit = true;
    [Header("Next options will be used only if [useAutoDrivingToExit] is set to true]")]
    [SerializeField] private TriggerEventEmitter autoDrivingTrigger;
    [SerializeField] private Transform autoDriveDestination; 
    [SerializeField] private BotVehicle player;
    
    [Header("Finish text (is showing only in LevelMode)")]
    [SerializeField] private FinishText_GUI finishText;
    
    public event Action OnExit;
    private bool _finishing;

    private int _nextLevelId;

    private void Awake()
    {
        autoDrivingTrigger.OnEnter.AddListener(OnPlayerEnterAutoDrivingTrigger);
        exitTrigger.OnEnter.AddListener(OnPlayerEnterExitTrigger);
    }

    public void SetNextLevel(int levelId)
    {
        if (mode != SceneExitMode.MenuMode) return;
        
        _nextLevelId = levelId;
    }

    private void OnPlayerEnterAutoDrivingTrigger(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (!useAutoDrivingToExit) return;
        
        player.isFollowing = true;
        player.followMode = BotVehicle.FollowMode.Single; 
        player.destinationPoint = autoDriveDestination.position;
        player.followMaxSpeed = 100;
    }

    private void OnPlayerEnterExitTrigger(Collider other)
    {
        Debug.Log("Player entered exit");
        if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log($"IS FINISHING: {_finishing}");
        if (_finishing) return;
        _finishing = true;
        Debug.Log("Start finishing");
        ScreenFade.In(1.5f, ScreenFadeType.Default);

        ScreenFade.onAfterIn += AfterFadeIn;
    }

    private void AfterFadeIn()
    {
        ScreenFade.onAfterIn -= AfterFadeIn;
        if (mode == SceneExitMode.LevelMode)
        {
            finishText.ShowFinishMark(GameManager.GetMarkByBounty(), GameManager.RunInfo.GetPointsByBonusTypes());
            finishText.OnEndAnimation += ExitToMenu;
        }
        else
        {
            OnExit?.Invoke();
            GameSceneManager.LoadMenu();
            StartCoroutine(ExitToLevel());
        }
    }

    private IEnumerator ExitToLevel()
    {
        var level = LevelManager.GetLevelByID(_nextLevelId);
        
        if (level == null) Debug.LogError($"SceneExit: no level with id {_nextLevelId}");
        
        yield return new WaitForSeconds(delayBeforeNewScene);
        
        GameSceneManager.LoadLevel(level);
    }

    private void ExitToMenu()
    {
        finishText.OnEndAnimation -= ExitToMenu;
        StartCoroutine(ExitToMenuCo());
    }

    private IEnumerator ExitToMenuCo()
    {
        yield return new WaitForSeconds(delayBeforeNewScene);
        OnExit?.Invoke();
        GameSceneManager.LoadMenu();
    }

    private enum SceneExitMode
    {
        LevelMode, // W trybie 'LevelMode' SceneExit przenosi gracza do menu 'korytaż'
        MenuMode, // W trybie 'MenuMode' SceneExit prenosi gracza na poziom odzysakny prez metodę setNextLevel
    }
}


