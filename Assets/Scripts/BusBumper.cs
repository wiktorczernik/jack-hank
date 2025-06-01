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

        if (smashable && smashable.bumpable && !smashable.wasHit)
        {
            smashable.ForceHit();

            Vector3 originalAngles = transform.localEulerAngles;
            Vector3 newAngles = originalAngles;
            newAngles.x = Random.Range(-180, 0);

            transform.localEulerAngles = newAngles;

            float multiplier = Mathf.Clamp01(vehicle.speedKmhForward / bumpMaxDesiredSpeed);
            
            smashable.usedRigidbody.AddForce(transform.forward * bumpSpeed * multiplier, ForceMode.VelocityChange);

            transform.localEulerAngles = originalAngles;
        }
    }
}
