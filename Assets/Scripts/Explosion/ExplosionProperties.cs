using System;
using UnityEngine;

[Serializable]
public struct ExplosionProperties
{
    public ExplosionProperties(Transform epicenterTransform, float force, float epicenterRadius, float shakeIntensity, float shakeMaxDistance)
    {
        this._epicenterParentTransform = epicenterTransform;
        this._epicenterPosition = Vector3.zero;
        this.force = force;
        this.epicenterRadius = epicenterRadius;
        this.shakeIntensity = shakeIntensity;
        this.shakeMaxDistance = shakeMaxDistance;
    }
    public ExplosionProperties(Vector3 epicenterPosition, float force, float epicenterRadius, float shakeIntensity, float shakeMaxDistance)
    {
        this._epicenterParentTransform = null;
        this._epicenterPosition = epicenterPosition;
        this.force = force;
        this.epicenterRadius = epicenterRadius;
        this.shakeIntensity = shakeIntensity;
        this.shakeMaxDistance = shakeMaxDistance;
    }

    [SerializeField] Transform _epicenterParentTransform;
    [SerializeField] Vector3 _epicenterPosition;

    public Vector3 epicenterPosition
    {
        get => _epicenterParentTransform == null ? _epicenterPosition : _epicenterParentTransform.position;
    }
    public float force;
    public float epicenterRadius;
    public float shakeIntensity;
    public float shakeMaxDistance;
}
