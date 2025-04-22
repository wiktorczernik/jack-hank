using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProjectileEntity), true)]
public class ProjectileEntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ProjectileEntity entity = (ProjectileEntity)target;

        using (var horizontalScope = new GUILayout.HorizontalScope("box"))
        {
            if (GUILayout.Button("Shoot"))
            {
                entity.Shoot();
            }

            if (GUILayout.Button("Halt"))
            {
                entity.Halt();
            }
        }
    }
}
