using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("Jump settings")]
    [SerializeField] float jumpVelocity = 50f;
    [SerializeField] float jumpTilt = -1f;
    [SerializeField] float stabilizationForce = 50f;
    [Range(0f, 1f)]
    [SerializeField] float trajectoryAngle = 0.5f;

    protected override void OnWorkBegin()
    {
        Rigidbody useRigidbody = physics.bodyRigidbody;
        Vector3 jumpTrajectory = vehicle.transform.up * trajectoryAngle;
        jumpTrajectory += vehicle.transform.forward;
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
}
