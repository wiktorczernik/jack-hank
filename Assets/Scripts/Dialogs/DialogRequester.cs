using UnityEngine;
using UnityEngine.Events;

namespace JackHank.Dialogs
{
    public class DialogRequester : MonoBehaviour
    {
        [Header("State")]
        public bool requested = false;
        [Header("Settings")]
        public Dialog dialog;
        public bool playOnce = false;

        [HideInInspector] string _saveKey;

        private void Start()
        {
            _saveKey = dialog.GetInstanceID().ToString();
            if (playOnce)
                requested = PlayerPrefs.HasKey(_saveKey);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other) return;
        }
        public void Request()
        {
            if (requested) return;
            requested = true;
            PlayerPrefs.SetInt(dialog.GetInstanceID().ToString(), 1);
            DialogPlayer.Request(dialog);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteKey(_saveKey);
        }
    }
}
