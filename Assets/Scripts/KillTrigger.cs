using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable, ExecuteInEditMode, RequireComponent(typeof(TriggerEventEmitter)), RequireComponent(typeof(MeshCollider))]
public class KillTrigger : MonoBehaviour
{
    [Header("Behaviour")]
    public float HalfHeight = 1f;
    public float Radius = 0.5f;
    [Tooltip("Does the size change linearly from one KillTrigger to another")] public bool adapt;
    [Range(3, 32), Tooltip("The number of verices int the bases of KillTrigger column")]
    public int meshPolygonPrecision = 12;

    [SerializeField] TriggerEventEmitter emitter;
    [SerializeField] MeshCollider trigger;
    Mesh colliderMesh;
    Mesh gizmosMesh;
    [SerializeField] public KillTrigger[] ChildNodes;

    public GameObject prefab;
    [NonSerialized] public Action forceUpdate;

    [Header("Connections")]
    public bool makeClone;


    private void Awake()
    {
        emitter.OnEnter.AddListener(TryKillCollider);

        void TryKillCollider(Collider c)
        {
            Vehicle v = c.GetComponentInParent<Vehicle>();
            if (!v) return;

            v.Kill();
        }

        OnValidate();
    }


    Vector3 lastPos = Vector3.zero;
    private void Update()
    {
        if (lastPos != transform.position) OnValidate();
        lastPos = transform.position;
    }



