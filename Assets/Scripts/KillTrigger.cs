using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable, RequireComponent(typeof(TriggerEventEmitter))]
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class KillTrigger : MonoBehaviour
{
    [Header("Behaviour")]
    public float HalfHeight = 1f;
    public float Radius = 0.5f;
    [Tooltip("Does the size change linearly from one KillTrigger to another")] public bool adapt;
    [Range(3, 32), Tooltip("The number of verices int the bases of KillTrigger column")]
    public int meshPolygonPrecision = 12;

    [SerializeField] TriggerEventEmitter emitter;
    [SerializeField] List<MeshCollider> triggers = new List<MeshCollider>();
    List<Mesh> gizmosMeshes = new List<Mesh>();
    [SerializeField] public KillTrigger[] ChildNodes;
    [NonSerialized] private List<KillTrigger> oldNodes;

    public GameObject prefab;
    [NonSerialized] public Action forceUpdate;

    [Header("Connections")]
    public bool makeClone;


    private void Awake()
    {
        emitter.OnEnter.AddListener(TryKillCollider);

        void TryKillCollider(Collider c)
        {
            if (c.isTrigger) return;

            Vehicle v = c.GetComponentInParent<Vehicle>();
            if (!v) return;

            v.Kill();
        }

#if UNITY_EDITOR
        OnValidate();
#endif
    }


#if UNITY_EDITOR
    Vector3 lastPos = Vector3.zero;
    bool needForReload = false;
    private void Update()
    {
        if (lastPos != transform.position) OnValidate();
        lastPos = transform.position;

        if (!needForReload) return;

        triggers = new List<MeshCollider>(GetComponents<MeshCollider>());
        int i = 0;
        foreach (var trigger in triggers)
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            DestroyImmediate(trigger);
            i++;
        }
        UpdateMesh();

        needForReload = false;
    }
#endif


#if UNITY_EDITOR
    public void OnValidate()
    {
        emitter = GetComponent<TriggerEventEmitter>();

        CheckForSelf();

        needForReload = true;

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

        oldNodes = new List<KillTrigger>(ChildNodes);

        return;
        // UnityEditorInternal.ComponentUtility.MoveComponentUp(target_component);
        foreach (var trigger in triggers)
        {
            int targetIdx = gameObject.GetComponentIndex(trigger) - 1;
            for (; targetIdx > 0; targetIdx--) UnityEditorInternal.ComponentUtility.MoveComponentUp(trigger);
        }
    }
#endif
    bool looking = false;
    void CheckForSelf()
    {
        looking = true;
        CheckForTrigger(this);
    }
    public void RevertBack()
    {
        ChildNodes = oldNodes.ToArray();
    }
    void CheckForTrigger(KillTrigger trigger)
    {
        if (!trigger.looking) return;
        if (new List<KillTrigger>(ChildNodes).Contains(trigger))
        {
            trigger.looking = false;
            Debug.LogError("A reference loop was tried to be made. That is not allowed!");
            trigger.RevertBack();
            return;
        }

        foreach (var child in ChildNodes)
        {
            if (child == null) continue;
            child.GetComponent<KillTrigger>().CheckForTrigger(trigger);
            if (!trigger.looking) return;
        }
    }
    public void UpdateMesh()
    {
        gizmosMeshes.Clear();

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

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        triggers[0].sharedMesh = mesh;

        giz.vertices = gizmos.ToArray();
        giz.triangles = indices.ToArray();
        giz.RecalculateBounds();
        giz.RecalculateNormals();
        gizmosMeshes.Add(giz);

        foreach (KillTrigger trigger in ChildNodes)
        {
            if (!trigger) continue;

            Vector3 dir = trigger.transform.position - transform.position;
            if (dir.magnitude <= trigger.Radius + Radius) continue;

            vertices.Clear();
            gizmos.Clear();
            indices.Clear();
            Mesh bridge = new Mesh();
            Mesh gizmo = new Mesh();

            Vector3 fwd = dir.normalized;
            Vector3 right = Vector3.Cross(fwd, Vector3.up).normalized;

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
            gizmos.Add(-fwd * trigger.Radius + trigger.transform.position + right * radius + Vector3.up * hheight);
            gizmos.Add(-fwd * trigger.Radius + trigger.transform.position + right * radius - Vector3.up * hheight);
            gizmos.Add(-fwd * trigger.Radius + trigger.transform.position - right * radius - Vector3.up * hheight);
            gizmos.Add(-fwd * trigger.Radius + trigger.transform.position - right * radius + Vector3.up * hheight);
            vertices.Add(fwd * Radius + right * Radius + Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius + right * Radius - Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius - right * Radius - Vector3.up * HalfHeight);
            vertices.Add(fwd * Radius - right * Radius + Vector3.up * HalfHeight);
            vertices.Add(-fwd * trigger.Radius + dir + right * radius + Vector3.up * hheight);
            vertices.Add(-fwd * trigger.Radius + dir + right * radius - Vector3.up * hheight);
            vertices.Add(-fwd * trigger.Radius + dir - right * radius - Vector3.up * hheight);
            vertices.Add(-fwd * trigger.Radius + dir - right * radius + Vector3.up * hheight);

            indices.Add(0);
            indices.Add(2);
            indices.Add(1); //
            indices.Add(0);
            indices.Add(3);
            indices.Add(2); //
            indices.Add(0);
            indices.Add(1);
            indices.Add(5); //
            indices.Add(0);
            indices.Add(5);
            indices.Add(4); //
            indices.Add(0);
            indices.Add(4);
            indices.Add(7); //
            indices.Add(0);
            indices.Add(7);
            indices.Add(3); //
            indices.Add(1);
            indices.Add(2);
            indices.Add(6); //
            indices.Add(1);
            indices.Add(6);
            indices.Add(5); //
            indices.Add(2);
            indices.Add(3);
            indices.Add(6); //
            indices.Add(6);
            indices.Add(3);
            indices.Add(7); //
            indices.Add(5);
            indices.Add(6);
            indices.Add(4); //
            indices.Add(6);
            indices.Add(7);
            indices.Add(4); //
            #endregion

            bridge.vertices = vertices.ToArray();
            bridge.triangles = indices.ToArray();
            bridge.RecalculateBounds();
            bridge.RecalculateNormals();
            OrderMesh(bridge);

            gizmo.vertices = gizmos.ToArray();
            gizmo.triangles = indices.ToArray();
            gizmo.RecalculateBounds();
            gizmo.RecalculateNormals();
            gizmosMeshes.Add(gizmo);
        }
    }
    private void OrderMesh(Mesh mesh)
    {
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.convex = true;
        mc.isTrigger = true;
        mc.sharedMesh = mesh;
        triggers.Add(mc);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        if (Selection.activeObject == gameObject) Gizmos.color = new Color(0f, 1f, 0f, 1f);
        foreach (Mesh mesh in gizmosMeshes) Gizmos.DrawWireMesh(mesh);
    }
#endif

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
            node.meshPolygonPrecision = meshPolygonPrecision;
        }

        ChildNodes = new List<KillTrigger>(ChildNodes) { node }.ToArray();
#if UNITY_EDITOR
        OnValidate();
#endif

        return newNode;
    }
    public void Cleanup()
    {
        var list = new List<KillTrigger>(ChildNodes);
        list.RemoveAll(b => b == null);
        ChildNodes = list.ToArray();
#if UNITY_EDITOR
        OnValidate();
#endif
    }
}
