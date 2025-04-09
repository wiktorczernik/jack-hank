using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ConfigurableJoint))]
public class VehicleWheel : MonoBehaviour
{
    [Header("State")]
    public bool isGrounded = false;
    public float distanceToGround = Mathf.Infinity;
    [Header("Settings")]
    public bool isDrivable = true;
    public bool isTurnable = true;
    public bool isDrifting = false;
    public float friction = 100;
    public float driftFriction = 50;
    [Header("Components")]
    public Rigidbody useRigidbody;
    public SphereCollider useCollider;
    public ConfigurableJoint joint;


    public void ApplyFriction() => ApplyFriction(1);
    public void ApplyFriction(float forceFraction)
    {
        forceFraction = Mathf.Clamp01(forceFraction);
        Vector3 dir = transform.up;
        float force = Vector3.Dot(useRigidbody.linearVelocity, dir) * (isDrifting ? driftFriction : friction);
        useRigidbody.AddForce(-dir * force * forceFraction);
    }
    public void ApplyGravity(float scale = 1.0f)
    {
        bool isGrounded = CheckGround();
        if (!isGrounded)
        {
            useRigidbody.AddForce(Physics.gravity * scale, ForceMode.Force);
        }
    }
    public bool CheckGround()
    {
        isGrounded = GetDistanceToGround() < 0.5f;
        return isGrounded;
    }
    public float GetDistanceToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            distanceToGround = hit.distance;
        }
        else
        {
            distanceToGround = Mathf.Infinity;
        }
        return distanceToGround;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        useRigidbody = GetComponent<Rigidbody>();
        useCollider = GetComponent<SphereCollider>();
        joint = GetComponent<ConfigurableJoint>();
    }
#endif
}
