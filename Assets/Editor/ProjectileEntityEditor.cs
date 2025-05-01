using UnityEngine;
using UnityEditor;


namespace JackHank.EditorUtils
{
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
}