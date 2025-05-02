#if UNITY_EDITOR
using System.Text;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseColliderEventEmitter<T> : MonoBehaviour where T : class
{
    public UnityEvent<T> OnEnter;
    public UnityEvent<T> OnStay;
    public UnityEvent<T> OnExit;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        StringBuilder nameBuilder = new StringBuilder();

        foreach (Collider col in colliders)
        {
            nameBuilder.Clear();

            Color color = col.isTrigger ? Color.yellow : Color.green;

            Gizmos.color = color;
            Gizmos.matrix = col.transform.localToWorldMatrix;
            
            nameBuilder.Append(col.name);
            nameBuilder.Append(" ");
            nameBuilder.Append(col.isTrigger ? "(Trigger)" : "(Collider)");
            GUIContent labelContent = new GUIContent(nameBuilder.ToString());

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = color;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            Handles.Label(col.bounds.center, labelContent, labelStyle);
            if (col is BoxCollider box)
            {
                DrawBoxCollider(box);
            }
            else if (col is SphereCollider sphere)
            {
                DrawSphereCollider(sphere);
            }
            else if (col is CapsuleCollider capsule)
            {
                DrawCapsuleCollider(capsule);
            }
        }
    }
    void DrawBoxCollider(BoxCollider box)
    {
        Gizmos.DrawWireCube(box.center, box.size);
    }

    void DrawSphereCollider(SphereCollider sphere)
    {
        Gizmos.DrawWireSphere(sphere.center, sphere.radius);
    }

    void DrawCapsuleCollider(CapsuleCollider capsule)
    {
        Gizmos.DrawWireSphere(capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius), capsule.radius);
        Gizmos.DrawWireSphere(capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius), capsule.radius);
    }
#endif
}
