using System;
using UnityEngine;

using JackHank.Cinematics;

public class IntroCutscenePlayer : MonoBehaviour
{
    [SerializeField] private CinematicSequence introCutscene;
    [SerializeField] private Transform cutsceneTransform;
    [SerializeField] private bool showOnDebug;

    private bool _afterInspectorFieldChecks;

    private void Awake()
    {
        var errorMessageStart = $"component [{GetType().Name}]; game object [{name}]:";
        if (introCutscene == null)
            Debug.LogError(errorMessageStart + " no Cinematic Sequence");
        if (cutsceneTransform == null)
            Debug.LogError(errorMessageStart + " transform for cinematic");

        _afterInspectorFieldChecks = true;
    }

    private void Start()
    {
        if (!_afterInspectorFieldChecks) return;

        if (!WillPlay()) return;

        if (CinematicPlayer.isPlaying)
            Debug.LogError(
                $"component [{GetType().Name}]; game object [{name}]: cant play intro cutscene, there is another playing cinematic");


        CinematicPlayer.PlaySequence(introCutscene, cutsceneTransform.position, cutsceneTransform.rotation,
            transform.localScale);

        CinematicPlayer.onEndPlay += AfterIntroCutscene;

        return;

        void AfterIntroCutscene()
        {
            if (OnceOnSceneFinished == null) return;

            OnceOnSceneFinished.Invoke();
            foreach (var @delegate in OnceOnSceneFinished.GetInvocationList()) OnceOnSceneFinished -= (Action)@delegate;

            AccountManager.LoggedInPlayerAccount.SetPlayerWatchedIntroCutscene();

            CinematicPlayer.onEndPlay -= AfterIntroCutscene;
        }
    }

    public bool WillPlay()
    {
        return (Debug.isDebugBuild && showOnDebug) ||
               (AccountManager.IsLoggedIn() && !AccountManager.LoggedInPlayerAccount.HasWatchedIntoCutscene());
    }

    public event Action OnceOnSceneFinished;
}