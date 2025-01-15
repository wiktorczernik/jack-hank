using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : Quest
{
    public BusStop NextBusStop;

    private void Awake()
    {
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            IsCompleted = true;
            if(NextBusStop != null)
            {
                NextBusStop.GetComponent<Collider>().enabled = true;
            }
        }
    }
}
