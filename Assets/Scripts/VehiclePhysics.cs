using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class VehiclePhysics : MonoBehaviour
{
    public event Action<AirTimeState> onTakeOff;
    public event Action<AirTimeState> onAir;
    public event Action<AirTimeState> onLand;

    public void TeleportWheels(Vector3 position)
    {
        var leftBackWheel = wheels[3];
        var leftFrontWheel = wheels[1];
        var rightBackWheel = wheels[2];
        var rightFrontWheel = wheels[0];

        var vectorToLeftFront = leftFrontWheel.useRigidbody.position - leftBackWheel.useRigidbody.position;
        var vectorToRightBack = rightBackWheel.useRigidbody.position - leftBackWheel.useRigidbody.position;
        var vectorToRightFront = rightFrontWheel.useRigidbody.position - leftBackWheel.useRigidbody.position;

        leftBackWheel.useRigidbody.position = position;
        leftFrontWheel.useRigidbody.position = position + vectorToLeftFront;
        rightBackWheel.useRigidbody.position = position + vectorToRightBack;
        rightFrontWheel.useRigidbody.position = position + vectorToRightFront;
    }

    public float GetForwardSpeed()
    {
        return Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward);
    }

    public void Accelerate(float force = 1.0f, bool useCurve = true)
    {
        var direction = transform.forward;
        var driftRotation = Quaternion.identity;
        float driftFactor = 0;
        if (isDrifting)
        {
            driftFactor = Mathf.Abs(driftAngular / driftMaxAngular);
            driftRotation = Quaternion.Euler(0, driftFactor * driftLeanAngle, 0);
        }

        var sp = Vector3.Dot(bodyRigidbody.linearVelocity, direction) * 3.6f;
        if (sp > maxForwardSpeed)
        {
            Debug.Log("Capped front");
            return;
        }

        var wheelGroundFactor = 0.0f;
        foreach (var wheel in wheels)
            if (wheel.CheckGround())
                wheelGroundFactor += 0.25f;
        var acceleration = forwardAcceleration;
        acceleration += driftFactor * driftBonusAcceleration;
        acceleration *= accelerationCurve.Evaluate(speedKmhForward / maxForwardSpeed);

        force = Mathf.Clamp01(force) * wheelGroundFactor;
        bodyRigidbody.AddForceAtPosition(driftRotation * direction * acceleration * force, centerOfMass.position);
    }

    public void Brake(float force = 1.0f)
    {
        if (Vector3.Dot(bodyRigidbody.linearVelocity, -transform.forward) * 3.6f > maxBackwardSpeed)
        {
            Debug.Log("Capped back");
            return;
        }

        force = Mathf.Clamp01(force);
        bodyRigidbody.AddForceAtPosition(-transform.forward * forwardBrakeForce * force, centerOfMass.position);
    }

    public void DoTurn(float input)
    {
        if (input == 0) return;
        input = Mathf.Clamp(input, -1, 1);
        if (isDrifting) return;
        var sp = Mathf.Abs(Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward));
        if (GetForwardSpeed() < 0) input = -input;
        sp *= 3.6f;
        sp /= maxForwardSpeed;
        sp = Mathf.Clamp01(sp);
        sp = turnForceCurve.Evaluate(sp);
        var isCounter = Mathf.Sign(bodyRigidbody.angularVelocity.y) != Mathf.Sign(input);
        bodyRigidbody.AddTorque(transform.up * input * (isCounter ? turnAginForce : turnForce) * sp);
    }

    public void DoDrift(float input)
    {
        if (!isDrifting) return;
        input = Mathf.Clamp(input, -1, 1);
        if (input != 0)
        {
            float driftSign = isDriftingRight ? 1 : -1;
            var chosenFactor = driftClimbRate;
            if (Mathf.Sign(input) != Mathf.Sign(driftAngular)) chosenFactor = driftClimbAginRate;
            driftAngular += input * chosenFactor * Time.fixedDeltaTime;
        }
        else
        {
            driftAngular = Mathf.SmoothStep(driftAngular, 0, driftSinkRate * Time.fixedDeltaTime);
        }

        driftAngular = Mathf.Clamp(driftAngular, -driftMaxAngular, driftMaxAngular);
        var newAngular = bodyRigidbody.angularVelocity;
        newAngular.y = driftAngular;
        bodyRigidbody.angularVelocity = newAngular;
    }

    public void DoAirRoll(Vector3 input)
    {
        if (isGrounded || !allowAirRoll) return;
        if (airTimeState.timePassed < airRollMinTime) return;

        var sideForce = transform.up;
        sideForce *= airRollForce;
        sideForce *= input.x;
        sideForce *= Time.fixedDeltaTime;

        var forwardForce = transform.right;
        forwardForce *= airRollForce;
        forwardForce *= input.y;
        forwardForce *= Time.fixedDeltaTime;

        bodyRigidbody.AddTorque(sideForce, ForceMode.VelocityChange);
        bodyRigidbody.AddTorque(forwardForce, ForceMode.VelocityChange);
    }

    [Obsolete]
    public bool IsGrounded()
    {
        return isGrounded;
    }

    private void CheckGrounded()
    {
        var oldGrounded = isGrounded;
        var newGrounded = false;
        var newDistance = Mathf.Infinity;
        var newState = airTimeState;
        // Average of ground normals under wheels
        var newGroundNormal = Vector3.zero;

        foreach (var wheel in wheels)
        {
            if (wheel.CheckGround())
                newGrounded = true;
            newDistance = Mathf.Min(newDistance, wheel.distanceToGround);
            newGroundNormal += wheel.groundNormal;
        }

        newGroundNormal /= wheels.Length;

        if (oldGrounded || (oldGrounded && !newGrounded))
        {
            newState.timePassed = 0;
            newState.groundHeight = 0;
            newState.maxGroundHeight = 0;
        }

        if (!newGrounded)
        {
            newState.timePassed += Time.fixedDeltaTime;
            newState.groundHeight = newDistance;
            newState.maxGroundHeight = Mathf.Max(newState.maxGroundHeight, newDistance);
        }

        isGrounded = newGrounded;
        airTimeState = newState;
        groundNormal = newGroundNormal;

        if (oldGrounded != newGrounded)
        {
            if (newGrounded)
                onLand?.Invoke(airTimeState);
            else
                onTakeOff?.Invoke(airTimeState);
        }
        else
        {
            onAir?.Invoke(airTimeState);
        }
    }

    private void HandleWheels()
    {
        foreach (var wheel in wheels)
        {
            wheel.isDrifting = isDrifting;
            wheel.ApplyFriction();
            wheel.ApplyGravity(bonusWheelGravity);
        }
    }

    private void HandleSkidmarks()
    {
        if (wheels[2].CheckGround())
        {
            if (isDrifting)
            {
                var factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - driftEndAngular;
                if (factor > 0.1f && driftEndBuildup <= float.Epsilon) smokeSourceRight.Play();
                skidmarkSourceRight.StartEmitting();
            }
            else
            {
                smokeSourceRight.Stop();
                skidmarkSourceRight.StopEmitting();
            }
        }
        else
        {
            smokeSourceRight.Stop();
            skidmarkSourceRight.StopEmitting();
        }

        if (wheels[3].CheckGround())
        {
            if (isDrifting)
            {
                var factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - driftEndAngular;
                if (factor > 0.1f) smokeSourceLeft.Play();
                skidmarkSourceLeft.StartEmitting();
            }
            else
            {
                smokeSourceLeft.Stop();
                skidmarkSourceLeft.StopEmitting();
            }
        }
        else
        {
            smokeSourceLeft.Stop();
            skidmarkSourceLeft.StopEmitting();
        }
    }

    private void DampenVelocity(Collision collision)
    {
        var newVelocity = lastVelocity;
        var reflect = true;
        var hitObject = collision.gameObject;

        GameEntity ent;
        if (!hitObject.TryGetComponent(out ent))
        {
            if (hitObject.GetComponentInParent<GameEntity>() != null)
                reflect = false;
        }
        else
        {
            reflect = false;
        }

        if (reflect)
        {
            var hitPointPos = Vector3.zero;
            var hitPointNormal = Vector3.zero;
            foreach (var contact in collision.contacts)
            {
                hitPointPos += contact.point;
                hitPointNormal += contact.normal;
            }

            hitPointPos /= collision.contacts.Length;
            hitPointNormal /= collision.contacts.Length;
            newVelocity = Vector3.Reflect(newVelocity, hitPointNormal);
        }

        bodyRigidbody.linearVelocity = newVelocity;

        lastVelocity = newVelocity;
    }

    private void ManageDriftState()
    {
        var angY = bodyRigidbody.angularVelocity.y;
        var absAngY = Mathf.Abs(angY);

        if (isDrifting)
        {
            if (absAngY < driftEndAngular || speedKmh < driftMinSpeed)
            {
                driftEndBuildup += Time.fixedDeltaTime;
                if (driftEndBuildup >= driftEndTime)
                {
                    isDrifting = false;
                    isDriftingRight = false;
                    driftAngular = 0.0f;
                    driftEndBuildup = 0.0f;
                }
            }
            else
            {
                driftEndBuildup = 0.0f;
            }
        }
        else
        {
            if (absAngY > driftStartAngular && speedKmh > driftMinSpeed)
            {
                isDrifting = true;
                isDriftingRight = angY > 0;
                driftAngular = angY;
                driftEndBuildup = 0.0f;
            }
        }
    }

    private void ApplyFriction()
    {
        var velocity = bodyRigidbody.linearVelocity;
        var magnitude = velocity.magnitude;
        if (magnitude > float.Epsilon)
        {
            var frictionForce = -velocity.normalized;
            frictionForce *= this.frictionForce;
            bodyRigidbody.AddForce(frictionForce);
        }
    }

    [Serializable]
    public struct AirTimeState
    {
        public AirTimeState(float timePassed, float distToGround, float maxDistToGround)
        {
            this.timePassed = timePassed;
            groundHeight = distToGround;
            maxGroundHeight = maxDistToGround;
        }

        public float timePassed;
        public float groundHeight;
        public float maxGroundHeight;
    }
}

