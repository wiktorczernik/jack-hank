using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileEntity : GameEntity
{
    [Header("Flight")]
    [Tooltip("Locks the projectile in place")] public bool isStationary = false;
    public float acceleration = 30f;
    public float startSpeed = 0f;

    [Header("Homing")]
    [Tooltip("Leave null to disable homing")] public Transform homingTarget = null;

    [Tooltip("Shifts the homing position relative to the target. By default set for the player bus")] public Vector3 localTargetShift = new Vector3(0f, -1.5f, 4.5f);
    [Tooltip("Shifts the homing position relative to the world")] public Vector3 targetShift;

    public float rotationRate = 40f;

    [Tooltip("The minimum angle between the velocity vector and the target vector to start dampening projectile's velocity")]
    public float minDampeningVelocityAngle = 60f;
    [Tooltip("The minimum angle between the forward vector and the target vector to start boosting the projectile")]
    public float maxBoostingLookAngle = 45f;
    [Tooltip("The minimum angle between the velocity vector and the target vector to start boosting the projectile")] 
    public float minBoostingVelocityAngle = 90f;

    public float turnBackDecceleration = 60f;
    public float turnBackBoost = 10f;
    public float snapAngle = 2f;

    [Header("Components")]
    public Rigidbody rigidbody_;

    [Header("Events")]
    public UnityEvent OnShot;
    public UnityEvent OnHalted;

    #region DATA
    [HideInInspector, Tooltip("Short for 'projectile forward'")] public Vector3 projFwd => transform.forward;
    #endregion

    protected virtual void FixedUpdate()
    {
        rigidbody_.isKinematic = isStationary;
        if (isStationary) return;

        bool applyBoost = false;

        if (homingTarget != null)
        {
            Vector3 targetVector = homingTarget.position + targetShift + homingTarget.rotation * localTargetShift - transform.position;

            float angle = Mathf.Abs(Vector3.Angle(projFwd, targetVector));
            float velAngle = Mathf.Abs(Vector3.Angle(rigidbody_.linearVelocity, targetVector));

            // Hard turning back
            if (velAngle >= minDampeningVelocityAngle && rigidbody_.linearVelocity.magnitude >= 0.1f) 
                rigidbody_.AddForce(-rigidbody_.linearVelocity.normalized * turnBackDecceleration, ForceMode.Acceleration);
            if (angle <= maxBoostingLookAngle && velAngle >= minBoostingVelocityAngle) applyBoost = true;

            // Rotating
            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(targetVector), Time.fixedDeltaTime * rotationRate);

            angle = Mathf.Abs(Vector3.Angle(projFwd, targetVector));

            if (angle <= snapAngle) transform.LookAt(homingTarget);
        }

        rigidbody_.AddForce(projFwd * (acceleration + (applyBoost ? turnBackBoost : 0f)), ForceMode.Acceleration);
    }

    public virtual void Shoot()
    {
        isStationary = false;
        rigidbody_.linearVelocity = projFwd * startSpeed;

        rigidbody_.constraints = RigidbodyConstraints.None;

        OnShot?.Invoke();
    }

    public virtual void Halt()
    {
        isStationary = true;
        rigidbody_.linearVelocity = Vector3.zero;

        OnHalted?.Invoke();
    }
}
