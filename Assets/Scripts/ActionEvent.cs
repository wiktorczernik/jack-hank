using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class ActionEvent : MonoBehaviour
	{
		[SerializeField] private string idleAnimationName = "idle";
		[SerializeField] private string actionAnimationName = "action";
        [SerializeField] private Animation usedAnimation;

        [SerializeField] private int explosionType = 1;
		[SerializeField] private ExplosionProperties explosionSettings;
		[SerializeField] private TriggerEventEmitter actionTrigger;

		[SerializeField] private bool shakeOnEnd;
		[SerializeField] private float shakeIntencity = 0.6f;

		private bool wasTriggered;

        private void Start()
        {
            actionTrigger.OnEnter.AddListener((col) => StartCoroutine(StartAction()));

            if (usedAnimation.GetClip(idleAnimationName) == null) return;

			usedAnimation.wrapMode = WrapMode.Loop;
            usedAnimation.Play(idleAnimationName);
        }

        private IEnumerator StartAction()
		{
			Debug.Log("Action started");
			if (wasTriggered) yield break;
			wasTriggered = true;

			usedAnimation.wrapMode = WrapMode.Once;

            usedAnimation.Play(actionAnimationName);

			yield return new WaitWhile(() => usedAnimation.isPlaying);

            ExplosionMaster.Create(explosionSettings, explosionType);

			if (shakeOnEnd) FindAnyObjectByType<CameraController>()?.Shake(shakeIntencity);
        }
	}
}