using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        var mainMenuLoad = SceneManager.LoadSceneAsync(GameScenes.mainMenu, LoadSceneMode.Additive);
        var essentialsLoad = SceneManager.LoadSceneAsync(GameScenes.essentials, LoadSceneMode.Additive);
        
        essentialsLoad.allowSceneActivation = false;
        mainMenuLoad.allowSceneActivation = false;

        while (!essentialsLoad.isDone || !mainMenuLoad.isDone)
        {
            if (essentialsLoad.progress >= 0.9f && mainMenuLoad.progress >= 0.9f)
            {
                // Audio listener is removed to prevent warnings
                var listener = Camera.main.GetComponent<AudioListener>();
                Destroy(listener);
                
                essentialsLoad.allowSceneActivation = true;
                mainMenuLoad.allowSceneActivation = true;

                SceneManager.UnloadSceneAsync(GameScenes.bootstrap);

                yield return null;

                var mainMenu = SceneManager.GetSceneByName(GameScenes.mainMenu);
                SceneManager.SetActiveScene(mainMenu);
            }
            yield return null;
        }

    }
}
