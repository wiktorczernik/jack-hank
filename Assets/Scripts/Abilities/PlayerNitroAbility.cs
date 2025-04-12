using UnityEngine;

public class PlayerNitroAbility : PlayerVehicleAbility
{
    [Header("Nitro Settings")]
    public float maxSpeed = 150f;
    public float accelerationForce = 150f;

    protected override void OnWorkTick()
    {
        var rigidbody = physics.bodyRigidbody;

        Vector3 nitroForce = rigidbody.transform.forward;
        nitroForce *= accelerationForce;

        float sp = Vector3.Dot(rigidbody.linearVelocity, nitroForce.normalized) * 3.6f;
        if (sp > maxSpeed)
        {
            Debug.Log("Capped nitro");
            return;
        }

        rigidbody.AddForce(nitroForce, ForceMode.Impulse);
    }
}
