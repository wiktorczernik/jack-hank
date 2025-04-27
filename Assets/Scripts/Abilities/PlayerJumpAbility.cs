using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("State")]
    [SerializeField] float speedBeforeJump;
    [Header("Jump settings")]
    [SerializeField] float jumpVelocity = 50f;
    [SerializeField] float jumpTilt = -1f;
    [SerializeField] float stabilizationForce = 50f;
    [Range(0f, 1f)]
    [SerializeField] float trajectoryAngle = 0.5f;

    protected override void OnWorkBegin()
    {
        physics.onLand += OnLand;

        Rigidbody useRigidbody = physics.bodyRigidbody;
        speedBeforeJump = useRigidbody.linearVelocity.magnitude;
        Vector3 jumpTrajectory = vehicle.transform.up * trajectoryAngle;
        jumpTrajectory += vehicle.transform.forward * (1 - trajectoryAngle);
        jumpTrajectory.Normalize();

        useRigidbody.AddForce(jumpTrajectory * jumpVelocity, ForceMode.VelocityChange);
        useRigidbody.AddTorque(-transform.right * jumpTilt, ForceMode.VelocityChange);
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
        physics.onLand -= OnLand;
    }
}
