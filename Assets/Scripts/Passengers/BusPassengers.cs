using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusPassengers : MonoBehaviour
{
    public int BusMaxCapacity = 20;
    
    public List<Passenger> Passengers;

    public int GetActualCapacity()
    {
        return BusMaxCapacity - Passengers.Count;
    }

    public void AddPassengerOnList(Passenger passenger)
    {
        Passengers.Add(passenger);
    }

    public void DeletePassengerFromList(Passenger passenger)
    {
        Passengers.Remove(passenger);
    }
}
