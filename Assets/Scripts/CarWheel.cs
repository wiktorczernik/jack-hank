using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ConfigurableJoint))]
public class CarWheel : MonoBehaviour
{
    [Header("Settings")]
    public bool isDrivable = true;
    public bool isTurnable = true;
    public float friction = 100;
    [Header("Components")]
    public Rigidbody useRigidbody;
    public SphereCollider useCollider;
    public ConfigurableJoint joint;


    public void ApplyFriction() => ApplyFriction(1);
    public void ApplyFriction(float forceFraction)
    {
        forceFraction = Mathf.Clamp01(forceFraction);
        Vector3 dir = transform.up;
        float force = Vector3.Dot(useRigidbody.linearVelocity, dir) * friction;
        useRigidbody.AddForce(-dir * force * forceFraction);
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
