using UnityEditor;
using UnityEngine;

namespace JackHank.EditorUtils.Toolbar
{
    public static class ToolbarLogic
    {
        public static void PlayHere()
        {
            EditorApplication.isPlaying = true;
        }
    }
}