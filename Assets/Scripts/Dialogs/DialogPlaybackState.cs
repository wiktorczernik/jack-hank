using System;
using System.Collections.Generic;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Stan odtwarzania dialogu
    /// </summary>
    [Serializable]
    public class DialogPlaybackState
    {
        /// <summary>
        /// Referencja do dialogu ktуry jest odtwarzany
        /// </summary>
        public Dialog dialog;
        /// <summary>
        /// Kiedy rozpocz№і siк dialog
        /// </summary>
        public float startTime;
        /// <summary>
        /// Ile czasu całkowitego dialogu juї zostaіo odegrane
        /// </summary>
        public float playedTime;
        /// <summary>
        /// Aktualnie pokazana transkrypcja
        /// </summary>
        public VoicelineTranscription currentTranscription;
        /// <summary>
        /// Lista transkrypcji które już zostały pokazane
        /// </summary>
        public List<VoicelineTranscription> shownTranscriptions;
        /// <summary>
        /// Transkrypcje oczekujące odtworzenia
        /// </summary>
        public Queue<VoicelineTranscription> pendingTranscriptions;
    }
}
