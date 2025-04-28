using UnityEngine;

public class HelibossTargetController : MonoBehaviour
{
    public Missle assignedMissle;
    public float farShift;
    public float closeShift;
    public float distanceThreshold;

    Vector2 v3xz(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    private void Update()
    {
        if (!assignedMissle) return;

        Vector3 localPos = transform.localPosition;
        float distance = (v3xz(transform.position) - v3xz(assignedMissle.transform.position)).magnitude;
        localPos.y = distance < distanceThreshold ? closeShift : farShift;
        transform.localPosition = localPos;
    }
}
