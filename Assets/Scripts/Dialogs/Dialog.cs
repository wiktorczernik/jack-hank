using System.Collections.Generic;
using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Reprezentacja dialogu dla systemu dialogów
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialog", menuName = "Jack&Hank/Dialogs/Dialog", order = 1)]
    public class Dialog : ScriptableObject
    {
        /// <summary>
        /// D³ugoœæ dialogu w sekundach
        /// </summary>
        public float duration;
        /// <summary>
        /// Priorytet odtwarzania dialogu
        /// </summary>
        public DialogPriority priority;
        /// <summary>
        /// Voiceline'y któe zostan¹ odtworzone
        /// </summary>
        public List<VoicelinePlaybackConfig> linePlaybacks;
    }
}
