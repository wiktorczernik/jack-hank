using UnityEngine;

public class Missle : ProjectileEntity
{
    [Header("Visual Effects")]
    [SerializeField] Transform MissleModel;
    public float rotationSpeed = 10f;

    private void Update()
    {
        if (!isStationary) MissleModel.Rotate(Vector3.forward, rotationSpeed);
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
