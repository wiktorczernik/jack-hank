using System;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Transkrypcja voiceline'u. Nie musi byж dla caіego nagrania, moїe byж czкњci№
    /// </summary>
    [Serializable]
    public class VoicelineTranscription
    {
        /// <summary>
        /// Postać która mówi ten wers
        /// </summary>
        public Character character;
        /// <summary>
        /// Zawartoњж transkrypcji
        /// </summary>
        public string text;
        /// <summary>
        /// Dіugoњж transkrypcji w sekundach
        /// </summary>
        public float duration;
        /// <summary>
        /// Opуџnienie pocz№tku transkrypcji (na potrzeby voiceline'u)
        /// </summary>
        public float startDelay;
    }
}
