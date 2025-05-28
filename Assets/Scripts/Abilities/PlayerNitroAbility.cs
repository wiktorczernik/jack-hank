using UnityEngine;

public class PlayerNitroAbility : PlayerVehicleAbility
{
    [Header("Nitro Settings")]
    public float maxSpeed = 150f;
    public float accelerationForce = 150f;

    [Header("Nitro Effects")]
    [SerializeField] ParticleSystem[] particleVisuals;

    protected override void OnWorkTick()
    {
        var rigidbody = physics.bodyRigidbody;

        Vector3 nitroForce = rigidbody.transform.forward;
        nitroForce *= accelerationForce;

        float sp = Vector3.Dot(rigidbody.linearVelocity, nitroForce.normalized) * 3.6f;
        if (sp > maxSpeed)
        {
            return;
        }

        rigidbody.AddForce(nitroForce, ForceMode.Impulse);
    }

    public void ActivateNitroEffects()
    {
        foreach (ParticleSystem effect in particleVisuals) effect.Play();
    }

    public void DeactivateNitroEffects()
    {
        foreach (ParticleSystem effect in particleVisuals) effect.Stop();
    }
}
