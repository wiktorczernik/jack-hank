using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using LevelManagement;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;
    private static Scene _essentialsScene;


    public static void LoadLevel(LevelInfo level) => _instance.StartCoroutine(LoadLevelAsync(level));
    public static void LoadMenu() => _instance.StartCoroutine(LoadMenuAsync());
    public static IEnumerator LoadLevelAsync(LevelInfo level)
    {
        UnloadAllExceptCritical();
        yield return LoadActiveSceneAsync(level.LevelSceneName);
    }
    public static IEnumerator LoadMenuAsync()
    {
        UnloadAllExceptCritical();
        yield return LoadActiveSceneAsync(GameScenes.menu);
    }

    #region Helpers
    private static IEnumerator LoadActiveSceneAsync(string sceneName)
    {
        UnloadAllExceptCritical();

        var menuLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        menuLoadOperation.allowSceneActivation = false;

        while (!menuLoadOperation.isDone)
        {
            if (menuLoadOperation.progress >= 0.9f)
            {
                menuLoadOperation.allowSceneActivation = true;

                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
            yield return null;
        }
        var loginScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loginScene);
    }
    private static void UnloadAllExceptCritical()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene == activeScene || scene == _essentialsScene) continue;

            SceneManager.UnloadSceneAsync(scene);
        }
    }
    #endregion

    private void Awake()
    {
        _instance = this;

        _essentialsScene = SceneManager.GetSceneByName("Essentials");
    }
}
