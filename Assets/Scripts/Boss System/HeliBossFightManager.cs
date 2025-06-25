using System.Collections;
using UnityEngine;

public class HeliBossFightManager : BossFightManager
{
    private PlayerVehicle player;
    public int initAmmoCount = 12;

    protected override void AfterEndCutscene()
    {
    }

    protected override void HandleTriggerEnter()
    {
        Begin();
    }

    protected override void OnBeginInterval()
    {
        player = FindFirstObjectByType<PlayerVehicle>();
        var heliBoss = FindFirstObjectByType<HeliBoss>();

        player.playerTurret.fireTarget = heliBoss;
        PlayerTurret.ammo = initAmmoCount;
        player.playerTurret.AllowFire();
    }

    protected override void OnBossDeathInterval()
    {
        player.playerTurret.fireTarget = null;
    }

    protected override void OnRestartInterval()
    {
    }

    protected override IEnumerator PrepareCo()
    {
        yield return null;
    }
}