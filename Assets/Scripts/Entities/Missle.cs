using UnityEngine;

public class Missle : ProjectileEntity
{
    [Header("Visual Effects")]
    [SerializeField] Transform MissleModel;
    public float rotationSpeed = 10f;

    [Header("Behaviour")]
    public float distanceThreshold;
    public float farShift;
    public float closeShift;

    Vector2 v3xz(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    private void Update()
    {
        if (!isStationary) MissleModel.Rotate(Vector3.forward, rotationSpeed);
    }
    protected override void FixedUpdate()
    {
        localTargetShift.y = v3xz(transform.position - homingTarget.position).magnitude > distanceThreshold ?
            farShift : closeShift;

        base.FixedUpdate();
    }

    protected override void InternalExplode(ExplosionProperties explosionProps)
    {
        Kill();
        Debug.Log("Internal Explode");
        Destroy(gameObject);
    }

    bool expl = false;
    public void Explode()
    {
        if (expl) return;
        SelfExplode(1);
        expl = true;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
