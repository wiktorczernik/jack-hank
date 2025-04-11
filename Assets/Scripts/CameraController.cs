using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineBasicMultiChannelPerlin perlinNoise;
    public PlayerVehicle playerVehicle;

    [Header("State")]
    public bool duringCinematic = false;
    public float shakeIntensity = 0f;

    [Header("Settings")]
    public float shakeMaxAmplitude = 50f;
    public float shakeRiseRate = 4f;
    public float shakeFallRate = 0.5f;
    public float shakeFrequency = 0.05f;

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
    }
    private void OnCinematicFrameUpdate(CinematicSequence.CameraFrameState frameState)
    {
        virtualCamera.ForceCameraPosition(frameState.worldPosition, frameState.rotation);
    }
    private void OnCinematicEnd()
    {
        duringCinematic = false;
    }

    private void OnEnable()
    {
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
#endif
}