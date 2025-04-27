using System.Collections;
using UnityEngine;

public class HeliBossFightManager : BossFightManager
{
    protected override void AfterEndCutscene()
    {

    }

    protected override void HandleTriggerEnter()
    {
        Begin();
    }

    protected override void OnBeginInterval()
    {

    }

    protected override void OnBossDeathInterval()
    {

    }

    protected override void OnRestartInterval()
    {

    }

    protected override IEnumerator PrepareCo()
    {
        yield return null;
    }
}
