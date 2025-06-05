using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        var essentialsLoad = SceneManager.LoadSceneAsync(GameScenes.essentials, LoadSceneMode.Additive);
        essentialsLoad.allowSceneActivation = false;

        while (essentialsLoad.progress < 0.9f)
            yield return null;

        essentialsLoad.allowSceneActivation = true;

        while (!essentialsLoad.isDone)
            yield return null;

        var listener = Camera.main.GetComponent<AudioListener>();
        if (listener != null)
            Destroy(listener);

        var mainMenuLoad = SceneManager.LoadSceneAsync(GameScenes.mainMenu, LoadSceneMode.Additive);
        mainMenuLoad.allowSceneActivation = false;

        while (mainMenuLoad.progress < 0.9f)
            yield return null;

        mainMenuLoad.allowSceneActivation = true;
        while (!mainMenuLoad.isDone)
            yield return null;

        SceneManager.UnloadSceneAsync(GameScenes.bootstrap);
        var mainMenuScene = SceneManager.GetSceneByName(GameScenes.mainMenu);
        SceneManager.SetActiveScene(mainMenuScene);
    }
}
