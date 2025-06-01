using System;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Stan odtwarzania voiceline'u
    /// </summary>
    [Serializable]
    public class VoicelinePlaybackState
    {
        /// <summary>
        /// Referencja do voiceline'u którego ten state dotyczy
        /// </summary>
        public Voiceline line;
        /// <summary>
        /// Referencja do ustawieñ odtwarzania tego voiceline'u
        /// </summary>
        public VoicelinePlaybackConfig playbackConfig;
        /// <summary>
        /// Na której sekundzie clip'u jest aktualnia odtworzenie
        /// </summary>
        public float timePlayed;
        /// <summary>
        /// Czy zosta³o odtworzone ca³kowicie?
        /// </summary>
        public bool wasPlayed;
        /// <summary>
        /// Czy aktualnie ten voiceline jest odtwarzany?
        /// </summary>
        public bool isPlaying;
    }
}
