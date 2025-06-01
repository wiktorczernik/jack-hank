using System.Collections.Generic;
using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Reprezentacja voiceline'u dla systemu dialogów
    /// </summary>
    [CreateAssetMenu(fileName = "New Voiceline", menuName = "Jack&Hank/Dialogs/Voiceline", order = 1)]
    public class Voiceline : ScriptableObject
    {
        /// <summary>
        /// Referencja do postaci, która wymawia ten voiceline
        /// </summary>
        public Character character;
        /// <summary>
        /// Referencja do pliku dŸwiêku
        /// </summary>
        public AudioClip audio;
        /// <summary>
        /// D³ugoœæ tego voiceline'u w sekundach
        /// </summary>
        public float duration;
        /// <summary>
        /// Zestaw transkrypcji tego voiceline'u
        /// </summary>
        public List<VoicelineTranscription> transcriptions;
    }
}
