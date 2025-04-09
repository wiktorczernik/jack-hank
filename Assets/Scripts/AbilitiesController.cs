using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using TMPro;

[RequireComponent (typeof(PlayerVehicle))]
public class AbilitiesController : MonoBehaviour
{
    public float speedBoostTime = 2.0f;
    public float speedBoostMaxSpeedMultiplier = 3.0f;
    public float speedBoostAccelerationMultiplier = 2.0f;
    public float speedBoostBreakForceMultiplier = 1.5f;

    public float jumpAcceleration = 5000f;
    public float jumpAirTiltBackwardForce = 40;

    public int speedBoostCharges = 0;
    public int jumpCharges = 0;

    private float boostElapsedTime;
    private bool isBoosting = false;
    private bool enableAfterBoostSlowDown = false;

    private bool isJumping;

    private PlayerVehicle parent;
    private VehiclePhysics physics;

    private void Awake()
    {
        parent = GetComponent<PlayerVehicle>();
        physics = parent.physics;
    }

    public void OnSpeedBoost()
    {
        if(!isBoosting && speedBoostCharges > 0)
        {
            isBoosting = true;
            speedBoostCharges--;
            enableAfterBoostSlowDown = true;
            boostElapsedTime = 0;
            physics.maxForwardSpeed *= speedBoostMaxSpeedMultiplier;
            physics.forwardAcceleration *= (speedBoostAccelerationMultiplier - 1.0f);
        }
    }

    public void OnJump()
    {
            if (!isJumping && jumpCharges > 0)
            {
                isJumping = true;
                jumpCharges--;
                physics.bodyRigidbody.AddForce(Vector3.up * jumpAcceleration, ForceMode.Impulse); // Jump
                physics.bodyRigidbody.AddTorque(-transform.right * jumpAirTiltBackwardForce, ForceMode.Acceleration); // Add slight tilt backwards
        }
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleSpeedBoost();

        if (!physics.IsGrounded())
        {
            physics.bodyRigidbody.linearDamping = 1.5f;  // Increase air drag
            physics.bodyRigidbody.angularDamping = 2.0f;  // Reduce rotations
        }
        else
        {
            physics.bodyRigidbody.linearDamping = 0.1f;  // Restore normal state
            physics.bodyRigidbody.angularDamping = 0.05f;
        }

    }

    private void HandleJump()
    {
        if (isJumping)
        {
            if (physics.IsGrounded())
            {
                isJumping = false;
            }
        }
    }


    public void addJumpCharge()
    {
        jumpCharges++;
    }

    public void addSpeedBoostCharge()
    {
        speedBoostCharges++;
    }

    private void HandleSpeedBoost()
    {
        if (isBoosting)
        {
            boostElapsedTime += Time.deltaTime;    
            if(boostElapsedTime >= speedBoostTime) { 
                isBoosting=false;
                physics.maxForwardSpeed /= speedBoostMaxSpeedMultiplier;
                physics.forwardAcceleration /= (speedBoostAccelerationMultiplier - 1.0f);
                return;
            }
            physics.Accelerate(1.0f);
            return;
        }
        float forwardSpeed = physics.GetForwardSpeed() * 3.6f;
        if (physics.IsGrounded())
        {
            if(forwardSpeed > (physics.maxForwardSpeed + 1.0f) && enableAfterBoostSlowDown) {
                physics.bodyRigidbody.AddForce(-transform.forward * physics.forwardBrakeForce * speedBoostBreakForceMultiplier);
            }
            else
            {
                enableAfterBoostSlowDown= false;
            }
        }
    }

}
