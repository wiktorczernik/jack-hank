using UnityEditor;
using UnityEngine;

namespace JackHank.EditorUtils.Toolbar
{
    public class ToolbarWindow : EditorWindow
    {
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
            float padding = 10;

            Rect playHereRect = new Rect(new Vector2(padding, padding), new Vector2(100, position.height - 2 * padding));

            GUI.enabled = !EditorApplication.isPlaying;
            if (GUI.Button(playHereRect, "Play"))
            {
                ToolbarLogic.PlayHere();
            }
            GUI.enabled = !EditorApplication.isPlaying;
            if (GUI.Button(playHereRect, "Play Here"))
            {
                ToolbarLogic.PlayHere();
            }
            GUI.enabled = true;
        }
    }
}
