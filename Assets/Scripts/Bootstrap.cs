using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        var loginLoad = SceneManager.LoadSceneAsync(GameScenes.login, LoadSceneMode.Additive);
        var essentialsLoad = SceneManager.LoadSceneAsync(GameScenes.essentials, LoadSceneMode.Additive);

        loginLoad.allowSceneActivation = false;
        essentialsLoad.allowSceneActivation = false;

        while (!loginLoad.isDone || !essentialsLoad.isDone)
        {
            if (loginLoad.progress >= 0.9f && essentialsLoad.progress >= 0.9f)
            {
                // Audio listener is removed to prevent warnings
                var listener = Camera.main.GetComponent<AudioListener>();
                Destroy(listener);

                loginLoad.allowSceneActivation = true;
                essentialsLoad.allowSceneActivation = true;

                SceneManager.UnloadSceneAsync(GameScenes.bootstrap);

                yield return null;

                var loginScene = SceneManager.GetSceneByName(GameScenes.login);
                SceneManager.SetActiveScene(loginScene);
            }
            yield return null;
        }

    }
}
