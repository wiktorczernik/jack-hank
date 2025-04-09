using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOut : MonoBehaviour
{
    [SerializeField] private OutSceneNextSceneInputType howToTakeSceneName = OutSceneNextSceneInputType.FromInspector;
    [SerializeField] private string nextSceneName;

    private void Awake()
    {
        if (howToTakeSceneName == OutSceneNextSceneInputType.FromCode) nextSceneName = "";
    }

    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerVehicle>(out var player)) return;

        SceneManager.LoadScene(nextSceneName);
    }

    private enum OutSceneNextSceneInputType
    {
        FromCode,
        FromInspector,
    }
}


