using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetDrivingOn : Bet //by ukonczyc ten zaklad musisz jechac np. po chodniku bez przerwy
{
    [SerializeField] private TrackVerifer track;
    [SerializeField] private List<Zone> zones;
    [SerializeField] private List<Zone> actualZones;
    [SerializeField] private bool IsPlayerOnZone;
    [SerializeField] private float timeRequirement;
    [SerializeField] private float actualTimeInZone;

    void Start()
    {
        IsPlayerOnZone = false;
        actualZones = new List<Zone>();
        actualZones.Clear();
        actualTimeInZone = 0;
        StartCoroutine(CheckZone());
    }

    IEnumerator CheckZone()
    {
        yield return new WaitForSeconds(3);
        actualZones = GetCurrentZones();
        IsPlayerOnZone = (actualZones.Count > 0) ? true : false;
        track.isOnPavement = (IsPlayerOnZone) ? true : false;

        if (IsPlayerOnZone)
        {
            actualTimeInZone += 3;
        }
        else
        {
            track.startMoving = Vector3.zero;
            actualTimeInZone = 0;
        }
        
        if(actualTimeInZone >= timeRequirement)
        {
            IsCompleted = true;
            //track.endMoving = track.transform.position;
            //if(Vector3.Distance(track.startMoving, track.endMoving) > 2f) //tutaj ustawiam jaki minimalny dystans trzeba pokonaæ by uznaæ zaklad
            //{
            //    IsCompleted = true;
            //}
            //else
            //{
            //    track.startMoving = track.transform.position;
            //    track.endMoving = Vector3.zero;
            //    StartCoroutine(CheckZone());
            //}
        }
        else
            StartCoroutine(CheckZone());
    }

    public List<Zone> GetCurrentZones()
    {
        return zones.Where(z => z.IsPlayerHere).ToList();
    }
}
