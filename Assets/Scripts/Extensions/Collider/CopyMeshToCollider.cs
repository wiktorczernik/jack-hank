using UnityEngine;

public class CopyMeshToCollider : MonoBehaviour
{
    private void Awake()
    {
        Mesh m = GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshCollider>().sharedMesh = m;
    }
}
