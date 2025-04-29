using UnityEngine;

public class PointGizmoVisualization : MonoBehaviour
{
    [SerializeField] private DrawFigure drawFigure;
    [SerializeField] private DrawStart drawStart;
    [SerializeField] private float size;

    private readonly Vector3 parallepipedSides = new(5, 2, 2.5f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var drawCenter = transform.position;

        if (drawStart == DrawStart.BackDownRight)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(size / 2, size / 2, size * 2);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(parallepipedSides.x * size / 2, parallepipedSides.y * size / 2,
                    parallepipedSides.z * size / 2);
        }
        else if (drawStart == DrawStart.BackDownLeft)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(size / 2, size / 2, size * -2);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(parallepipedSides.x * size / 2, parallepipedSides.y * size / 2,
                    parallepipedSides.z * size / -2);
        }
        else if (drawStart == DrawStart.BackDownCenter)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(0, size / -2, 0);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(0, parallepipedSides.y * size / 2,
                    0);
        }

        if (drawFigure == DrawFigure.Sphere)
        {
            DrawSphere(drawCenter);
            DrawDirection(drawCenter, size + 0.2f);
        }
        else if (drawFigure == DrawFigure.Parallelepiped)
        {
            DrawParallelepiped(drawCenter);
            DrawDirection(drawCenter, size * parallepipedSides.x / 2 + 0.2f);
        }
    }

    private void DrawSphere(Vector3 center)
    {
        Gizmos.DrawSphere(center, size);
    }

    private void DrawParallelepiped(Vector3 center)
    {
        var sizes = parallepipedSides * size / 2;
        var rotation = transform.rotation;

        for (var x = -1; x <= 1; x += 2)

        for (var y = -1; y <= 1; y += 2)

        for (var z = -1; z <= 1; z += 2)
        {
            var localOffset = new Vector3(sizes.x * x, sizes.y * y, sizes.z * z);
            var current = center + rotation * localOffset;

            if (x == -1)
            {
                var neighborOffset = new Vector3(sizes.x * -x, sizes.y * y, sizes.z * z);
                var neighbor = center + rotation * neighborOffset;
                Gizmos.DrawLine(current, neighbor);
            }

            if (y == -1)
            {
                var neighborOffset = new Vector3(sizes.x * x, sizes.y * -y, sizes.z * z);
                var neighbor = center + rotation * neighborOffset;
                Gizmos.DrawLine(current, neighbor);
            }

            if (z == -1)
            {
                var neighborOffset = new Vector3(sizes.x * x, sizes.y * y, sizes.z * -z);
                var neighbor = center + rotation * neighborOffset;
                Gizmos.DrawLine(current, neighbor);
            }
        }
    }

    private void DrawDirection(Vector3 center, float offset)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center + transform.rotation * (new Vector3(1, 0, 0) * offset),
            center + transform.rotation * (new Vector3(1, 0, 0) * (offset + 5)));
    }
}

internal enum DrawStart
{
    Center,
    BackDownLeft,
    BackDownRight,
    BackDownCenter
}

internal enum DrawFigure
{
    Parallelepiped,
    Sphere
}