using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestShift : Quest
{
    [SerializeField] private BusStop actualDestination;

    private void Start()
    {
        actualDestination.GetComponent<Collider>().enabled = true;
    }

    private void Update()
    {
        if (actualDestination.IsCompleted & !IsCompleted)
        {
            actualDestination.GetComponent<Collider>().enabled = false;
            if (actualDestination.NextBusStop == null)
            {
                IsCompleted = true;
                Debug.Log("Ukonczyles trase! Brawo!!!");
            }
            else
            {
                actualDestination = actualDestination.NextBusStop;
            }
            
        }
    }

}
