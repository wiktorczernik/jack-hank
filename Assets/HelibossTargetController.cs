using UnityEngine;

public class HelibossTargetController : MonoBehaviour
{
    public Missle assignedMissle;
    public float farShift;
    public float closeShift;

    float CurveEasing(float x, float start, float finish)
    {
        return 1 / (1 + Mathf.Exp(10 / (start - finish) * (x - (start + finish) / 2)));
    }
    private void Update()
    {
        Vector3 localPos = transform.localPosition;
        float distance = (transform.position - assignedMissle.transform.position).magnitude;
        float val = CurveEasing(distance, 4f, 0.5f);
        localPos.y = closeShift * val + farShift * (1 - val);
    }
}
