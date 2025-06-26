using UnityEngine;

using JackHank.Cinematics;
using FMOD.Studio;
using FMODUnity;
using System;


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

    [Header("Audio")]
    [SerializeField]
    EventReference engineEventRef;
    [SerializeField]
    EventReference driftEventRef;
    [SerializeField]
    EventReference bumpEventRef;

    EventInstance engineEventInstance;
    EventInstance driftEventInstance;


    public void PlayEnvironmentBump(Collision collision)
    {
        Debug.Log("Bump!");
        RuntimeManager.PlayOneShot(bumpEventRef);
    }

    private void Update()
    {
        if (CinematicPlayer.isPlaying) return;

        float driftFactor = vehicleController.driftAngular / vehicleController.driftMaxAngular;
        driftFactor = Mathf.Abs(driftFactor);
        driftFactor = Mathf.Clamp01(driftFactor);

        float speedFactor = vehicleController.speedKmhForward / 150f;
        speedFactor = Mathf.Clamp01(speedFactor);

        engineEventInstance.setParameterByName("Velocity", speedFactor);

        driftEventInstance.setParameterByName("Drift_Intensity", driftFactor);
    }

    private void Awake()
    {
        engineEventInstance = RuntimeManager.CreateInstance(engineEventRef);
        driftEventInstance = RuntimeManager.CreateInstance(driftEventRef);
    }
    private void OnEnable()
    {
        engineEventInstance.start();

        CinematicPlayer.onBeginPlay += OnCinematicBegin;
        CinematicPlayer.onEndPlay += OnCinematicEnd;
        vehicleController.onDriftBegin += OnDriftBegin;
        vehicleController.onDriftEnd += OnDriftEnd;
        vehicleController.onEnvironmentBump.AddListener(PlayEnvironmentBump);
    }
    private void OnDisable()
    {
        CinematicPlayer.onBeginPlay -= OnCinematicBegin;
        CinematicPlayer.onEndPlay -= OnCinematicEnd;
        vehicleController.onDriftBegin -= OnDriftBegin;
        vehicleController.onDriftEnd -= OnDriftEnd;
        vehicleController.onEnvironmentBump.RemoveListener(PlayEnvironmentBump);
    }
    private void OnDestroy()
    {
        driftEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        driftEventInstance.release();
        engineEventInstance.release();
    }

    private void OnDriftBegin()
    {
        Debug.Log("Drift Begin");
        driftEventInstance.start();
    }
    private void OnDriftEnd()
    {
        Debug.Log("Drift End");
        driftEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void OnCinematicBegin()
    {
        driftEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        engineEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    private void OnCinematicEnd()
    {
        engineEventInstance.start();
    }
}
