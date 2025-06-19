using System.Linq;
using FMODUnity;
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
        /// Nagranie in the flesh
        /// </summary>
        public EventReference audioEvent;
        /// <summary>
        /// D³ugoœæ dialogu w sekundach
        /// </summary>
        public float audioDuration;
        public float transcriptionDuration;
        /// <summary>
        /// Priorytet odtwarzania dialogu
        /// </summary>
        public DialogPriority priority;
        /// <summary>
        /// Voiceline'y któe zostan¹ odtworzone
        /// </summary>
        public VoicelineTranscription[] transcriptions;

        private void OnValidate()
        {
            if (transcriptions != null)
            {
                transcriptionDuration = transcriptions.Sum(x => x.duration + x.startDelay);
            }
        }
    }
}
