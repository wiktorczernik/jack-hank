using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOfPavement : Zone
{
    [SerializeField] private TrackVerifer bus;

    private void Update()
    {
        if (IsPlayerHere & !bus.isOnPavement)
        {
            bus.startMoving = bus.transform.position;
            bus.isOnPavement = true;
        }
    }
}
