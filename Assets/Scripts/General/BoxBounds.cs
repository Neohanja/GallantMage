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

    public bool PointWithinBounds(Vector2 point, float buffer)
    {
        return point.x >= start.x - buffer && point.x <= start.x + size.x + buffer &&
               point.y >= start.y - buffer && point.y <= start.y + size.y + buffer;
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
            return new Vector2(start.x + size.x / 2, start.y + size.y / 2);
        }
    }
}
