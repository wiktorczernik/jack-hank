using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ExplosionVisuals visuals;

    public ExplosionProperties properties = new ExplosionProperties() { 
        force = 500f,
        epicenterRadius = 10f,
        shakeMaxDistance = 100f,
        shakeIntensity = 0.5f
    };
    [Header("Audio")]
    public EventReference audioEventRef;

    public void Init()
    {
        visuals.Init();
        Collider[] colliders = Physics.OverlapSphere(transform.position, properties.shakeMaxDistance);
        List<Vehicle> affectedVehicles = new List<Vehicle>();
        foreach(var collider in colliders)
        {
            RuntimeManager.PlayOneShot(audioEventRef);

            Vector3 cPosition = collider.transform.position;
            float distance = Vector3.Distance(cPosition, transform.position);

            SmashableEntity smashable;

            smashable = collider.GetComponent<SmashableEntity>();
            if (!smashable)
                smashable = collider.GetComponentInParent<SmashableEntity>();

            if (smashable)
            {
                if (distance > properties.epicenterRadius) continue;
                smashable.Hurt(properties.damage * properties.damageFalloff.Evaluate(distance / properties.epicenterRadius));
                smashable.Explode(properties);
                continue;
            }

            Vehicle vehicle = collider.GetComponentInParent<Vehicle>();

            if (vehicle && !affectedVehicles.Contains(vehicle))
            {
                if (distance > properties.shakeMaxDistance) continue;
                vehicle.Hurt(properties.damage * properties.damageFalloff.Evaluate(distance / properties.epicenterRadius));
                //Debug.Log(properties.damage * properties.damageFalloff.Evaluate(distance / properties.epicenterRadius) + " " + properties.damageFalloff.Evaluate(distance / properties.epicenterRadius));
                affectedVehicles.Add(vehicle);

                if (vehicle is PlayerVehicle) ((PlayerVehicle)vehicle).NotifyExplosionNearby(properties);
            }
        }
    }
}
