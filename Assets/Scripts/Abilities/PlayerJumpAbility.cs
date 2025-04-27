using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("State")]
    [SerializeField] float speedBeforeJump;
    [Header("Jump settings")]
    [SerializeField] float pitchTilt = -1f;
    [SerializeField] float upwardSpeed = 30f;
    [SerializeField] float minForwardSpeed = 50f;
    [SerializeField] float stabilizationForce = 50f;

    protected override void OnWorkBegin()
    {
        physics.onLand += OnLand;

        Rigidbody useRigidbody = physics.bodyRigidbody;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(useRigidbody.linearVelocity, physics.transform.up);
        speedBeforeJump = Mathf.Max(minForwardSpeed, horizontalVelocity.magnitude);

        float forwardSpeed = speedBeforeJump;
        Vector3 jumpVelocity = Vector3.zero;
        jumpVelocity += vehicle.transform.forward * forwardSpeed;
        jumpVelocity += vehicle.transform.up * upwardSpeed;

        useRigidbody.linearVelocity = jumpVelocity;
        useRigidbody.AddTorque(-transform.right * pitchTilt, ForceMode.VelocityChange);
    }
    protected override void OnWorkTick()
    {
        if (physics.isGrounded) return;

        Vector3 up = transform.up;
        Vector3 targetUp = physics.groundNormal;
        Vector3 torque = Vector3.Cross(up, targetUp);
        torque *= stabilizationForce;
        torque *= Time.fixedDeltaTime;
        physics.bodyRigidbody.AddTorque(torque, ForceMode.VelocityChange);
    }
    public override bool UsageConditionsSatisfied()
    {
        return physics.isGrounded;
    }

    private void OnLand(VehiclePhysics.AirTimeState airTime)
    {
        physics.bodyRigidbody.linearVelocity = vehicle.transform.forward * speedBeforeJump;
        physics.bodyRigidbody.angularVelocity = Vector3.zero;
        physics.onLand -= OnLand;
    }
}
