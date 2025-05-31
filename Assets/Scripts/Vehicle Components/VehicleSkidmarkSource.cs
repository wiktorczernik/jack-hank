using UnityEngine;

public class VehicleSkidmarkSource : MonoBehaviour
{
    public bool isEmitting => trailRenderer.emitting;

    [SerializeField] TrailRenderer trailRenderer;


    private void Awake()
    {
        trailRenderer.emitting = false;
    }

    public void StartEmitting()
    {
        if (isEmitting) return;
        trailRenderer.emitting = true;
    }
    public void StopEmitting()
    {
        if (!isEmitting) return;
        trailRenderer.emitting = false;
    }
}
