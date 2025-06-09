using System;
using System.Collections.Generic;
using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Stan odtwarzania dialogu
    /// </summary>
    [Serializable]
    public class DialogPlaybackState
    {
        public DialogPlaybackState(Dialog dialog) 
        {
            this.dialog = dialog;
            startTime = Time.time;
            playedTime = 0;
            pendingTranscriptions = new();
            foreach (var transcription in dialog.transcriptions)
                pendingTranscriptions.Enqueue(transcription);
            shownTranscriptions = new();
        }

        /// <summary>
        /// Podpowiada czy odtworzono audio całkowicie
        /// </summary>
        public bool playedAudio = false;
        /// <summary>
        /// Podpowiada czy transkrypcje audio całkowicie
        /// </summary>
        public bool playedTranscriptions = false;
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
