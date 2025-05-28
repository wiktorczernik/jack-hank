using UnityEngine;

public class AmmoCrateBillboardEffect : MonoBehaviour
{
    [SerializeField] Transform Particles;

    private void Update()
    {
        if (!Camera.main) return;
        Particles.LookAt(Camera.main.transform);
    }
}
