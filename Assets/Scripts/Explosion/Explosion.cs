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

    public void Init()
    {
        visuals.Init();
        Collider[] colliders = Physics.OverlapSphere(transform.position, properties.shakeMaxDistance);
        foreach(var collider in colliders)
        {
            Vector3 cPosition = collider.transform.position;
            float distance = Vector3.Distance(cPosition, transform.position);

            SmashableEntity smashable;

            smashable = collider.GetComponent<SmashableEntity>();
            if (!smashable)
                smashable = collider.GetComponentInParent<SmashableEntity>();

            if (smashable)
            {
                if (distance > properties.epicenterRadius) continue;
                smashable.Explode(properties);
                continue;
            }

            PlayerVehicle pVehicle = collider.GetComponentInParent<PlayerVehicle>();

            if (pVehicle)
            {
                if (distance > properties.shakeMaxDistance) continue;
                pVehicle.NotifyExplosionNearby(properties);
            }
        }
    }
}
