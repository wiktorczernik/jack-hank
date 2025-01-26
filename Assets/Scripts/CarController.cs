using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public int speedKmh => Mathf.RoundToInt(BodyRigidbody.linearVelocity.magnitude * 3.6f);
    public int speedKmhForward => Mathf.Abs(Mathf.RoundToInt(Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward) * 3.6f));

    public VehicleSkidmarkSource skidmarkSourceLeft;
    public VehicleSkidmarkSource skidmarkSourceRight;

    public ParticleSystem smokeSourceLeft;
    public ParticleSystem smokeSourceRight;

    public Rigidbody BodyRigidbody => bodyRigidbody;
    public Transform CenterOfMass;

    [Header("Wheels")]
    public CarWheel[] wheels;

    [Header("Settings")]
    public float MaxSpeed = 100;
    public float Acceleration = 1700f; 
    public float BrakeForce = 1000f;
    public float TurnForce = 1500f;
    public float DriftTurnForce = 1000f;
    [Obsolete]
    public float DriftForce = 5000f;
    public float DriftAngle = 45f;
    public AnimationCurve TestCurve;
    public AnimationCurve TurnForceCurve;

    [Header("Input")]
    [SerializeField] Vector2 moveInput = Vector2.zero;
    [SerializeField] bool isDrifting = false;


    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input;
    }
    public float GetForwardSpeed()
    {
        return Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward);
    }

    public bool IsDrifting()
    {
        return Mathf.Abs(bodyRigidbody.angularVelocity.y) > 1f;
    }
    public void Accelerate(float force = 1.0f)
    {
        float sp = Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward) * 3.6f;
        if (sp > MaxSpeed)
        {
            Debug.Log("Capped front");
            return;
        }
        float wheelGroundFactor = 0.0f;
        foreach(var wheel in wheels)
        {
            if (wheel.IsGrounded())
            {
                wheelGroundFactor += 0.25f;
            }
        }
        force = Mathf.Clamp01(force) * wheelGroundFactor;
        Vector3 direction = transform.forward;

        BodyRigidbody.AddForceAtPosition(direction * Acceleration * force, CenterOfMass.position);
    }

    public void Brake(float force = 1.0f)
    {
        if (Vector3.Dot(BodyRigidbody.linearVelocity, -transform.forward) * 3.6f > MaxSpeed)
        {
            Debug.Log("Capped back");
            return;
        }
        force = Mathf.Clamp01(force);
        BodyRigidbody.AddForceAtPosition(-transform.forward * BrakeForce * force, CenterOfMass.position);
    }
    public void DoTurn(float input)
    {
        input = Mathf.Clamp(input, -1, 1);
        float sp = Mathf.Abs(Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward));
        if (GetForwardSpeed() < 0)
        {
            input = -input;
        }
        sp *= 3.6f;
        sp /= MaxSpeed;
        sp = Mathf.Clamp01(sp);
        sp = TurnForceCurve.Evaluate(sp);
        BodyRigidbody.AddTorque(transform.up * input * ( IsDrifting() ? DriftTurnForce : TurnForce ) * sp);
    }
    void HandleWheels()
    {
        foreach(CarWheel wheel in wheels)
        {
            wheel.ApplyFriction();
        }
    }
    void HandleSkidmarks()
    {
        
        if (wheels[2].IsGrounded())
        {
            if (IsDrifting())
            {
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - 1f;
                if (factor > 0.5f)
                {
                    smokeSourceRight.Play();
                }
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
        if (wheels[3].IsGrounded())
        {
            if (IsDrifting())
            {
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - 1f;
                if (factor > 0.5f)
                {
                    smokeSourceLeft.Play();
                }
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

    private void FixedUpdate()
    {
        if (!GameManager.isDuringRun) return;
        HandleWheels();
        HandleSkidmarks();

        if (moveInput.y > 0)
        {
            Accelerate(moveInput.y);
        }
        else if (moveInput.y < 0)
        {
            Brake(Mathf.Abs(moveInput.y));
        }
        if (moveInput.x != 0)
        {
            DoTurn(moveInput.x);
        }
        if (IsDrifting())
        {
            float ratio = speedKmh / MaxSpeed;
            Vector3 dir = Quaternion.AngleAxis(BodyRigidbody.angularVelocity.y * -DriftAngle, Vector3.up) * transform.forward;
            bodyRigidbody.AddForceAtPosition(dir * DriftForce * Time.fixedDeltaTime, bodyRigidbody.transform.TransformPoint(bodyRigidbody.centerOfMass), ForceMode.Acceleration);
        }
    }

    void Awake()
    {
        bodyRigidbody = GetComponent<Rigidbody>();
        bodyRigidbody.maxLinearVelocity = 150f;
    }


    Rigidbody bodyRigidbody;
}
