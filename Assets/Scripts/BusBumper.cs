using UnityEngine;

public class BusBumper : MonoBehaviour
{
    public TriggerEventEmitter bumperTrigger;
    public VehiclePhysics vehicle;
    public float bumpSpeed = 25f;
    public float bumpMaxDesiredSpeed = 70f;


    private void OnEnable()
    {
        bumperTrigger.OnEnter.AddListener(OnObjectEnter);
    }
    private void OnDisable()
    {
        bumperTrigger.OnEnter.RemoveListener(OnObjectEnter);
    }


    private void OnObjectEnter(Collider collider)
    {
        SmashableEntity smashable = collider.GetComponentInParent<SmashableEntity>();

        if (smashable)
        {
            smashable.ForceHit();

            Vector3 originalAngles = transform.localEulerAngles;
            Vector3 newAngles = originalAngles;
            
            float randomAngle = newAngles.x = Random.Range(-45, 0);
            if (Random.Range(1, 2) == 2)
                newAngles.x = 0 + randomAngle;
            else
                newAngles.x = -180 - randomAngle;

            transform.localEulerAngles = newAngles;

            float multiplier = Mathf.Clamp01(vehicle.speedKmhForward / bumpMaxDesiredSpeed);

            var rigidbody = smashable.usedRigidbody;
            smashable.usedRigidbody.AddForce(transform.forward * bumpSpeed * multiplier, ForceMode.VelocityChange);

            transform.localEulerAngles = originalAngles;
        }
    }
}
