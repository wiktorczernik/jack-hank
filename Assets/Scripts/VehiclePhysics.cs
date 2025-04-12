using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class VehiclePhysics : MonoBehaviour
{
    public event Action<AirTimeState> onTakeOff;
    public event Action<AirTimeState> onAir;
    public event Action<AirTimeState> onLand;

    #region State
    [Header("State")]
    public bool isGrounded = false;
    public Vector3 groundNormal = Vector3.up;
    public Vector3 lastVelocity = Vector3.zero;
    public AirTimeState airTimeState = new AirTimeState(0f, 0f, 0f);
    public bool isDrifting = false;
    public bool isDriftingRight = false;
    public Vector3 input = Vector3.zero;
    public float driftAngular = 0;
    public float driftEndBuildup = 0;
    public int speedKmh;
    public int speedKmhForward;
    #endregion

    #region Speed
    [Header("Speed")]
    public float maxForwardSpeed = 100;
    public float maxBackwardSpeed = 15;
    public float forwardAcceleration = 2300;
    public float forwardBrakeForce = 4500f;
    public float hitAlignTorque = 50f;
    public AnimationCurve accelerationCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.9871719f, -0.006936253f, -0.006936253f)
            {
                weightedMode = WeightedMode.Out,
                outWeight = 0.2405324f
            },
            new Keyframe(1.0f, 0.25f, -1.0398812f, -1.0398812f)
            {
                weightedMode = WeightedMode.In,
                inWeight = 0.099405944f
            }
        );
    #endregion

    #region Air roll
    [Header("Air roll")]
    public bool allowAirRoll = false;
    public float airRollForce = 1f;
    public float airRollMinTime = 1.25f;
    #endregion

    #region Turning
    [Header("Turning")]
    public float turnForce = 5000;
    public float turnAginForce = 7500;
    public float turnStabilization = 8;
    public AnimationCurve turnForceCurve = new AnimationCurve(
            new Keyframe(0.00029324335628189147f, -0.00036793947219848633f, -0.035219479352235797f, -0.035219479352235797f, 0.3333333432674408f, 0.4983673095703125f),
            new Keyframe(0.16586846113204957f, 0.7770610451698303f, 2.478717088699341f, 2.478717088699341f, 0.2105962336063385f, 0.07951834797859192f),
            new Keyframe(0.6138836145401001f, 0.8381440043449402f, -0.3972685933113098f, -0.3972685933113098f, 0.14587171375751496f, 0.163782998919487f),
            new Keyframe(0.99560546875f, 0.7938946485519409f, -0.059095773845911029f, -0.059095773845911029f, 0.07981926202774048f, 0.0f)
        );
    #endregion

    #region Wheels
    [Header("Wheels")]
    public VehicleWheel[] wheels;
    public float bonusWheelGravity = 50;
    #endregion

    #region Drifting
    [Header("Drifting")]
    public float driftBonusAcceleration = 1.5f;
    public float driftMinSpeed = 15f;
    public float driftTurnForce = 3000f;
    public float driftMoveForce = 1500f;
    public float driftLeanAngle = 15f;
    public float driftClimbRate = 1.7f;
    public float driftClimbAginRate = 3.75f;
    public float driftSinkRate = 6.25f;
    public float driftMaxAngular = 1.5f;
    public float driftStartAngular = 1.1f;
    public float driftEndAngular = 0.5f;
    public float driftEndTime = 0.5f;
    #endregion

    #region Physics
    [Header("Physics")]
    public Rigidbody bodyRigidbody;
    public CollisionEventEmitter collisionEvents;
    public Transform centerOfMass;
    public float frictionForce = 750f;
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


    public float GetForwardSpeed()
    {
        return Vector3.Dot(bodyRigidbody.linearVelocity, transform.forward);
    }
    public void Accelerate(float force = 1.0f, bool useCurve = true)
    {
        Vector3 direction = transform.forward;
        Quaternion driftRotation = Quaternion.identity;
        float driftFactor = 0;
        if (isDrifting)
        {
            driftFactor = Mathf.Abs(driftAngular / driftMaxAngular);
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
            if (wheel.CheckGround())
            {
                wheelGroundFactor += 0.25f;
            }
        }
        float acceleration = forwardAcceleration;
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
    public void DoAirRoll(Vector3 input)
    {
        if (isGrounded || !allowAirRoll) return;
        if (airTimeState.timePassed < airRollMinTime) return;

        Vector3 sideForce = transform.up;
        sideForce *= airRollForce;
        sideForce *= input.x;
        sideForce *= Time.fixedDeltaTime;
        
        Vector3 forwardForce = transform.right;
        forwardForce *= airRollForce;
        forwardForce *= input.y;
        forwardForce *= Time.fixedDeltaTime;

        bodyRigidbody.AddTorque(sideForce, ForceMode.VelocityChange);
        bodyRigidbody.AddTorque(forwardForce, ForceMode.VelocityChange);
    }
    [Obsolete]
    public bool IsGrounded() => isGrounded;
    void CheckGrounded()
    {
        bool oldGrounded = isGrounded;
        bool newGrounded = false;
        float newDistance = Mathf.Infinity;
        var newState = airTimeState;
        // Average of ground normals under wheels
        Vector3 newGroundNormal = Vector3.zero;

        foreach (var wheel in wheels)
        {
            if (wheel.CheckGround())
                newGrounded = true;
            newDistance = Mathf.Min(newDistance, wheel.distanceToGround);
            newGroundNormal += wheel.groundNormal;
        }
        newGroundNormal /= wheels.Length;

        if (oldGrounded || oldGrounded && !newGrounded)
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
    void HandleWheels()
    {
        foreach(VehicleWheel wheel in wheels)
        {
            wheel.isDrifting = isDrifting;
            wheel.ApplyFriction();
            wheel.ApplyGravity(bonusWheelGravity);
        }
    }
    void HandleSkidmarks()
    {
        
        if (wheels[2].CheckGround())
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
        if (wheels[3].CheckGround())
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

    void DampenVelocity(Collision collision)
    {
        Vector3 newVelocity = lastVelocity;
        bool reflect = true;
        var hitObject = collision.gameObject;

        GameEntity ent;
        if (!hitObject.TryGetComponent(out ent))
        {
            if (hitObject.GetComponentInParent<GameEntity>() != null)
                reflect = false;
        }
        else
            reflect = false;

        if (reflect)
        {
            Vector3 hitPointPos = Vector3.zero;
            Vector3 hitPointNormal = Vector3.zero;
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

    private void OnEnable()
    {
        collisionEvents.OnEnter.AddListener(DampenVelocity);
    }
    private void OnDisable()
    {
        collisionEvents.OnEnter.RemoveListener(DampenVelocity);
    }
    private void FixedUpdate()
    {
        bool isPlaying = GameManager.isDuringRun;

        CheckGrounded();

        ManageDriftState();

        HandleWheels();
        HandleSkidmarks();

        if (input.y > 0 && isPlaying && isGrounded)
        {
            Accelerate(input.y);
        }
        if (input.y < 0 && isPlaying && isGrounded)
        {
            Brake(-input.y);
        }
        if (isPlaying && isGrounded)
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
        if (!isGrounded && allowAirRoll)
        {
            DoAirRoll(input);
        }
        speedKmh = Mathf.RoundToInt(bodyRigidbody.linearVelocity.magnitude * 3.6f);
        speedKmhForward = Mathf.Abs(speedKmh);
        lastVelocity = bodyRigidbody.linearVelocity;
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
        if (wheels == null || wheels.Length == 0)
        {
            wheels = GetComponentsInChildren<VehicleWheel>();
        }
    }
#endif

    [Serializable]
    public struct AirTimeState
    {
        public AirTimeState(float timePassed, float distToGround, float maxDistToGround)
        {
            this.timePassed = timePassed;
            this.groundHeight = distToGround;
            this.maxGroundHeight = maxDistToGround;
        }

        public float timePassed;
        public float groundHeight;
        public float maxGroundHeight;
    }
}

