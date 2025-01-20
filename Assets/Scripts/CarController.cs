using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public int speedKmh => Mathf.RoundToInt(BodyRigidbody.linearVelocity.magnitude * 3.6f);

    public Rigidbody BodyRigidbody => bodyRigidbody;
    public Transform CenterOfMass;

    [Header("Wheels")]
    public CarWheel[] wheels;

    [Header("Settings")]
    public float MaxSpeed = 100;
    public float Acceleration = 1700f; 
    public float BrakeForce = 1000f;
    public float TurnForce = 1500f;
    public float DriftForce = 5000f;
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
    public void Accelerate(float force = 1.0f)
    {
        float sp = Vector3.Dot(BodyRigidbody.linearVelocity, transform.forward) * 3.6f;
        if (sp > MaxSpeed)
        {
            Debug.Log("Capped front");
            return;
        }
        force = Mathf.Clamp01(force);
        BodyRigidbody.AddForceAtPosition(transform.forward * Acceleration * force, CenterOfMass.position);
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
        BodyRigidbody.AddTorque(transform.up * input * TurnForce * sp);
    }
    void HandleWheels()
    {
        foreach(CarWheel wheel in wheels)
        {
            wheel.ApplyFriction();
        }
    }

    private void FixedUpdate()
    {
        HandleWheels();


        if (moveInput.y > 0)
        {
            Accelerate(moveInput.y);
        }
        else
        {
            Brake(moveInput.y);
        }
        if (moveInput.x != 0)
        {
            DoTurn(moveInput.x);
        }
    }

    void Awake()
    {
        bodyRigidbody = GetComponent<Rigidbody>();
        bodyRigidbody.maxLinearVelocity = 150f;
    }


    Rigidbody bodyRigidbody;
}
