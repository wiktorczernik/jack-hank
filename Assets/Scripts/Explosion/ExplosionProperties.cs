using System;
using UnityEngine;

[Serializable]
public struct ExplosionProperties
{
    public ExplosionProperties(Transform epicenterTransform, float force, float epicenterRadius, float shakeIntensity, float shakeMaxDistance, float damage, GameEntity initiator = null)
    {
        this._epicenterParentTransform = epicenterTransform;
        this._epicenterPosition = Vector3.zero;
        this.force = force;
        this.epicenterRadius = epicenterRadius;
        this.shakeIntensity = shakeIntensity;
        this.shakeMaxDistance = shakeMaxDistance;
        this.initiator = initiator;
        this.damage = damage;
        this.damageFalloff = new AnimationCurve();
    }
    public ExplosionProperties(Vector3 epicenterPosition, float force, float epicenterRadius, float shakeIntensity, float shakeMaxDistance, float damage, GameEntity initiator = null)
    {
        this._epicenterParentTransform = null;
        this._epicenterPosition = epicenterPosition;
        this.force = force;
        this.epicenterRadius = epicenterRadius;
        this.shakeIntensity = shakeIntensity;
        this.shakeMaxDistance = shakeMaxDistance;
        this.initiator = initiator;
        this.damage = damage;
        this.damageFalloff = new AnimationCurve();
    }

    /// <summary>
    /// Transform of epicenter. Will override epicenterPosition if set
    /// </summary>
    [SerializeField] Transform _epicenterParentTransform;
    /// <summary>
    /// World position of epicenter
    /// </summary>
    [SerializeField] Vector3 _epicenterPosition;

    /// <summary>
    /// World position of epicenter
    /// </summary>
    public Vector3 epicenterPosition
    {
        get => _epicenterParentTransform == null ? _epicenterPosition : _epicenterParentTransform.position;
    }
    public GameEntity initiator;
    /// <summary>
    /// Explosion force that objects within epicenter will feel
    /// </summary>
    public float force;
    /// <summary>
    /// Radius of explosion epicenter (zone of destruction)
    /// </summary>
    public float epicenterRadius;
    /// <summary>
    /// Intensity of camera shake
    /// </summary>
    public float shakeIntensity;
    /// <summary>
    /// Maximum distance where player camera can still feel explosion shake
    /// </summary>
    public float shakeMaxDistance;

    public float damage;
    public AnimationCurve damageFalloff;
}
