using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("Jump settings")]
    [SerializeField] float jumpVelocity = 50f;
    [SerializeField] float jumpTilt = -1f;
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
    public override bool UsageConditionsSatisfied()
    {
        return physics.isGrounded;
    }
}
