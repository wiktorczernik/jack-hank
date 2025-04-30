using UnityEngine;

using JHCinematics;


public class VehicleSoundController : MonoBehaviour
{
    public VehiclePhysics vehicleController;
    public AnimationCurve driftVolumeCurve;
    public AnimationCurve driftPitchCurve;
    public AnimationCurve engineVolumeCurve;
    public float driftVolumeLerp = 3f;

    public AudioSource driftSoundSource;
    public AudioSource engineSoundSource;

    private void Update()
    {
        float driftFactor = Mathf.Abs(vehicleController.bodyRigidbody.angularVelocity.y);
        driftFactor = Mathf.Clamp01(driftFactor - 0.8f);
        float engineFactor = 1 + Mathf.Clamp01(vehicleController.speedKmhForward / 150f) * 2.5f;
        driftSoundSource.volume = Mathf.Lerp(driftSoundSource.volume, driftVolumeCurve.Evaluate(driftFactor), driftVolumeLerp * Time.deltaTime);
        driftSoundSource.pitch = driftPitchCurve.Evaluate(driftFactor);
        engineSoundSource.pitch = engineFactor;
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
