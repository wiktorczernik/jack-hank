using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ExplosionVisuals visuals;

    public float force = 500f;
    public float impactRadius = 10f;
    public float busDistance = 100f;
    public float intensity = 0.5f;


    public void Init()
    {
        visuals.Init();
        Collider[] colliders = Physics.OverlapSphere(transform.position, busDistance);
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
                if (distance > impactRadius) continue;
                smashable.AddExplosionForce(transform.position, force, impactRadius);
                continue;
            }

            PlayerVehicle pVehicle = collider.GetComponentInParent<PlayerVehicle>();

            if (pVehicle)
            {
                if (distance > busDistance) continue;
                pVehicle.NotifyExplosionNearby(intensity, distance, impactRadius);
            }
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Init();
        }
    }
}
