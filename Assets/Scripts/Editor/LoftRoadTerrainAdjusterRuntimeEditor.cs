using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

[CustomEditor(typeof(LoftRoadTerrainAdjusterRuntime))]
public class LoftRoadTerrainAdjusterRuntimeEditor : Editor
{
    LoftRoadTerrainAdjusterRuntimeEditor editor;

    public void OnEnable()
    {
        this.editor = this;

        LoftRoadTerrainAdjusterRuntime targetGameObject = (LoftRoadTerrainAdjusterRuntime)target;

        if (targetGameObject.splineContainer != null)
        {
            targetGameObject.splineContainer.Spline.changed -= OnPathChanged;
            targetGameObject.splineContainer.Spline.changed += OnPathChanged;
        }
        if (targetGameObject.loftRoad != null)
        {
            targetGameObject.loftRoad.OnLofted -= OnPathChanged;
            targetGameObject.loftRoad.OnLofted += OnPathChanged;
        }

        targetGameObject.SaveOriginalTerrainHeights();
    }


    void OnDisable()
    {
        LoftRoadTerrainAdjusterRuntime targetGameObject = (LoftRoadTerrainAdjusterRuntime)target;

        // remove original terrain data
        targetGameObject.CleanUp();

        // remove existing listeners
        if (targetGameObject.splineContainer != null)
        {
            targetGameObject.splineContainer.Spline.changed -= OnPathChanged;
        }
        if (targetGameObject.loftRoad != null)
        {
            targetGameObject.loftRoad.OnLofted -= OnPathChanged;
        }

    }


    void OnPathChanged()
    {
        LoftRoadTerrainAdjusterRuntime targetGameObject = (LoftRoadTerrainAdjusterRuntime)target;

        targetGameObject.ShapeTerrain();
    }

    public override void OnInspectorGUI()
    {
        LoftRoadTerrainAdjusterRuntime targetGameObject = (LoftRoadTerrainAdjusterRuntime)target;

        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            // if anything (eg falloff) changed recreate the terrain under the path
            OnPathChanged();
        }

        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("Flatten entire terrain"))
        {
            SetTerrainHeight(targetGameObject.terrain, 0f);
        }

        EditorGUILayout.EndVertical();
    }

    void SetTerrainHeight(Terrain terrain, float height)
    {
        TerrainData terrainData = terrain.terrainData;

        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;
        float[,] allHeights = terrainData.GetHeights(0, 0, w, h);

        float terrainMin = terrain.transform.position.y + 0f;
        float terrainMax = terrain.transform.position.y + terrain.terrainData.size.y;
        float totalHeight = terrainMax - terrainMin;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                allHeights[y, x] = 0f;
            }
        }

        terrain.terrainData.SetHeights(0, 0, allHeights);
    }

}