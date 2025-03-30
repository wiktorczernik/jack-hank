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
    public float MaxBackwardSpeed = 15f;
    public float Acceleration = 2300f; 
    public float BrakeForce = 4500f;
    public float TurnForce = 5000f;
    public float CounterTurnForce = 7500;
    public float TurnStabilizeSpeed = 8f;
    public float DriftTurnForce = 3000f;
    public float DriftMoveForce = 100f;
    public float DriftAngle = 45f;
    public float DriftSpeeddown = 3;
    public float DriftSpeedup = 5;
    public float DriftMinSpeed = 15f;
    public float MinFrictionMagnitude = 50f;
    public float MaxFrictionForce = 50f;
    public float DriftCounterSpeedup = 8;
    public float MaxDriftAngularY = 1.5f;
    public float DriftStartThreshold = 1.1f;
    public float DriftEndThreshold = 0.4f;
    public float DriftEndMaxTimer = 1.0f;
    public float AdditionalWheelGravity = 50f;
    public AnimationCurve TestCurve;
    public AnimationCurve TurnForceCurve;

    [Header("Input")]
    [SerializeField] Vector2 moveInput = Vector2.zero;
    [Header("State")]
    public bool isDrifting = false;
    public bool isDriftingRight = false;
    public float driftAngularY = 0.0f;
    public float driftEndTimer = 0.0f;


    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input;
    }
    public float GetForwardSpeed()
    {
        return Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward);
    }
    public void Accelerate(float force = 1.0f)
    {
        Vector3 direction = transform.forward;
        Quaternion driftRotation = Quaternion.identity;
        if (isDrifting)
        {
            float driftFactor = driftAngularY / MaxDriftAngularY;
            driftRotation = Quaternion.Euler(0, driftFactor * DriftAngle, 0);
        }
        float sp = Vector3.Dot(BodyRigidbody.linearVelocity, direction) * 3.6f;
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
        BodyRigidbody.AddForceAtPosition(driftRotation * direction * Acceleration * force, CenterOfMass.position);
    }

    public void Brake(float force = 1.0f)
    {
        if (Vector3.Dot(BodyRigidbody.linearVelocity, -transform.forward) * 3.6f > MaxBackwardSpeed)
        {
            Debug.Log("Capped back");
            return;
        }
        force = Mathf.Clamp01(force);
        BodyRigidbody.AddForceAtPosition(-transform.forward * BrakeForce * force, CenterOfMass.position);
    }
    public void DoTurn(float input)
    {
        if (input == 0) return;
        input = Mathf.Clamp(input, -1, 1);
        if (isDrifting) return;
        float sp = Mathf.Abs(Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward));
        if (GetForwardSpeed() < 0)
        {
            input = -input;
        }
        sp *= 3.6f;
        sp /= MaxSpeed;
        sp = Mathf.Clamp01(sp);
        sp = TurnForceCurve.Evaluate(sp);
        bool isCounter = Mathf.Sign(bodyRigidbody.angularVelocity.y) != Mathf.Sign(input);
        BodyRigidbody.AddTorque(transform.up * input * (isCounter ? CounterTurnForce : TurnForce) * sp);
    }
    public void DoDrift(float input)
    {
        if (!isDrifting) return;
        input = Mathf.Clamp(input, -1, 1);
        if (input != 0)
        {
            float driftSign = isDriftingRight ? 1 : -1;
            float chosenFactor = DriftSpeedup;
            if (Mathf.Sign(input) != Mathf.Sign(driftAngularY))
            {
                chosenFactor = DriftCounterSpeedup;
            }
            driftAngularY += input * chosenFactor * Time.fixedDeltaTime;
        }
        else
        {
            driftAngularY = Mathf.SmoothStep(driftAngularY, 0, DriftSpeeddown * Time.fixedDeltaTime);
        }
        driftAngularY = Mathf.Clamp(driftAngularY, -MaxDriftAngularY, MaxDriftAngularY);
        Vector3 newAngular = bodyRigidbody.angularVelocity;
        newAngular.y = driftAngularY;
        bodyRigidbody.angularVelocity = newAngular;
    }
    void HandleWheels()
    {
        foreach(CarWheel wheel in wheels)
        {
            wheel.isDrifting = isDrifting;
            wheel.ApplyFriction();
            wheel.ApplyGravity(AdditionalWheelGravity);
        }
    }
    void HandleSkidmarks()
    {
        
        if (wheels[2].IsGrounded())
        {
            if (isDrifting)
            {
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - DriftEndThreshold;
                if (factor > 0.1f && driftEndTimer <= float.Epsilon)
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
            if (isDrifting)
            {
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - DriftEndThreshold;
                if (factor > 0.1f)
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

    private void ManageDriftState()
    {
        float angY = bodyRigidbody.angularVelocity.y;
        float absAngY = Mathf.Abs(angY);

        if (isDrifting)
        {
            if (absAngY < DriftEndThreshold || speedKmh < DriftMinSpeed)
            {
                driftEndTimer += Time.fixedDeltaTime;
                if (driftEndTimer >= DriftEndMaxTimer)
                {
                    isDrifting = false;
                    isDriftingRight = false;
                    driftAngularY = 0.0f;
                    driftEndTimer = 0.0f;
                }
            }
            else
            {
                driftEndTimer = 0.0f;
            }
        }
        else
        {
            if (absAngY > DriftStartThreshold && speedKmh > DriftMinSpeed)
            {
                isDrifting = true;
                isDriftingRight = angY > 0;
                driftAngularY = angY;
                driftEndTimer = 0.0f;
            }
        }
    }
    private void ApplyFriction()
    {
        Vector3 velocity = bodyRigidbody.linearVelocity;
        float magnitude = velocity.magnitude;
        Vector3 frictionForce = -velocity.normalized;
        if (magnitude >= MinFrictionMagnitude)
        {
            frictionForce *= MaxFrictionForce;
        }
        bodyRigidbody.AddForce(frictionForce);
    }

    private void FixedUpdate()
    {
        bool isPlaying = GameManager.IsDuringRun;

        ManageDriftState();

        HandleWheels();
        HandleSkidmarks();

        if (moveInput.y > 0 && isPlaying)
        {
            Accelerate(moveInput.y);
        }
        if (moveInput.y < 0 && isPlaying)
        {
            Brake(-moveInput.y);
        }
        if (isPlaying)
        {
            DoTurn(moveInput.x);
            DoDrift(moveInput.x);
        }
        ApplyFriction();
        if (!isDrifting && moveInput.x == 0)
        {
            Vector3 av = bodyRigidbody.angularVelocity;
            av.y = 0;
            bodyRigidbody.angularVelocity = Vector3.Lerp(bodyRigidbody.angularVelocity, av, TurnStabilizeSpeed * Time.fixedDeltaTime);
        }
    }

    void Awake()
    {
        bodyRigidbody = GetComponent<Rigidbody>();
        bodyRigidbody.maxLinearVelocity = 150f;
    }


    Rigidbody bodyRigidbody;
}
