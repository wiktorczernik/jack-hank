using UnityEngine;

public abstract class Vehicle : GameEntity
{
    public VehiclePhysics physics;

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        physics.bodyRigidbody.position = position;
        physics.bodyRigidbody.rotation = rotation;
        physics.TeleportWheels(position);
    }

    public void Teleport(Vector3 position)
    {
        physics.bodyRigidbody.position = position;
        physics.TeleportWheels(position);
    }

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
