using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameRunInfo runInfo { get; private set; } = null;
    public static bool isDuringRun { get; private set; } = false;


    public static Action<GameRunInfo> OnRunBegin;

    public void BeginRun()
    {
        runInfo = new GameRunInfo();
        isDuringRun = true;
        OnRunBegin?.Invoke(runInfo);
        PrepareLevelEntities();
    }

    private void PrepareLevelEntities()
    {
        SmashableEntity[] smashables = GameObject.FindObjectsByType<SmashableEntity>(FindObjectsSortMode.None);
        foreach (SmashableEntity smashable in smashables)
        {
            smashable.OnHit?.AddListener(OnHitSmashable);
        }
    }

    private void OnHitSmashable(SmashableEntity smashable)
    {
        runInfo.AddBountyPoints(smashable.bountyPointsReward);
    }
    private void GameRunFrameTick()
    {
        runInfo.time += Time.deltaTime;   
    }


    private void Awake()
    {
        BeginRun();
    }
    private void Update()
    {
        if (isDuringRun)
        {
            GameRunFrameTick();
        }
    }
}
