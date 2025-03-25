using UnityEngine;

public class PlayerVehicleSeatController
{
    public int availableSeats;

    private readonly PlayerVehicleSeat[] _seats;


    public PlayerVehicleSeatController(Transform[] seatPoints)
    {
        int count = seatPoints.Length;
        availableSeats = count;
        _seats = new PlayerVehicleSeat[count];
        for (int i = 0; i < count; i++)
        {
            _seats[i] = new PlayerVehicleSeat(seatPoints[i]);
        }
    }

    public PlayerVehicleSeat Occupy(PickupablePassenger passenger)
    {
        if (availableSeats == 0)
        {
            return null;
        }
        foreach (PlayerVehicleSeat seat in _seats)
        {
            if (seat.isOccupied) continue;
            seat.Occupy(passenger);
            availableSeats--;
            return seat;
        }
        return null;
    }

}
public class PlayerVehicleSeat
{
    public Transform point;
    public PickupablePassenger passenger { get; private set; } = null;
    public bool isOccupied { get; private set; } = false;

    public PlayerVehicleSeat(Transform point)
    {
        this.point = point;
    }

    public void Occupy(PickupablePassenger passenger)
    {
        this.passenger = passenger;
        isOccupied = true;
    }
    public void Free()
    {
        this.passenger = null;
        isOccupied = false;
    }
}