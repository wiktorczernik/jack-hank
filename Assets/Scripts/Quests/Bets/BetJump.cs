using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetJump : Bet
{
    [SerializeField] private TargetRamp ramp;
    [SerializeField] private Transform busPositionGlobal;

    private void Start()
    {
        ramp.busPosition = busPositionGlobal;
    }

    private void Update()
    {
        if(ramp.IsCompleted & !IsCompleted)
        {
            IsCompleted = true;
        }
    }
}
