using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KillTrigger))]
public class KillTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        KillTrigger trigger = (KillTrigger)target;

        GUILayoutOption height = GUILayout.Height(30f);

        using (var horizontalScope = new GUILayout.HorizontalScope("box", height))
        {
            if (GUILayout.Button("Reload", height))
            {
                trigger.OnValidate();
            }
            if (GUILayout.Button("New Connection", height))
            {
                GameObject conn = trigger.MakeNewConnection();
                Selection.activeObject = conn;
            }
            if (GUILayout.Button("Remove Null", height))
            {
                trigger.Cleanup();
            }
        }

        EditorUtility.SetDirty(trigger);
    }

    private void OnSceneGUI()
    {
        KillTrigger trigger = (KillTrigger)target;

        Handles.color = new Color(1f, 0.5f, 0f, 1f);
        EditorGUI.BeginChangeCheck();

        float newRadius = Handles.RadiusHandle(Quaternion.identity, trigger.transform.position, trigger.Radius);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(trigger, "Changed node connector trigger radius");
            trigger.Radius = newRadius;

            trigger.OnValidate();
        }

        EditorGUI.BeginChangeCheck();

        Vector3 pos = trigger.transform.position + Vector3.up * trigger.HalfHeight;
        Vector3 newPos = Handles.Slider(pos, Vector3.up);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(trigger, "Changed node connector trigger height");
            trigger.HalfHeight += Vector3.Dot(pos - newPos, Vector3.down);

            trigger.OnValidate();
        }
    }
}
