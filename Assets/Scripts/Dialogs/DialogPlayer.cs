using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Zarz¹dza odtwarzaniem dialogów
    /// </summary>
    public class DialogPlayer : MonoBehaviour
    {
        /// <summary>
        /// Stan odtwarzania dialogu
        /// </summary>
        public static DialogPlaybackState playbackState { get; internal set; }

        /// <summary>
        /// Wywo³ywane gdy odtwarzanie dialogu rozpoczyna siê
        /// </summary>
        public static event Action<DialogPlaybackState> onDialogBegin;
        /// <summary>
        /// Wywo³ywane gyd odtwarzanie dialogu koñczy siê
        /// </summary>
        public static event Action<DialogPlaybackState> onDialogEnd;
        public static event Action<VoicelineTranscription> onTranscriptionBegin;
        public static event Action<VoicelineTranscription> onTranscriptionEnd;

        private static DialogPlayer _instance;
        [Header("State")]
        [SerializeField] bool _isPlaying;

        EventInstance audioEventInstance;


        /// <summary>
        /// Prosi DialogPlayer o odtworzenie dialogu
        /// </summary>
        /// <param name="dialog">Dialog do odtwarzania</param>
        public static void Request(Dialog dialog)
        {
            playbackState = new DialogPlaybackState(dialog);
            var i = _instance;
            i.StartCoroutine(i.DialogPlaybackSequence());
        }

        private void Awake()
        {
            _instance = this;
        }
        private void Update()
        {
            _isPlaying = playbackState != null;

            var s = playbackState;

            if (s == null) return;
            if (s.playedAudio) return;

            s.playedTime += Time.deltaTime;

            PLAYBACK_STATE audioPlaybackState;
            audioEventInstance.getPlaybackState(out audioPlaybackState);
            s.playedAudio = audioPlaybackState == PLAYBACK_STATE.STOPPED;
        }

        IEnumerator DialogPlaybackSequence()
        {
            var state = playbackState;

            audioEventInstance = RuntimeManager.CreateInstance(state.dialog.audioEvent);
            audioEventInstance.start();
            onDialogBegin?.Invoke(state);

            VoicelineTranscription transcription;
            while (playbackState.pendingTranscriptions.TryPeek(out transcription))
            {
                float delay = transcription.startDelay;
                float duration = transcription.duration;

                // Starting delay for transcription
                yield return new WaitForSeconds(delay);

                state.currentTranscription = transcription;
                state.pendingTranscriptions.Dequeue();
                onTranscriptionBegin?.Invoke(transcription);

                // Wait until transcription finishes
                yield return new WaitForSeconds(duration);

                onTranscriptionEnd?.Invoke(transcription);
                state.shownTranscriptions.Add(transcription);
                state.currentTranscription = null;
            }
            state.playedTranscriptions = true;

            yield return new WaitUntil(() => state.playedTranscriptions && state.playedAudio);

            onDialogEnd?.Invoke(state);

            audioEventInstance.release();
            playbackState = null;
        }
    }
}
