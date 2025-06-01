using System;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Ustawienia odtwarzania voiceline'u podczas dialogu
    /// </summary>
    [Serializable]
    public class VoicelinePlaybackConfig
    {
        /// <summary>
        /// Referencja do voiceline'u którego ten config dotyczy
        /// </summary>
        public Voiceline line;
        /// <summary>
        /// Ile sekund po rozpoczêciu dialogu powinno zagraæ
        /// </summary>
        public float startTime;
    }
}
