using System;
using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Zarz¹dza odtwarzaniem dialogów
    /// </summary>
    public class DialogPlayer : MonoBehaviour
    {
        /// <summary>
        /// Stan odtwarzania dialogu
        /// </summary>
        public static DialogPlaybackState playbackState { get; internal set; }

        /// <summary>
        /// Wywo³ywane gdy odtwarzanie dialogu rozpoczyna siê
        /// </summary>
        public static event Action<DialogPlaybackState> onDialogBegin;
        /// <summary>
        /// Wywo³ywane gyd odtwarzanie dialogu koñczy siê
        /// </summary>
        public static event Action<DialogPlaybackState> onDialogEnd;


        /// <summary>
        /// Prosi DialogPlayer o odtworzenie dialogu
        /// </summary>
        /// <param name="dialog">Dialog do odtwarzania</param>
        public static void Request(Dialog dialog)
        {

        }
    }
}
