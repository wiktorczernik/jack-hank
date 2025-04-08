using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Explosion State")]
    public bool exploded = false;

    [Header("Explosion Settings")]
    public bool canBeExploded = false;
    public bool canSelfExplode = false;
    public ExplosionProperties selfExplosionProps = new ExplosionProperties()
    {
        force = 500f,
        epicenterRadius = 10f,
        shakeMaxDistance = 100f,
        shakeIntensity = 0.5f
    };

    public event Action<ExplosionProperties> onSelfExplode;
    public event Action<ExplosionProperties> onExplode;


    public void SelfExplode()
    {
        if (!canSelfExplode) return;
        ExplosionMaster.Create(selfExplosionProps);
        InternalSelfExplode();
        onSelfExplode?.Invoke(selfExplosionProps);
        exploded = true;
    }
    public void Explode(ExplosionProperties explosionProps)
    {
        if (!canBeExploded) return;
        InternalExplode(explosionProps);
        onExplode?.Invoke(explosionProps);
        exploded = true;
    }

    protected virtual void InternalSelfExplode() { }
    protected virtual void InternalExplode(ExplosionProperties explosionProps) { }
}
