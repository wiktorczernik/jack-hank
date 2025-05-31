using UnityEngine;

public class PointGizmoVisualization : MonoBehaviour
{
    [SerializeField] private DrawFigure drawFigure;
    [SerializeField] private DrawStart drawStart;
    [SerializeField] private float size;

    private readonly Vector3 _parallepipedSizes = new(2.5f, 2, 5);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var drawCenter = transform.position;

        if (drawStart == DrawStart.BackDownRight)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(size / 2, size / 2, size * 2);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(_parallepipedSizes.x * size / 2, _parallepipedSizes.y * size / 2,
                    _parallepipedSizes.z * size / 2);
        }
        else if (drawStart == DrawStart.BackDownLeft)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(size / 2, size / 2, size * -2);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(_parallepipedSizes.x * size / 2, _parallepipedSizes.y * size / 2,
                    _parallepipedSizes.z * size / -2);
        }
        else if (drawStart == DrawStart.BackDownCenter)
        {
            if (drawFigure == DrawFigure.Sphere)
                drawCenter += new Vector3(0, size / -2, 0);
            else if (drawFigure == DrawFigure.Parallelepiped)
                drawCenter += new Vector3(0, _parallepipedSizes.y * size / 2,
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
            DrawDirection(drawCenter, size * _parallepipedSizes.x / 2 + 0.2f);
        }
    }

    private void DrawSphere(Vector3 center)
    {
        Gizmos.DrawSphere(center, size);
    }

    private void DrawParallelepiped(Vector3 center)
    {
        var sizes = _parallepipedSizes * size / 2;
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
        Gizmos.DrawLine(center + transform.rotation * (Vector3.forward * offset),
            center + transform.rotation * (Vector3.forward * (offset + 5)));
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