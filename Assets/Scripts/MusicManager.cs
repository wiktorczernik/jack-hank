using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Run Music")]
    public AudioClip runMusicMax;
    public AudioClip runMusicMin;

    [Header("Boss Music")]
    public AudioClip bossMusic;

    [Header("Components")]
    public AudioSource audioSource;


    void Awake()
    {
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

    void OnBossBegin()
    {
        audioSource.clip = bossMusic;
        audioSource.Play();
    }
    void OnRunBegin(GameRunInfo runInfo)
    {
        audioSource.clip = runMusicMax;
        audioSource.Play();
    }
    void OnRunEnd(GameRunInfo runInfo)
    {
        StopMusic();
    }
    void OnBossEnd()
    {
        StopMusic();
    }

    void StopMusic()
    {
        audioSource.Stop();
    }
}
