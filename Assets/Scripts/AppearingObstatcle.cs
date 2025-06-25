using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;


public class AppearingObstatcle : MonoBehaviour
{
    [Header("How obstacles appear")]
    [SerializeField] private ObstacleAppearType obstacleAppearType = ObstacleAppearType.WithEffects;
    [SerializeField] private GameObject objectToJustAppear;

    [Header("Aminating")]
    [SerializeField] private Animation usedAnimation;
    [SerializeField] private string idleAnimationName = "idle";
    [SerializeField] private string actionAnimationName = "action";

    [Header("Particles after and before action animation")]
    [SerializeField] private ParticleSystem[] particlesPrefabsBefore;
    [SerializeField] private Transform particlesTransformBefore;
    [SerializeField] private ParticleSystem[] particlesPrefabsAfter;
    [SerializeField] private Transform particlesTransformAfter;

    [Header("Shake after and before action animation")]
    [SerializeField] private bool shakeBefore;
    [SerializeField] private float shakeIntencityBefore = 0.6f;
    [SerializeField] private bool shakeAfter;
    [SerializeField] private float shakeIntencityAfter = 0.6f;

    [Header("Other")]
    [SerializeField] private TriggerEventEmitter[] appearTriggers;

    private CameraController cameraController;
    private bool wasTriggered;

    private void Start()
    {
        foreach (var trigger in appearTriggers)
        {
            trigger.OnEnter.AddListener(OnTiggerEnter);
        }

        if (obstacleAppearType == ObstacleAppearType.JustAppear)
        {
            if (objectToJustAppear == null)
            { 
                Debug.LogError($"AppearingObstacle({name}): selected appearing type is JustAppear but no objctToJustAppear");
                objectToJustAppear.SetActive(false);
            }
        } else if (obstacleAppearType == ObstacleAppearType.WithEffects)
        {
            cameraController = FindAnyObjectByType<CameraController>();

            if ((shakeAfter || shakeBefore) && cameraController == null)
                Debug.LogError($"AppearingObstacle({name}): no camera controller in active scene");

            if (particlesPrefabsBefore.Length > 0 && particlesTransformBefore == null)
                Debug.LogError($"AppearingObstacle({name}): no transform for particels that play before animation");

            if (particlesPrefabsAfter.Length > 0 && particlesTransformAfter == null)
                Debug.LogError($"AppearingObstacle({name}): no transform for particles that play after animation");   

            if (usedAnimation.GetClip(actionAnimationName) == null)
                Debug.LogError($"AppearingObstacle({name}): no action animation with name '{actionAnimationName}'");

            if (usedAnimation.GetClip(idleAnimationName) == null) return;

            usedAnimation.wrapMode = WrapMode.Loop;
            usedAnimation.Play(idleAnimationName);
        }
    }

    private void OnTiggerEnter(Collider col)
    {
        if (!col.CompareTag("Player") || wasTriggered) return;
        wasTriggered = true;

        if (obstacleAppearType == ObstacleAppearType.JustAppear)
            objectToJustAppear.SetActive(true);
        else if (obstacleAppearType == ObstacleAppearType.WithEffects)
            StartCoroutine(AppearCo());
    }

    private IEnumerator AppearCo()
    {
        yield return PlayParticlesCo(particlesPrefabsBefore, particlesTransformBefore);

        if (shakeBefore) cameraController.Shake(shakeIntencityBefore);

        usedAnimation.wrapMode = WrapMode.Once;
        usedAnimation.Play(actionAnimationName);

        yield return new WaitWhile(() => usedAnimation.isPlaying);

       StartCoroutine(PlayParticlesCo(particlesPrefabsAfter, particlesTransformAfter));

        if (shakeAfter) cameraController.Shake(shakeIntencityAfter);
    }

    private IEnumerator PlayParticlesCo(ParticleSystem[] particlesPrefabs, Transform trans)
    {
        var timeToCheck = 0f;
        foreach (var particlesPrefab in particlesPrefabs)
        {
            var particles = Instantiate(particlesPrefab, transform);
            particles.transform.SetPositionAndRotation(trans.position, trans.rotation);
            particles.transform.localScale = trans.localScale;

            particles.Play();

            if (particles.main.duration > timeToCheck) timeToCheck = particles.main.duration;
        }

        yield return new WaitForSeconds(timeToCheck);
    }

    enum ObstacleAppearType
    {
        WithEffects,
        JustAppear,
    }
}
