using System;
using System.Collections;
using JackHank.Dialogs;
using UnityEngine;

namespace JackHank.Cinematics
{
    public class CinematicPlayer : MonoBehaviour
    {
        /// <summary>
        /// Determines if every sequence will be automatically skipped
        /// </summary>
        public static bool autoSkip { get; set; }
        /// <summary>
        /// Tells if some sequence is being played
        /// </summary>
        public static bool isPlaying
        {
            get
            {
                if (!_instance) return false;
                return _instance._isPlaying;
            }
            private set
            {
                if (!_instance) return;
                _instance._isPlaying = value;
            }
        }
        /// <summary>
        /// Sequence that is being played at the momemnt
        /// </summary>
        public static CinematicSequence playedSequence
        {
            get
            {
                if (!_instance) return null;
                return _instance._playedSequence;
            }
            private set
            {
                if (!_instance) return;
                _instance._playedSequence = value;
            }
        }

        /// <summary>
        /// Called each frame of played cinematic sequence
        /// </summary>
        public static Action<CinematicSequence.CameraFrameState> onFrameUpdate;
        /// <summary>
        /// Called when started playing cinematic sequence
        /// </summary>
        public static Action onBeginPlay;
        /// <summary>
        /// Called after cinematic sequence ended
        /// </summary>
        public static Action onEndPlay;

        static CinematicPlayer _instance;

        [SerializeField] bool _isPlaying;
        [SerializeField] CinematicSequence _playedSequence;

        private AudioSource _audioSource;


        private void Awake()
        {
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays desired cinematic sequence
        /// </summary>
        /// <param name="worldPos">Position of spawned prefab</param>
        /// <param name="rotation">Rotation of spawned prefab</param>
        /// <param name="scale">Scale of spawned prefab</param>
        /// <param name="parent">Parent of spawned prefab</param>
        /// <returns>True if started playing, otherwise false</returns>
        public static bool PlaySequence(CinematicSequence sequence, Vector3 worldPos, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            if (isPlaying) return false;
            if (!_instance) return false;
            _instance.StartCoroutine(PlaySequenceCo(sequence, worldPos, rotation, scale, parent));
            return true;
        }
        /// <summary>
        /// Plays desired cinematic sequence
        /// </summary>
        /// <param name="worldPos">Position of spawned prefab</param>
        /// <param name="rotation">Rotation of spawned prefab</param>
        /// <param name="scale">Scale of spawned prefab</param>
        /// <param name="parent">Parent of spawned prefab</param>
        public static IEnumerator PlaySequenceCo(CinematicSequence sequence, Vector3 worldPos, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            if (!_instance)
            {
                Debug.LogError("Tried to play cinematic sequence when singleton is null!");
                yield break;
            }
            if (isPlaying)
            {
                Debug.LogError("Tried to play cinematic sequence when it's already playing.", _instance);
                yield break;
            }
            if (!sequence)
            {
                Debug.LogError("Cinematic sequence is null!", _instance);
                yield break;
            }
            if (sequence.duration < 0)
            {
                Debug.LogError("Cinematic sequence duration is invalid. Maybe Animator is missing?", sequence);
                yield break;
            }
            if (!sequence.prefab)
            {
                Debug.LogError("Tried to play cinematic sequence that has no prefab set.", sequence);
                yield break;
            }

            GameObject instance = Instantiate(sequence.prefab, worldPos, rotation);
            instance.transform.localScale = scale;
            if (parent)
                instance.transform.SetParent(parent);

            isPlaying = true;
            playedSequence = sequence;
            onBeginPlay?.Invoke();

            var audioSource = _instance._audioSource;
            if (audioSource && sequence.audio)
            {
                audioSource.Stop();
                audioSource.clip = sequence.audio;
                audioSource.Play();
            }
            if (sequence.dialog)
            {
                DialogPlayer.Request(sequence.dialog);
            }

            Transform cameraAnchor = instance.transform.GetChild(0);

            if (!autoSkip)
            {
                float timePassed = 0f;
                while (timePassed <= sequence.duration)
                {
                    CinematicSequence.CameraFrameState tickState = new();
                    tickState.worldPosition = cameraAnchor.position;
                    tickState.rotation = cameraAnchor.rotation;
                    timePassed += Time.deltaTime;
                    onFrameUpdate?.Invoke(tickState);
                    yield return null;
                }
                CinematicSequence.CameraFrameState lastState = new();
                lastState.worldPosition = cameraAnchor.position;
                lastState.rotation = cameraAnchor.rotation;
                onFrameUpdate?.Invoke(lastState);
            }


            isPlaying = false;
            playedSequence = null;
            onEndPlay?.Invoke();

            Destroy(instance);
        }
    }
}