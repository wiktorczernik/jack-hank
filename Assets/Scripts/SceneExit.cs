using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using JackHank.Cinematics;

public class SceneExit : MonoBehaviour
{
    [SerializeField] private OutSceneNextSceneInputType howToTakeSceneName = OutSceneNextSceneInputType.FromInspector;
    [Tooltip("The value of this field set via inspector is used only if field [howToTakeSceneName] is set to [FromInspector] otherwise it is ignored")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private TriggerEventEmitter exitZone;
    [SerializeField] private bool useBotWhenPlayerInTrigger = true;
    [Header("Next options will be used only if [useBotWhenPlayerInTrigger] is set to true]")]
    [SerializeField] private TriggerEventEmitter turnOnBotZone;
    [SerializeField] private Transform pointToDesignateFroBot;
    [SerializeField] private BotVehicle botVehicle;
    [SerializeField] private bool showStatisticsOnExit;
    [Header("GUI")]
    [SerializeField] private FinishText_GUI finishText;
    [SerializeField] private FadeTransition_GUI fadeTransition;

    public event Action OnExit;
    public bool finishing = false;

    private void Awake()
    {
        if (howToTakeSceneName == OutSceneNextSceneInputType.FromCode) nextSceneName = "";

        turnOnBotZone.OnEnter.AddListener(OnPlayerEnterInBotZone);
        exitZone.OnEnter.AddListener(OnPlayerEnterExitZone);
    }

    public void SetNextScene(string sceneName)
    {
        if (howToTakeSceneName != OutSceneNextSceneInputType.FromCode) return;
        
        nextSceneName = sceneName;
    }

    private void OnPlayerEnterInBotZone(Collider other)
    {
        if (!other.gameObject.CompareTag("Vehicle")) return;
        
        if (!useBotWhenPlayerInTrigger) return;
        
        botVehicle.isFollowing = true;
        botVehicle.followMode = BotVehicle.FollowMode.Single; 
        botVehicle.destinationPoint = pointToDesignateFroBot.position;
        botVehicle.followMaxSpeed = 100;
    }

    private void OnPlayerEnterExitZone(Collider other)
    {
        if (!other.gameObject.CompareTag("Vehicle")) return;
        if (finishing) return;
        finishing = true;

        fadeTransition.StartFadeIn();

        fadeTransition.OnFadeInEnded += AfterFadeIn;
    }

    private void AfterFadeIn()
    {
        if (showStatisticsOnExit)
        {
            finishText.ShowFinishMark(GameManager.GetMarkByBounty(), GameManager.RunInfo.GetPointsByBonusTypes());
            finishText.OnEndAnimation += LateExit;
        }
        else
        {
            OnExit?.Invoke();
            GameSceneManager.LoadActiveScene(nextSceneName, null, null);
        }
    }

    private void LateExit()
    {
        OnExit?.Invoke();
        GameSceneManager.LoadActiveScene(nextSceneName, null, null);
    }

    private enum OutSceneNextSceneInputType
    {
        FromCode,
        FromInspector,
    }
}


