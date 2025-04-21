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
        Debug.Log("Internal Explode");
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
