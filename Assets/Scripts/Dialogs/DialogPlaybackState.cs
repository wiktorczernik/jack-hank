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
        /// Referencja do dialogu który jest odtwarzany
        /// </summary>
        public Dialog dialog;
        /// <summary>
        /// Kiedy rozpocz¹³ siê dialog
        /// </summary>
        public float startTime;
        /// <summary>
        /// Ile czasu ca³kowitego dialogu ju¿ zosta³o odegrane
        /// </summary>
        public float playedTime;
        /// <summary>
        /// Stan odtwarzania pojedyñczych voiceline'ów
        /// </summary>
        public List<VoicelinePlaybackState> lineStates;
    }
}
