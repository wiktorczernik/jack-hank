using UnityEngine;

public abstract class Vehicle : GameEntity
{
    public VehiclePhysics physics;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (physics == null)
        {
            physics = GetComponentInChildren<VehiclePhysics>();
        }
    }
#endif
}
