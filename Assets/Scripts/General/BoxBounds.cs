using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxBounds
{
    public Vector2 start;
    public Vector2 size;

    public BoxBounds(Vector2 startPoint, Vector2 boxSize)
    {
        start = startPoint;
        size = boxSize;
    }

    public BoxBounds(Mesh mesh)
    {
        float startX = 0;
        float startY = 0;
        float endX = 0;
        float endY = 0;

        for(int v = 0; v < mesh.vertices.Length; v++)
        {
            if (mesh.vertices[v].x < startX) startX = mesh.vertices[v].x;
            if (mesh.vertices[v].y < startY) startX = mesh.vertices[v].y;
            if (mesh.vertices[v].x > endX) endX = mesh.vertices[v].x;
            if (mesh.vertices[v].y > endY) endY = mesh.vertices[v].y;
        }

        size = new Vector2(endX - startX, endY - startY);
        start = new Vector2(startX, startY);
    }

    public void MoveBuilding(Vector2 offset)
    {
        start += offset;
    }

    public BoxBounds Copy()
    {
        Vector2 startN = new Vector2(start.x, start.y);
        Vector2 sizeN = new Vector2(size.x, size.y);

        return new BoxBounds(startN, sizeN);
    }

    public bool PointWithinBounds(Vector2 point, float buffer)
    {
        return point.x >= start.x - buffer && point.x <= start.x + size.x + buffer &&
               point.y >= start.y - buffer && point.y <= start.y + size.y + buffer;
    }

    public bool BoxOverlap(BoxBounds other, float buffer)
    {
        return PointWithinBounds(other.UpperLeft, buffer) ||
            PointWithinBounds(other.LowerLeft, buffer) ||
            PointWithinBounds(other.UpperRight, buffer) ||
            PointWithinBounds(other.LowerRight, buffer) ||
            PointWithinBounds(other.Center, buffer) ||
            other.PointWithinBounds(UpperLeft, buffer) ||
            other.PointWithinBounds(LowerLeft, buffer) ||
            other.PointWithinBounds(UpperRight, buffer) ||
            other.PointWithinBounds(LowerRight, buffer) ||
            other.PointWithinBounds(Center, buffer);
    }

    public Vector2 UpperLeft
    {
        get
        {
            return new Vector2(start.x, start.y + size.y);
        }
    }

    public Vector2 UpperRight
    {
        get
        {
            return new Vector2(start.x + size.x, start.y + size.y);
        }
    }

    public Vector2 LowerLeft
    {
        get
        {
            return new Vector2(start.x, start.y);
        }
    }

    public Vector2 LowerRight
    {
        get
        {
            return new Vector2(start.x + size.x, start.y);
        }
    }

    public Vector2 Center
    {
        get
        {
            return new Vector2(start.x + (size.x / 2f), start.y + (size.y / 2f));
        }
    }
}
