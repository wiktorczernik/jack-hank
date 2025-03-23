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

    public float jumpAcceleration = 10000f;
    public float jumpTime = 1.0f;

    private float boostElapsedTime;
    private bool isBoosting = false;
    private bool enableAfterBoostSlowDown = false;

    private float jumpElapsedTime;
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

/*    public void OnJump()
    {
        if (carController.isGrounded())
        {
            if(!isJumping)
            {
                isJumping = true;
                jumpElapsedTime = 0;
                print("jump");
            }
        }
    }*/

    private void FixedUpdate()
    {
        //HandleJump();
        HandleSpeedBoost();
    }

/*    private void HandleJump()
    {
        if (isJumping)
        {
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime <= jumpTime)
            {
                carController.BodyRigidbody.AddForce(transform.up * jumpAcceleration);
            }
            else
            {
                isJumping = false;
            }
        }
    }
*/
    private void HandleSpeedBoost()
    {
        if (isBoosting)
        {
            boostElapsedTime += Time.deltaTime;    
            if(boostElapsedTime >= speedBoostTime) { 
                isBoosting=false;
                carController.MaxSpeed /= speedBoostMaxSpeedMultiplier;
                carController.Acceleration /= (speedBoostAccelerationMultiplier - 1.0f);
                
                return;
            }
            carController.Accelerate(1.0f, true);
            return;
        }
        float forwardSpeed = carController.GetForwardSpeed() * 3.6f;
        if(forwardSpeed > (carController.MaxSpeed + 1.0f) && enableAfterBoostSlowDown) {
            carController.BodyRigidbody.AddForceAtPosition(-transform.forward * carController.BrakeForce * speedBoostBreakForceMultiplier, carController.CenterOfMass.position);
        }
        else
        {
            enableAfterBoostSlowDown= false;
        }
    }

}
