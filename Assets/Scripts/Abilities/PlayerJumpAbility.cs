using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("State")]
    [SerializeField] float speedBeforeJump;
    [SerializeField] float startPitch;

    [Header("Jump settings")]
    [SerializeField] AnimationCurve pitchCurve;
    [SerializeField] float upwardSpeed = 30f;
    [SerializeField] float minForwardSpeed = 50f;

    [Header("Torque settings")]
    [SerializeField] float almostGroundedTorqueForce = 10f;
    [SerializeField] float pitchTorqueForce = 10f;
    [SerializeField] float rollStabilizationForce = 50f;


    public override bool ContinueWorkWhile()
    {
        return !physics.isGrounded;
    }
    protected override void OnWorkBegin()
    {
        physics.onLand += OnLand;

        Rigidbody useRigidbody = physics.bodyRigidbody;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(useRigidbody.linearVelocity, physics.transform.up);
        speedBeforeJump = Mathf.Max(minForwardSpeed, horizontalVelocity.magnitude);
        Vector3 jumpVelocity = Vector3.zero;
        jumpVelocity += vehicle.transform.forward * speedBeforeJump;
        jumpVelocity += vehicle.transform.up * upwardSpeed;
        Vector3 angular = physics.bodyRigidbody.angularVelocity;
        angular.x = 0;
        angular.z = 0;
        physics.bodyRigidbody.angularVelocity = angular;

        Vector3 currentEuler = vehicle.transform.eulerAngles;
        startPitch = currentEuler.x;
        pitchCurve.keys[0].value = startPitch;

        useRigidbody.linearVelocity = jumpVelocity;
    }
    protected override void OnWorkTick()
    {
        if (physics.isGrounded) return;

        Rigidbody rb = physics.bodyRigidbody;

        float desiredPitch = pitchCurve.Evaluate(workTime);

        Vector3 currentEuler = vehicle.transform.eulerAngles;
        float rawPitch = currentEuler.x;
        float rawRoll = currentEuler.z;

        float currentPitch = (rawPitch > 180f) ? rawPitch - 360f : rawPitch;
        float currentRoll = (rawRoll > 180f) ? rawRoll - 360f : rawRoll;

        float pitchError = desiredPitch - currentPitch;
        float rollError = -currentRoll;

        Vector3 angularVelLocal = vehicle.transform.InverseTransformDirection(rb.angularVelocity);
        float pitchVelocity = angularVelLocal.x;
        float rollVelocity = angularVelLocal.z;

        float pitchDamping = -pitchVelocity;
        float rollDamping = -rollVelocity;

        float pitchControl = pitchError * pitchTorqueForce + pitchDamping * pitchTorqueForce * 0.5f;
        float rollControl = rollError * rollStabilizationForce + rollDamping * rollStabilizationForce * 0.5f;

        Vector3 torqueWorld = vehicle.transform.right * pitchControl
                            + vehicle.transform.forward * rollControl;

        rb.AddTorque(torqueWorld * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public override bool UsageConditionsSatisfied()
    {
        return physics.isGrounded;
    }

    private void OnLand(VehiclePhysics.AirTimeState airTime)
    {
        Debug.Log("LANDED");
        physics.bodyRigidbody.linearVelocity = vehicle.transform.forward * speedBeforeJump;
        physics.bodyRigidbody.angularVelocity = Vector3.zero;
        Vector3 angular = physics.bodyRigidbody.angularVelocity;
        angular.x = 0;
        angular.z = 0;
        physics.bodyRigidbody.angularVelocity = angular;
        physics.onLand -= OnLand;
    }
}
