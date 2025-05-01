using UnityEditor;
using UnityEngine;

namespace JackHank.EditorUtils.Toolbar
{
    public class ToolbarWindow : EditorWindow
    {
        const float windowPadding = 10f;
        const float elementPadding = 5f;
        const float buttonWidth = 100f;
        const float largeButtonWidth = 200f;

        [MenuItem("Window/JackHank/Toolbar")]
        public static void ShowWindow()
        {
            var instance = (ToolbarWindow) GetWindow(typeof(ToolbarWindow));

            instance.titleContent = new GUIContent("Toolbar");
            instance.minSize = new Vector2(500, 100);
            instance.maxSize = instance.minSize;
        }
        void OnGUI()
        {
            Rect playHereRect = new Rect(
                position: new Vector2(windowPadding, windowPadding), 
                size: new Vector2(buttonWidth, position.height - 2 * windowPadding)
            );
            Rect domainReloadRect = new Rect(
                position: new Vector2(windowPadding + buttonWidth + elementPadding, windowPadding),
                size: new Vector2(buttonWidth, position.height - 2 * windowPadding)
            );
            Rect cinemaSkipRect = new Rect(
                position: new Vector2(windowPadding + buttonWidth + elementPadding + buttonWidth + elementPadding, windowPadding),
                size: new Vector2(largeButtonWidth, position.height - 2 * windowPadding)
            );

            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isPlaying;
            if (GUI.Button(playHereRect, "Play Here"))
            {
                ToolbarLogic.RequestPlayHere();
            }
            
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isPlaying;
            if (GUI.Button(domainReloadRect, "Reload Domain"))
            {
                ToolbarLogic.RequestDomainReload();
            }

            GUI.enabled = !EditorApplication.isPlaying;
            if (GUI.Button(cinemaSkipRect, ToolbarLogic.autoSkipCinematics ? "Disable Cinema Skip" : "Enable Cinema Skip"))
            {
                ToolbarLogic.ToggleCinematicAutoskip();
            }
        }
    }
}
