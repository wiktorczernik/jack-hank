using System;
using System.Collections;
using UnityEngine;

namespace JackHank.Cinematics
{
    public class CinematicPlayer : MonoBehaviour
    {
        /// <summary>
        /// Determines if every sequence will be automatically skipped
        /// </summary>
        public static bool autoSkip { get; set; } = false;
        /// <summary>
        /// Tells if some sequence is being played
        /// </summary>
        public static bool isPlaying { get; private set; }
        /// <summary>
        /// Sequence that is being played at the momemnt
        /// </summary>
        public static CinematicSequence playedSequence { get; private set; }

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


        private void Awake()
        {
            _instance = this;
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

            Animation animation = instance.GetComponent<Animation>();
            animation.Play();

            Transform cameraAnchor = instance.transform.GetChild(0);

            isPlaying = true;
            playedSequence = sequence;
            onBeginPlay?.Invoke();

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
            else
            {
                yield return null;
            }


                isPlaying = false;
            playedSequence = null;
            onEndPlay?.Invoke();

            Destroy(instance);
        }
    }
}