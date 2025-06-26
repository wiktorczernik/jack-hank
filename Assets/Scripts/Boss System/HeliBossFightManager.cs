using System.Collections;
using UnityEngine;

public class HeliBossFightManager : BossFightManager
{
    private PlayerVehicle player;
    public int initAmmoCount = 64;
    public float initSpeedInKmH = 50;
    public GameObject obstacleToActivateAfterBeginCutscene;

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
        player.physics.bodyRigidbody.linearVelocity = playerSpawnAfterBeginCutscene.forward * (initSpeedInKmH / 2);
        obstacleToActivateAfterBeginCutscene.SetActive(true);
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