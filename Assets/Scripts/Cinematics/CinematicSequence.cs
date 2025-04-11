using UnityEngine;

[CreateAssetMenu(fileName = "Cinematic Sequence", menuName = "Jack&Hank/Cinematic Sequence", order = 1)]
public class CinematicSequence : ScriptableObject
{
    public float duration = -1f;
    public GameObject prefab;
    public AudioClip audio;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!prefab) return;
        Animation animation;
        if (prefab.TryGetComponent(out animation))
        {
            duration = animation.clip.length;
        }  
    }
#endif

    /// <summary>
    /// Describes camera state at specific frame
    /// </summary>
    public struct CameraFrameState
    {
        /// <summary>
        /// World position of camera in scene
        /// </summary>
        public Vector3 worldPosition;
        /// <summary>
        /// Rotation of camera in scene
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// Field of view of camera
        /// </summary>
        public float fieldOfView;
    }
}