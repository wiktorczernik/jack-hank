using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent (typeof(CarController))]
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

    private CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    public void OnSpeedBoost()
    {
        if(!isBoosting)
        {
            isBoosting = true;
            enableAfterBoostSlowDown = true;
            boostElapsedTime = 0;
            carController.MaxSpeed *= speedBoostMaxSpeedMultiplier;
            carController.Acceleration *= (speedBoostAccelerationMultiplier - 1.0f);
        }
    }

    public void OnJump()
    {
            if (!isJumping)
            {
                isJumping = true;
                carController.BodyRigidbody.AddForce(Vector3.up * jumpAcceleration, ForceMode.Impulse); // Jump
                carController.BodyRigidbody.AddTorque(-transform.right * jumpAirTiltBackwardForce, ForceMode.Acceleration); // Add slight tilt backwards
        }
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleSpeedBoost();

        if (!carController.isGrounded())
        {
            carController.BodyRigidbody.linearDamping = 1.5f;  // Increase air drag
            carController.BodyRigidbody.angularDamping = 2.0f;  // Reduce rotations
        }
        else
        {
            carController.BodyRigidbody.linearDamping = 0.1f;  // Restore normal state
            carController.BodyRigidbody.angularDamping = 0.05f;
        }

    }

    private void HandleJump()
    {
        if (isJumping)
        {
            if (carController.isGrounded())
            {
                isJumping = false;
            }
        }
    }


    private void HandleSpeedBoost()
    {
        if (isBoosting)
        {
            boostElapsedTime += Time.deltaTime;    
            if(boostElapsedTime >= speedBoostTime) { 
                isBoosting=false;
                carController.MaxSpeed /= speedBoostMaxSpeedMultiplier;
                carController.Acceleration /= (speedBoostAccelerationMultiplier - 1.0f);
                carController.ignoreGrounded = false;
                return;
            }
            carController.ignoreGrounded = true;
            carController.Accelerate(1.0f);
            return;
        }
        float forwardSpeed = carController.GetForwardSpeed() * 3.6f;
        if (carController.isGrounded())
        {
            if(forwardSpeed > (carController.MaxSpeed + 1.0f) && enableAfterBoostSlowDown) {
                carController.BodyRigidbody.AddForce(-transform.forward * carController.BrakeForce * speedBoostBreakForceMultiplier);
            }
            else
            {
                enableAfterBoostSlowDown= false;
            }
        }
    }

}
