using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class TerrainAdjusterRuntime : MonoBehaviour
{
    public Terrain terrain;

    [Range(0f, 1f)]
    public float brushFallOff = 0.3f;

    [Range(1f, 10f)]
    public float brushSpacing = 1f;

    [HideInInspector]
    public SplineContainer splineContainer;

    private float[,] originalTerrainHeights;

    // the blur radius values being used for the various passes
    public int[] initialPassRadii = { 15, 7, 2 };

    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();

        if (splineContainer == null)
            Debug.LogError("Script must be attached to a PathCreator GameObject");

    }

    public void SaveOriginalTerrainHeights()
    {
        if (terrain == null || splineContainer == null)
            return;

        Debug.Log("Saving original terrain data");

        TerrainData terrainData = terrain.terrainData;

        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;

        originalTerrainHeights = terrainData.GetHeights(0, 0, w, h);

    }

    public void CleanUp()
    {
        originalTerrainHeights = null;
        Debug.Log("Deleting original terrain data");
    }

    public void ShapeTerrain()
    {
        if (terrain == null || splineContainer == null)
            return;

        // save original terrain in case the terrain got added later
        if (originalTerrainHeights == null)
            SaveOriginalTerrainHeights();

        Vector3 terrainPosition = terrain.gameObject.transform.position;
        TerrainData terrainData = terrain.terrainData;

        // both GetHeights and SetHeights use normalized height values, where 0.0 equals to terrain.transform.position.y in the world space and 1.0 equals to terrain.transform.position.y + terrain.terrainData.size.y in the world space
        // so when using GetHeight you have to manually divide the value by the Terrain.activeTerrain.terrainData.size.y which is the configured height ("Terrain Height") of the terrain.
        float terrainMin = terrain.transform.position.y + 0f;
        float terrainMax = terrain.transform.position.y + terrain.terrainData.size.y;
        float totalHeight = terrainMax - terrainMin;

        // clone the original data, the modifications along the path are based on them
        float[,] allHeights = originalTerrainHeights.Clone() as float[,];

        // the blur radius values being used for the various passes
        for (int pass = 0; pass < initialPassRadii.Length; pass++)
        {
            int radius = initialPassRadii[pass];

            // points as vertices, not equi-distant
            // Vector3[] vertexPoints = splineContainer.Spline. .vertices;

            // equi-distant points
            List<Vector3> distancePoints = new List<Vector3>();

            Spline spline = splineContainer.Spline;
            Vector3 lastPoint = Vector3.zero;
            float splineLength = spline.GetLength();

            for (float t = 0; t <= splineLength; t += brushSpacing)
            {
                float clampedDistance = Mathf.Clamp(t, 0f, splineLength);
                Vector3 point = splineContainer.transform.TransformPoint(SplineUtility.EvaluatePosition(spline, clampedDistance / splineLength));
                distancePoints.Add(point);
            }

            // sort by height reverse
            // sequential height raising would just lead to irregularities, ie when a higher point follows a lower point
            // we need to proceed from top to bottom height
            distancePoints.Sort((a, b) => -a.y.CompareTo(b.y));

            Vector3[] points = distancePoints.ToArray();

            foreach (var point in points)
            {

                float targetHeight;

                int centerX;
                int centerY;

                GetPositionHeightmapInfo(point, out centerX, out centerY, out targetHeight);

                AdjustTerrain(allHeights, radius, centerY, centerX, targetHeight);

            }
        }

        terrain.terrainData.SetHeights(0, 0, allHeights);
    }

    private void GetPositionHeightmapInfo(Vector3 worldPosition, out int xIndex, out int zIndex, out float targetHeight)
    {
        if (terrain == null)
        {
            Debug.LogError("No active terrain found!");
            xIndex = -1;
            zIndex = -1;
            targetHeight = -1;
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        Vector3 terrainPosition = terrain.transform.position;
        Vector3 localPosition = worldPosition - terrainPosition;

        float normalizedX = localPosition.x / terrainData.size.x;
        float normalizedZ = localPosition.z / terrainData.size.z;

        xIndex = Mathf.FloorToInt(normalizedX * (terrainData.heightmapResolution - 1));
        zIndex = Mathf.FloorToInt(normalizedZ * (terrainData.heightmapResolution - 1));

        xIndex = Mathf.Clamp(xIndex, 0, terrainData.heightmapResolution - 1);
        zIndex = Mathf.Clamp(zIndex, 0, terrainData.heightmapResolution - 1);

        float normalizedHeight = (worldPosition.y - 0.05f - terrainPosition.y) / terrainData.size.y;
        normalizedHeight = Mathf.Clamp01(normalizedHeight);

        targetHeight = normalizedHeight;
    }

    private void AdjustTerrain(float[,] heightMap, int radius, int centerX, int centerY, float targetHeight)
    {
        float deltaHeight = targetHeight;
        int sqrRadius = radius * radius;

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int offsetY = -radius; offsetY <= radius; offsetY++)
        {
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                int sqrDstFromCenter = offsetX * offsetX + offsetY * offsetY;

                // check if point is inside brush radius
                if (sqrDstFromCenter <= sqrRadius)
                {
                    // calculate brush weight with exponential falloff from center
                    float dstFromCenter = Mathf.Sqrt(sqrDstFromCenter);
                    float t = dstFromCenter / radius;
                    float brushWeight = Mathf.Exp(-t * t / brushFallOff);

                    // raise terrain
                    int brushX = centerX + offsetX;
                    int brushY = centerY + offsetY;

                    if (brushX >= 0 && brushY >= 0 && brushX < width && brushY < height)
                    {
                        heightMap[brushX, brushY] += deltaHeight * brushWeight;

                        // clamp the height
                        if (heightMap[brushX, brushY] > targetHeight)
                        {
                            heightMap[brushX, brushY] = targetHeight;
                        }
                    }
                }
            }
        }
    }

}