    public void OnValidate()
    {
        emitter = GetComponent<TriggerEventEmitter>();
        trigger = GetComponent<MeshCollider>();

        trigger.convex = true;
        trigger.isTrigger = true;
        UpdateMesh();

        trigger.sharedMesh = colliderMesh;

        CheckForSelf();

        foreach (var child in ChildNodes)
        {
            if (!child) continue;

            KillTrigger kt = child.GetComponent<KillTrigger>();

            try { kt.forceUpdate -= OnValidate; }
            finally
            {
                kt.forceUpdate += OnValidate;
            }
        }
        forceUpdate?.Invoke();
    }
    bool looking = false;
    void CheckForSelf()
    {
        looking = true;
        CheckForTrigger(this);
    }
    void CheckForTrigger(KillTrigger trigger)
    {
        if (!trigger.looking) return;
        if (new List<KillTrigger>(ChildNodes).Contains(trigger))
        {
            looking = false;
            Debug.LogError("A reference loop was tried to be made. That is not allowed!");
            Undo.PerformUndo();
            return;
        }

        foreach (var child in ChildNodes)
        {
            if (child == null) continue;
            child.GetComponent<KillTrigger>().CheckForTrigger(trigger);
        }
    }
    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        Mesh giz = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> gizmos = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int i = 0; i < 2 * meshPolygonPrecision; i++)
        {
            float angle = 2 * Mathf.PI * i / meshPolygonPrecision;
            Vector3 pos = new Vector3(Radius * Mathf.Cos(angle),
                i >= meshPolygonPrecision ? -HalfHeight : HalfHeight,
                Radius * Mathf.Sin(angle));
            vertices.Add(pos);
            gizmos.Add(pos + transform.position);
        }
        for (int i = 0; i <= meshPolygonPrecision - 3; i++)
        {
            indices.Add(0);
            indices.Add(i + 2);
            indices.Add(i + 1);
        }
        for (int i = 0; i <= meshPolygonPrecision - 3; i++)
        {
            indices.Add(meshPolygonPrecision);
            indices.Add(meshPolygonPrecision + i + 1);
            indices.Add(meshPolygonPrecision + i + 2);
        }
        for (int i = 0; i < meshPolygonPrecision - 1; i++)
        {
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(i + meshPolygonPrecision);
            indices.Add(i + 1);
            indices.Add(i + meshPolygonPrecision + 1);
            indices.Add(i + meshPolygonPrecision);
        }
        indices.Add(meshPolygonPrecision - 1);
        indices.Add(0);
        indices.Add(2 * meshPolygonPrecision - 1);
        indices.Add(0);
        indices.Add(meshPolygonPrecision);
        indices.Add(2 * meshPolygonPrecision - 1);

        foreach (KillTrigger trigger in ChildNodes)
        {
            Vector3 dir = trigger.transform.position - transform.position;
            if (dir.magnitude <= trigger.Radius + Radius) continue;

            int vertBaseIdx = vertices.Count;

            Vector3 fwd = dir.normalized;
            Vector3 right = Quaternion.FromToRotation(Vector3.forward, Vector3.right) * fwd;

            //     i+7 -- i+4
            //    / |    / |
            // i+3 -|- i   |
            //  |  i+6 |- i+5
            //  | /    | / 
            // i+2 -- i+1 
            #region boring vert assignment
            gizmos.Add(fwd * Radius + transform.position + right * Radius + Vector3.up * HalfHeight);
            gizmos.Add(fwd * Radius + transform.position + right * Radius - Vector3.up * HalfHeight);
            gizmos.Add(fwd * Radius + transform.position - right * Radius - Vector3.up * HalfHeight);
            gizmos.Add(fwd * Radius + transform.position - right * Radius + Vector3.up * HalfHeight);
            float hheight = adapt ? trigger.HalfHeight : HalfHeight;
            float radius = adapt ? trigger.Radius : Radius;
            gizmos.Add(-fwd * radius + trigger.transform.position + right * radius + Vector3.up * hheight);
            gizmos.Add(-fwd * radius + trigger.transform.position + right * radius - Vector3.up * hheight);
            gizmos.Add(-fwd * radius + trigger.transform.position - right * radius - Vector3.up * hheight);
            gizmos.Add(-fwd * radius + trigger.transform.position - right * radius + Vector3.up * hheight);
            vertices.Add(fwd * Radius + right * Radius + Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius + right * Radius - Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius - right * Radius - Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius - right * Radius + Vector3.up * HalfHeight);
            vertices.Add(-fwd * radius + dir + right * radius + Vector3.up * hheight);
            vertices.Add(-fwd * radius + dir + right * radius - Vector3.up * hheight);
            vertices.Add(-fwd * radius + dir - right * radius - Vector3.up * hheight);
            vertices.Add(-fwd * radius + dir - right * radius + Vector3.up * hheight);

            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 1);
            indices.Add(vertBaseIdx + 2);
            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 2);
            indices.Add(vertBaseIdx + 3);
            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 5);
            indices.Add(vertBaseIdx + 1);
            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 4);
            indices.Add(vertBaseIdx + 5);
            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 7);
            indices.Add(vertBaseIdx + 4);
            indices.Add(vertBaseIdx);
            indices.Add(vertBaseIdx + 3);
            indices.Add(vertBaseIdx + 7);
            indices.Add(vertBaseIdx + 1);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 2);
            indices.Add(vertBaseIdx + 1);
            indices.Add(vertBaseIdx + 5);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 2);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 3);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 7);
            indices.Add(vertBaseIdx + 3);
            indices.Add(vertBaseIdx + 5);
            indices.Add(vertBaseIdx + 4);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 6);
            indices.Add(vertBaseIdx + 4);
            indices.Add(vertBaseIdx + 7);
            #endregion
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        giz.vertices = gizmos.ToArray();
        giz.triangles = indices.ToArray();
        giz.RecalculateBounds();
        giz.RecalculateNormals();

        colliderMesh = mesh;
        gizmosMesh = giz;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        if (Selection.activeObject == gameObject) Gizmos.color = new Color(0f, 1f, 1f, 1f);
        Gizmos.DrawWireMesh(gizmosMesh);
    }

    public GameObject MakeNewConnection()
    {
        Transform parent = transform;
        if (transform.parent && transform.parent.TryGetComponent(out KillTrigger _)) parent = transform.parent;

        GameObject newNode = Instantiate(prefab, parent);
        newNode.transform.position = transform.position + Vector3.forward * Radius;

        newNode.gameObject.name = "KillTriggerNode";

        KillTrigger node = newNode.GetComponent<KillTrigger>();

        if (makeClone)
        {
            node.makeClone = true;
            node.HalfHeight = HalfHeight;
            node.Radius = Radius;
        }

        ChildNodes = new List<KillTrigger>(ChildNodes) { node }.ToArray();
        OnValidate();

        return newNode;
    }
    public void Cleanup()
    {
        var list = new List<KillTrigger>(ChildNodes);
        list.RemoveAll(b => b == null);
        ChildNodes = list.ToArray();
        OnValidate();
    }
}
