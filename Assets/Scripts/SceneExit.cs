using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    private BotVehicle _botVehicle;
    

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
        
        _botVehicle.isFollowing = true;
        _botVehicle.followMode = BotVehicle.FollowMode.Single; 
        _botVehicle.destinationPoint = pointToDesignateFroBot.position;
        _botVehicle.followMaxSpeed = 100;
    }

    private void OnPlayerEnterExitZone(Collider other)
    {
        Debug.Log("Something entered");
        if (!other.gameObject.CompareTag("Vehicle")) return;
        Debug.Log("IT IS PLAYER");
        SceneManager.LoadScene(nextSceneName);
    }

    private enum OutSceneNextSceneInputType
    {
        FromCode,
        FromInspector,
    }
}


