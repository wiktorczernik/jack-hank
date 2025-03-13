using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class VehiclePhysics : MonoBehaviour
{
    #region State
    [Header("State")]
    [SerializeField] bool _isDrifting = false;
    [SerializeField] bool _isDriftingRight = false;
    [SerializeField] Vector2 _input = Vector2.zero;
    [SerializeField] float _driftAngular = 0.0f;
    [SerializeField] float _driftEndBuildup = 0.0f;
    public bool isDrifting { get => _isDrifting; private set => _isDrifting = value; }
    public bool isDriftingRight { get => _isDriftingRight; private set => _isDriftingRight = value; }
    public Vector3 input { get => _input; private set => _input = value; }
    public float driftAngular { get => _driftAngular; private set => _driftAngular = value; }
    public float driftEndBuildup { get => _driftEndBuildup; private set => _driftEndBuildup = value; }
    public int speedKmh => Mathf.RoundToInt(bodyRigidbody.linearVelocity.magnitude * 3.6f);
    public int speedKmhForward => Mathf.Abs(Mathf.RoundToInt(Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward) * 3.6f));
    #endregion

    #region Speed
    [Header("Speed")]
    [SerializeField] float _maxForwardSpeed = 100f;
    [SerializeField] float _maxBackwardSpeed = 100f;
    [SerializeField] float _forwardAcceleration = 100f;
    [SerializeField] float _forwardBrakeForce = 100f;
    public float maxForwardSpeed { get => _maxForwardSpeed; private set => _maxForwardSpeed = value; }
    public float maxBackwardSpeed { get => _maxBackwardSpeed; private set => _maxBackwardSpeed = value; }
    public float forwardAcceleration { get => _forwardAcceleration; private set => _forwardAcceleration = value; }
    public float forwardBrakeForce { get => _forwardBrakeForce; private set => _forwardBrakeForce = value; }
    #endregion

    #region Turning
    [Header("Turning")]
    [SerializeField] float _turnForce = 5000f;
    [SerializeField] float _turnAginForce = 7500;
    [SerializeField] float _turnStabilization = 8f;
    [SerializeField] AnimationCurve _turnForceCurve;
    public float turnForce { get => _turnForce; private set => _turnForce = value; }
    public float turnAginForce { get => _turnAginForce; private set => _turnAginForce = value; }
    public float turnStabilization { get => _turnStabilization; private set => _turnStabilization = value; }
    public AnimationCurve turnForceCurve { get => _turnForceCurve; private set => _turnForceCurve = value; }
    #endregion

    #region Wheels
    [Header("Wheels")]
    [SerializeField] CarWheel[] _wheels;
    [SerializeField] float _bonusWheelGravity = 50f;
    public CarWheel[] wheels { get => _wheels; private set => _wheels = value; }
    public float bonusWheelGravity { get => _bonusWheelGravity; private set => _bonusWheelGravity = value; }
    #endregion

    #region Drifting
    [Header("Drifting")]
    [SerializeField] float _driftMinSpeed = 15f;
    [SerializeField] float _driftTurnForce = 3000f;
    [SerializeField] float _driftMoveForce = 1500f;
    [SerializeField] float _driftLeanAngle = 15f;
    [SerializeField] float _driftClimbRate = 1.7f;
    [SerializeField] float _driftClimbAginRate = 3.75f;
    [SerializeField] float _driftSinkRate = 6.25f;
    [SerializeField] float _driftMaxAngular = 1.5f;
    [SerializeField] float _driftStartAngular = 1.1f;
    [SerializeField] float _driftEndAngular = 0.5f;
    [SerializeField] float _driftEndTime = 0.5f;
    public float driftMinSpeed { get => _driftMinSpeed; private set => _driftMinSpeed = value; }
    public float driftTurnForce { get => _driftTurnForce; private set => _driftTurnForce = value; }
    public float driftMoveForce { get => _driftMoveForce; private set => _driftMoveForce = value; }
    public float driftLeanAngle { get => _driftLeanAngle; private set => _driftLeanAngle = value; }
    public float driftClimbRate { get => _driftClimbRate; private set => _driftClimbRate = value; }
    public float driftClimbAginRate { get => _driftClimbAginRate; private set => _driftClimbAginRate = value; }
    public float driftSinkRate { get => _driftSinkRate; private set => _driftSinkRate = value; }
    public float driftMaxAngular { get => _driftMaxAngular; private set => _driftMaxAngular = value; }
    public float driftStartAngular { get => _driftStartAngular; private set => _driftStartAngular = value; }
    public float driftEndAngular { get => _driftEndAngular; private set => _driftEndAngular = value; }
    public float driftEndTime { get => _driftEndTime; private set => _driftEndTime = value; }
    #endregion

    #region Physics
    [Header("Physics")]
    [SerializeField] Rigidbody _bodyRigidbody;
    [SerializeField] Transform _centerOfMass;
    [SerializeField] float _frictionForce = 750f;
    public Rigidbody bodyRigidbody { get => _bodyRigidbody; private set => _bodyRigidbody = value; }
    public Transform centerOfMass { get => _centerOfMass; private set => _centerOfMass = value; }
    public float frictionForce { get => _frictionForce; private set => _frictionForce = value; }
    #endregion

    #region Visuals
    [Header("Visuals")]
    [Obsolete]
    public VehicleSkidmarkSource skidmarkSourceLeft;
    [Obsolete]
    public VehicleSkidmarkSource skidmarkSourceRight;
    [Obsolete]
    public ParticleSystem smokeSourceLeft;
    [Obsolete]
    public ParticleSystem smokeSourceRight;
    #endregion


    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }
    public float GetForwardSpeed()
    {
        return Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward);
    }
    public void Accelerate(float force = 1.0f)
    {
        Vector3 direction = transform.forward;
        Quaternion driftRotation = Quaternion.identity;
        if (isDrifting)
        {
            float driftFactor = driftAngular / driftMaxAngular;
            driftRotation = Quaternion.Euler(0, driftFactor * driftLeanAngle, 0);
        }
        float sp = Vector3.Dot(bodyRigidbody.linearVelocity, direction) * 3.6f;
        if (sp > maxForwardSpeed)
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
        bodyRigidbody.AddForceAtPosition(driftRotation * direction * forwardAcceleration * force, centerOfMass.position);
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
        if (_isDrifting) return;
        float sp = Mathf.Abs(Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward));
        if (GetForwardSpeed() < 0)
        {
            input = -input;
        }
        sp *= 3.6f;
        sp /= maxForwardSpeed;
        sp = Mathf.Clamp01(sp);
        sp = turnForceCurve.Evaluate(sp);
        bool isCounter = Mathf.Sign(bodyRigidbody.angularVelocity.y) != Mathf.Sign(input);
        bodyRigidbody.AddTorque(transform.up * input * (isCounter ? turnAginForce : turnForce) * sp);
    }
    public void DoDrift(float input)
    {
        if (!isDrifting) return;
        input = Mathf.Clamp(input, -1, 1);
        if (input != 0)
        {
            float driftSign = isDriftingRight ? 1 : -1;
            float chosenFactor = driftClimbRate;
            if (Mathf.Sign(input) != Mathf.Sign(driftAngular))
            {
                chosenFactor = driftClimbAginRate;
            }
            driftAngular += input * chosenFactor * Time.fixedDeltaTime;
        }
        else
        {
            driftAngular = Mathf.SmoothStep(driftAngular, 0, driftSinkRate * Time.fixedDeltaTime);
        }
        driftAngular = Mathf.Clamp(driftAngular, -driftMaxAngular, driftMaxAngular);
        Vector3 newAngular = bodyRigidbody.angularVelocity;
        newAngular.y = driftAngular;
        bodyRigidbody.angularVelocity = newAngular;
    }
    void HandleWheels()
    {
        foreach(CarWheel wheel in wheels)
        {
            wheel.isDrifting = isDrifting;
            wheel.ApplyFriction();
            wheel.ApplyGravity(bonusWheelGravity);
        }
    }
    void HandleSkidmarks()
    {
        
        if (wheels[2].IsGrounded())
        {
            if (isDrifting)
            {
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - driftEndAngular;
                if (factor > 0.1f && driftEndBuildup <= float.Epsilon)
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
                float factor = Mathf.Abs(bodyRigidbody.angularVelocity.y) - driftEndAngular;
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
        Vector3 velocity = bodyRigidbody.linearVelocity;
        float magnitude = velocity.magnitude;
        if (magnitude > float.Epsilon)
        {
            Vector3 frictionForce = -velocity.normalized;
            frictionForce *= this.frictionForce;
            bodyRigidbody.AddForce(frictionForce);
        }
    }

    private void FixedUpdate()
    {
        bool isPlaying = GameManager.isDuringRun;

        ManageDriftState();

        HandleWheels();
        HandleSkidmarks();

        if (input.y > 0 && isPlaying)
        {
            Accelerate(input.y);
        }
        if (input.y < 0 && isPlaying)
        {
            Brake(-input.y);
        }
        if (isPlaying)
        {
            DoTurn(input.x);
            DoDrift(input.x);
        }
        ApplyFriction();
        if (!isDrifting && input.x == 0)
        {
            Vector3 av = bodyRigidbody.angularVelocity;
            av.y = 0;
            bodyRigidbody.angularVelocity = Vector3.Lerp(bodyRigidbody.angularVelocity, av, turnStabilization * Time.fixedDeltaTime);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (bodyRigidbody == null)
        {
            bodyRigidbody = GetComponent<Rigidbody>();
            bodyRigidbody.maxLinearVelocity = maxForwardSpeed;
        }
        if (centerOfMass == null)
        {
            centerOfMass = bodyRigidbody.transform;
        }
        if (wheels.Length == 0)
        {
            wheels = GetComponentsInChildren<CarWheel>();
        }
    }
#endif
}
