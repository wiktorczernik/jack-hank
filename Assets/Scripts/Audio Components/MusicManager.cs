using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music Events")]
    [SerializeField]
    protected EventReference runMusic;
    [SerializeField]
    protected EventReference bossMusic;

    EventInstance runEventInstance;
    EventInstance bossEventInstance;


    void Awake()
    {
        runEventInstance = RuntimeManager.CreateInstance(runMusic);
        bossEventInstance = RuntimeManager.CreateInstance(bossMusic);
        GameManager.OnRunBegin += OnRunBegin;
        GameManager.OnRunFinish += OnRunEnd;
    }
    private void Start()
    {
        GameManager.Local.bossFightManager.OnBegin += OnBossBegin;
        GameManager.Local.bossFightManager.OnRestart += StopMusic;
        GameManager.Local.bossFightManager.OnPrepareBegin += StopMusic;
        GameManager.OnDeath += StopMusic;
        GameManager.Local.bossFightManager.OnEnd += StopMusic;
    }

    private void OnDestroy()
    {
        GameManager.OnRunBegin -= OnRunBegin;
        GameManager.OnRunFinish -= OnRunEnd;
        GameManager.Local.bossFightManager.OnBegin -= OnBossBegin;
        GameManager.Local.bossFightManager.OnRestart -= StopMusic;
        GameManager.Local.bossFightManager.OnPrepareBegin -= StopMusic;
        GameManager.OnDeath -= StopMusic;
        GameManager.Local.bossFightManager.OnEnd -= StopMusic;
        runEventInstance.release();
        bossEventInstance.release();
    }

    void OnBossBegin()
    {
        runEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bossEventInstance.start();
    }
    void OnRunBegin(GameRunInfo runInfo)
    {
        runEventInstance.start();
    }
    void OnRunEnd(GameRunInfo runInfo)
    {
        runEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    void OnBossEnd()
    {
        bossEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    void StopMusic()
    {
        runEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bossEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
