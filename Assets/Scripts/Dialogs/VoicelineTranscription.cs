using System;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Transkrypcja voiceline'u. Nie musi byæ dla ca³ego nagrania, mo¿e byæ czêœci¹
    /// </summary>
    [Serializable]
    public class VoicelineTranscription
    {
        /// <summary>
        /// Zawartoœæ transkrypcji
        /// </summary>
        public string text;
        /// <summary>
        /// D³ugoœæ transkrypcji w sekundach
        /// </summary>
        public float duration;
        /// <summary>
        /// OpóŸnienie pocz¹tku transkrypcji (na potrzeby voiceline'u)
        /// </summary>
        public float startDelay;
    }
}
