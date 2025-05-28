using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using JackHank.Cinematics;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class ToolbarLogic
{
    public static bool requestedPlayhere { get; private set; }
    public static bool autoSkipCinematics
    {
        get => CinematicPlayer.autoSkip;
        private set => CinematicPlayer.autoSkip = value;
    }

    public static void RequestPlayHere()
    {
        requestedPlayhere = true;
        SetFirstBehaviourEnabled<SceneEnter>(false);
        SetFirstBehaviourEnabled<IntroCutscenePlayer>(false);
        EditorApplication.isPlaying = true;
    }
    public static void RequestDomainReload()
    {
        EditorUtility.RequestScriptReload();
    }
    public static void ToggleCinematicAutoskip()
    {
        autoSkipCinematics = !autoSkipCinematics;
    }

    static void OnEnterPlayMode()
    {
        if (requestedPlayhere)
        {
            var player = FindFirstBehaviour<PlayerVehicle>();
            Vector3 cameraPos;
            Quaternion cameraRot;
            if (player && GetEditorCameraTransform(out cameraPos, out cameraRot))
            {
                Vector3 playerPos = cameraPos;
                Quaternion playerRot = Quaternion.Euler(Vector3.up * cameraRot.eulerAngles.y);

                RaycastHit hit;
                if (Physics.Raycast(cameraPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    playerPos = hit.point;
                    playerPos += Vector3.up;
                }
                player.Teleport(playerPos, playerRot);
            }
        }
    }
    static void OnEnterEditMode()
    {
        requestedPlayhere = false;
        SetFirstBehaviourEnabled<SceneEnter>(true);
    }

    #region Editor Camera shortcuts
    public static bool GetEditorCameraTransform(out Vector3 position, out Quaternion rotation)
    {
        var sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }

        var transform = sceneView.camera.transform;
        position = transform.position;
        rotation = transform.rotation;
        return true;
    }
    #endregion
    #region MonoBehaviour shortcuts
    static T FindFirstBehaviour<T>() where T : MonoBehaviour
    {
        return MonoBehaviour.FindFirstObjectByType<T>();
    }
    static bool SetFirstBehaviourEnabled<T>(bool enabled) where T : MonoBehaviour
    {
        return SetBehaviourEnabled(FindFirstBehaviour<T>(), enabled);
    }
    static bool SetBehaviourEnabled(MonoBehaviour behaviour, bool enabled)
    {
        if (behaviour)
        {
            behaviour.enabled = enabled;
            return true;
        }
        return false;
    }
    #endregion

    #region Initialization
    // register an event handler when the class is initialized
    static ToolbarLogic()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }
    static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode) OnEnterEditMode();
        if (state == PlayModeStateChange.EnteredPlayMode) OnEnterPlayMode();
    }
    #endregion
}