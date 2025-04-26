using UnityEngine;

public class AmmoCrateBillboardEffect : MonoBehaviour
{
    [SerializeField] Transform Particles;

    private void Update()
    {
        Particles.LookAt(Camera.main.transform);
    }
}
