using Cinemachine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Components")]
    public CinemachineBrain brain;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineBasicMultiChannelPerlin perlinNoise;
    public CinemachineTransposer transposer;
    public CinemachineComposer composer;
    [Header("Player")]
    public PlayerVehicle playerVehicle;

    [Header("State")]
    public bool duringCinematic = false;
    public float shakeIntensity = 0f;
    public float speedTargetFov = 0f;

    [Header("Settings")]
    public float speedFovLerp = 1.5f;
    public float speedMinFov = 55f;
    public float speedMaxFov = 70f;
    public float effectsMinSpeed = 70f;
    public float effectsMaxSpeed = 140f;
    public float shakeMaxAmplitude = 50f;
    public float shakeRiseRate = 4f;
    public float shakeFallRate = 0.5f;
    public float shakeFrequency = 0.05f;

    [Header("Effects")]
    [SerializeField] Material blurMaterial;
    [SerializeField] ParticleSystem SpeedLines;
    [SerializeField] AnimationCurve fovCurve;

    public void Shake(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);

        if (intensity > shakeIntensity)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeRise(intensity));
        }
    }

    IEnumerator ShakeRise(float targetIntensity)
    {
        while (shakeIntensity < targetIntensity - float.Epsilon)
        {
            shakeIntensity = Mathf.Clamp01(shakeIntensity + shakeRiseRate * Time.deltaTime);
            perlinNoise.m_AmplitudeGain = shakeMaxAmplitude * shakeIntensity;
            perlinNoise.m_FrequencyGain = shakeFrequency;
            yield return new WaitForEndOfFrame();
        }
        yield return ShakeFall();
    }
    IEnumerator ShakeFall()
    {
        while (shakeIntensity > 0)
        {
            shakeIntensity = Mathf.Clamp01(shakeIntensity - shakeFallRate * Time.deltaTime);
            perlinNoise.m_AmplitudeGain = shakeMaxAmplitude * shakeIntensity;
            perlinNoise.m_FrequencyGain = shakeFrequency;
            yield return new WaitForEndOfFrame();
        }
    }
    
    private void OnExplosionNearby(ExplosionProperties properties)
    {
        Shake(properties.shakeIntensity);
    }


    private void OnCinematicBegin()
    {
        duringCinematic = true;
        composer.enabled = false;
        transposer.enabled = false;
    }
    private void OnCinematicFrameUpdate(CinematicSequence.CameraFrameState frameState)
    {
        virtualCamera.ForceCameraPosition(frameState.worldPosition, frameState.rotation);
        transposer.ForceCameraPosition(frameState.worldPosition, frameState.rotation);
        composer.ForceCameraPosition(frameState.worldPosition, frameState.rotation);
    }
    private void OnCinematicEnd()
    {
        duringCinematic = false;
        composer.enabled = true;
        transposer.enabled = true;
    }

    float CurveEasing(float x, float start, float finish)
    {
        return 1 / (1 + Mathf.Exp(10 / (start - finish) * (x - (start + finish) / 2)));
    }
    private void ManageSpeedFov()
    {
        float currentFov = virtualCamera.m_Lens.FieldOfView;
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(currentFov, speedTargetFov, speedFovLerp * Time.deltaTime);
    }
    private void ApplySpeedEffects()
    {
        float currentFov = virtualCamera.m_Lens.FieldOfView;
        float speed = playerVehicle.physics.GetForwardSpeed() * 3.6f;
        float minSpeed = effectsMinSpeed, maxSpeed = effectsMaxSpeed;

        if (speed < minSpeed) {
            var s_ = SpeedLines.shape;
            s_.radius = 100f;
            speedTargetFov = 55;
            blurMaterial.SetFloat("_Level", 0);
            return;
        }

        float lvl = CurveEasing(speed, minSpeed, maxSpeed);

        var main = SpeedLines.main;
        main.startSpeed = Mathf.Lerp(8, 20, lvl);
        var emmision = SpeedLines.emission;
        emmision.rateOverTime = Mathf.Lerp(75, 120, lvl);
        var shape = SpeedLines.shape;
        shape.radius = Mathf.Lerp(11, 8, lvl);

        blurMaterial.SetFloat("_Level", lvl);

        speedTargetFov = speedMinFov + (speedMaxFov - speedMinFov) * fovCurve.Evaluate(Mathf.Min(Mathf.Max(speed - minSpeed, 0) / (maxSpeed - minSpeed), 1f));
    }

    private void OnEnable()
    {
        speedTargetFov = speedMinFov;
        playerVehicle.onExplosionNearby.AddListener(OnExplosionNearby);
        CinematicPlayer.onBeginPlay += OnCinematicBegin;
        CinematicPlayer.onFrameUpdate += OnCinematicFrameUpdate;
        CinematicPlayer.onEndPlay += OnCinematicEnd;
    }
    private void OnDisable()
    {
        playerVehicle.onExplosionNearby.RemoveListener(OnExplosionNearby);
        CinematicPlayer.onBeginPlay -= OnCinematicBegin;
        CinematicPlayer.onFrameUpdate -= OnCinematicFrameUpdate;
        CinematicPlayer.onEndPlay -= OnCinematicEnd;
    }

    private void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    }
    private void Update()
    {
        ApplySpeedEffects();
        ManageSpeedFov();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
#endif
}