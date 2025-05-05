using System;
using System.Collections;
using LevelManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static bool isLoading;
    public static float loadingProgress;

    private static GameSceneManager _instance;
    private static Scene _essentialsScene;

    private void Awake()
    {
        _instance = this;

        _essentialsScene = SceneManager.GetSceneByName("Essentials");
    }

    public static event Action onLoadTick;
    public static event Action onMenuLoadBegin;
    public static event Action onMenuLoadEnd;
    public static event Action<LevelInfo> onLevelLoadBegin;
    public static event Action<LevelInfo> onLevelLoadEnd;


    public static void LoadLevel(LevelInfo level)
    {
        _instance.StartCoroutine(LoadLevelAsync(level));
    }

    public static void LoadMenu()
    {
        _instance.StartCoroutine(LoadMenuAsync());
    }

    public static void LoadFirstLevel()
    {
        _instance.StartCoroutine(LoadFirstLevelAsync());
    }

    public static IEnumerator LoadLevelAsync(LevelInfo level)
    {
        onLevelLoadBegin?.Invoke(level);
        yield return LoadActiveSceneAsync(
            level.LevelSceneName,
            () => { onLevelLoadBegin?.Invoke(level); },
            () => { onLevelLoadEnd?.Invoke(level); });
        onLevelLoadEnd?.Invoke(level);
    }

    public static IEnumerator LoadFirstLevelAsync()
    {
        yield return LoadLevelAsync(LevelManager.GetLevelsList()[0]);
    }

    public static IEnumerator LoadMenuAsync()
    {
        yield return LoadActiveSceneAsync(GameScenes.menu, () => onMenuLoadBegin?.Invoke(),
            () => onMenuLoadEnd?.Invoke());
    }

    public static void ReloadLevel()
    {
        _instance.StartCoroutine(LoadActiveSceneAsync(SceneManager.GetActiveScene().name, null, null));
    }

    #region Helpers

    private static IEnumerator LoadActiveSceneAsync(string sceneName, Action beginEvent, Action endEvent)
    {
        isLoading = true;
        loadingProgress = 0.0f;
        beginEvent?.Invoke();

        yield return UnloadAllExceptCritical();

        onLoadTick?.Invoke();

        var menuLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        menuLoadOperation.allowSceneActivation = false;

        while (!menuLoadOperation.isDone)
        {
            loadingProgress = menuLoadOperation.progress;
            if (menuLoadOperation.progress >= 0.9f)
            {
                menuLoadOperation.allowSceneActivation = true;

                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }

            onLoadTick?.Invoke();
            yield return null;
        }

        loadingProgress = 1.0f;
        onLoadTick?.Invoke();

        var loginScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loginScene);

        endEvent?.Invoke();
        isLoading = false;
        loadingProgress = 0.0f;
    }

    private static IEnumerator UnloadAllExceptCritical()
    {
        var activeScene = SceneManager.GetActiveScene();
        for (var i = 0; i < SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (scene == activeScene || scene == _essentialsScene) continue;

            yield return SceneManager.UnloadSceneAsync(scene);
        }
    }

    #endregion
}