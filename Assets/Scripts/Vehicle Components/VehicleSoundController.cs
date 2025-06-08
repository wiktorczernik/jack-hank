using UnityEngine;

using JackHank.Cinematics;


public class VehicleSoundController : MonoBehaviour
{
    public VehiclePhysics vehicleController;
    public AnimationCurve engineVolumeCurve;
    public float driftVolumeLerp = 3f;
    public float driftRealVolume = 0.75f;
    public float driftMinNormalVolume = 0.3f;
    public float driftMinPitch = 0.7f;
    public float driftGainPitch = 0.3f;
    [Header("Environment Bump")]
    public float bumpMinPitch = 0.5f;
    public float bumpMaxPitch = 1.5f;

    public AudioSource driftSoundSource;
    public AudioSource engineSoundSource;
    public AudioSource bumpSoundSource;

    public void PlayEnvironmentBump()
    {
        if (bumpSoundSource.isPlaying) return;
        bumpSoundSource.pitch = Random.Range(bumpMinPitch, bumpMaxPitch);
        bumpSoundSource.Play();
    }

    private void Update()
    {
        float driftFactor = Mathf.Clamp01(Mathf.Abs(vehicleController.driftAngular / vehicleController.driftMaxAngular));

        float targetDriftVolume;
        if (!vehicleController.isDrifting)
        {
            targetDriftVolume = 0;
        }
        else
        {
            targetDriftVolume = driftMinNormalVolume;
            targetDriftVolume += driftFactor * (1 - driftMinNormalVolume);
        }

        float currentDriftVolume = Mathf.Lerp(driftSoundSource.volume, targetDriftVolume, driftVolumeLerp * Time.deltaTime);
        currentDriftVolume *= driftRealVolume * Mathf.Clamp01(Time.timeScale);
        
        float currentDriftPitch = driftMinPitch; 
        currentDriftPitch += Time.timeScale * driftFactor * driftGainPitch;

        float engineFactor = 1 + Mathf.Clamp01(vehicleController.speedKmhForward / 150f) * 2.5f * Time.timeScale;
        
        driftSoundSource.volume = currentDriftVolume;
        driftSoundSource.pitch = currentDriftPitch;
        
        engineSoundSource.pitch = engineFactor;
        engineSoundSource.volume = Mathf.Clamp01(Time.timeScale);
    }

    private void OnEnable()
    {
        CinematicPlayer.onBeginPlay += OnCinematicBegin;
        CinematicPlayer.onEndPlay += OnCinematicEnd;
    }
    private void OnDisable()
    {
        CinematicPlayer.onBeginPlay -= OnCinematicBegin;
        CinematicPlayer.onEndPlay -= OnCinematicEnd;
    }

    private void OnCinematicBegin()
    {
        engineSoundSource.enabled = false;
        driftSoundSource.enabled = false;
    }
    private void OnCinematicEnd()
    {
        engineSoundSource.enabled = true;
        driftSoundSource.enabled = true;
    }
}
