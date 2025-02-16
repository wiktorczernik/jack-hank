using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ConfigurableJoint))]
public class CarWheel : MonoBehaviour
{
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
        bool isGrounded = IsGrounded();
        if (!isGrounded)
        {
            useRigidbody.AddForce(Physics.gravity * scale, ForceMode.Force);
        }
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            return true;
        }
        return false;
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
