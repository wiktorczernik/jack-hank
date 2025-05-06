using System.Collections.Generic;
using UnityEngine;

public class FarmField : MonoBehaviour
{
    public enum DestructionEnum : byte
    {
        MinorHarm = 1,
        MajorHarm = 5
    }

    [SerializeField] Transform Model;
    [SerializeField] ParticleSystem Major;
    [SerializeField] ParticleSystem Minor;

    [Tooltip("In km/h")] public float majorDestructionSpeed = 30f;
    public float majorDestructionSpeedDist = 1.5f;
    public float majorDestructionDist = 0.7f;
    public float hitTickPeriod = 1f;
    public int destructionLevel { get; private set; }

    float tickTimer = 0f;

    public void OnEnter(Collider collider)
    {
        Transform tracked = collider.transform.parent;
        if (!tracked) return;
        if (!tracked.TryGetComponent(out Vehicle vehicle)) return;

        Hit(vehicle);
    }
    public void OnStay(Collider collider)
    {
        Transform tracked = collider.transform.parent;
        if (!tracked) return;
        if (!tracked.TryGetComponent(out Vehicle vehicle)) return;

        if (tickTimer >= hitTickPeriod) Hit(vehicle);
        else tickTimer += Time.deltaTime;
    }
    public void OnLeave(Collider collider)
    {
        Transform tracked = collider.transform.parent;
        if (!tracked) return;
        if (!tracked.TryGetComponent(out Vehicle vehicle)) return;

        tickTimer = 0f;
    }


    public void Hit(Vehicle vehicle)
    {
        tickTimer = 0f;

        if (vehicle.physics.bodyRigidbody.linearVelocity.magnitude * 3.6f > majorDestructionSpeed
            && (vehicle.transform.position - transform.position).magnitude <= majorDestructionSpeedDist
            || (vehicle.transform.position - transform.position).magnitude <= majorDestructionDist)
            DestroyField(DestructionEnum.MajorHarm);
        else DestroyField(DestructionEnum.MinorHarm);
    }

    public void DestroyField(DestructionEnum harm)
    {
        if (destructionLevel == 5) return;

        destructionLevel += (int)harm;
        destructionLevel = Mathf.Clamp(destructionLevel, 0, 5);

        if (destructionLevel >= (int)DestructionEnum.MajorHarm)
        {
            Model.localScale = new Vector3(0.8f, 0.05f, 0.8f);
            Major.Play();
        }
        else if (destructionLevel >= (int)DestructionEnum.MinorHarm)
        {
            Model.localScale = new Vector3(0.9f, Model.localScale.y * 0.6f, 0.9f);
            Minor.Play();
        }
    }
}
