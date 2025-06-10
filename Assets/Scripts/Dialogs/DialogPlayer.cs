using System;
using System.Collections;
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
        [Header("Components")]
        [SerializeField] AudioSource _audioSource;


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

            if (_audioSource.clip == null) return;

            var s = playbackState;
            _audioSource.pitch = Time.timeScale;
            _audioSource.volume = Mathf.Clamp01(Time.timeScale);
            if (Time.timeScale < 0.1f)
                _audioSource.Pause();
            else
                _audioSource.UnPause();

            if (s == null) return;
            if (s.playedAudio) return;

            s.playedTime += Time.deltaTime;
            if (s.playedTime >= s.dialog.audioDuration)
            {
                s.playedTime = s.dialog.audioDuration;
                s.playedAudio = true;
                _audioSource.Stop();
                _audioSource.clip = null;
            }
        }

        IEnumerator DialogPlaybackSequence()
        {
            var audioSource = _instance._audioSource;
            var state = playbackState;

            audioSource.Stop();
            audioSource.clip = playbackState.dialog.audioClip;
            audioSource.Play();

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

            audioSource.clip = null;
            playbackState = null;
        }
    }
}
