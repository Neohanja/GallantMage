using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Comments signify things that need to be readded during specific testing in seperate environment
public class PolyBounds : MonoBehaviour
{
    public List<Transform> points;
    public List<Vector3> pointPlots;
    public Vector3 central;
    public float size;
    public float prevBuffer = Mathf.Infinity;
    public bool showDebug;

    //private void Awake()
    public void Init()
    {
        points = new List<Transform> ();
        BoundPoint[] t = GetComponentsInChildren<BoundPoint>();

        for (int i = 0; i < t.Length; i++)
        {
            points.Add (t[i].transform);
            t[i].GetComponent<MeshRenderer>().enabled = false;
        }
        if (points.Count > 2) CalcCenter(0f);
    }

    public List<Vector3> GetPoints()
    {
        if (points == null) Init();
        List<Vector3> p = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            p.Add (points[i].position);
        }
        p.Add(central);

        return p;
    }

    private void Update()
    {
        if (showDebug)
        {
            for (int p = 0; p < points.Count; p++)
            {
                Debug.DrawLine(points[p].position, points[(p + 1) % points.Count].position, Color.blue);
                Debug.DrawLine(points[p].position, central, Color.magenta);
            }
        }
    }

    public float Size()
    {
        float lX = float.MaxValue, rX = float.MinValue;
        float lY = float.MaxValue, uY = float.MinValue;

        for (int p = 0; p < points.Count; p++)
        {
            float x = points[p].position.x, y = points[p].position.z;

            if (x < lX) lX = x;
            if (x > rX) rX = x;
            if (y < lY) lY = y;
            if (y > uY) uY = y;
        }

        if (points != null && points.Count > 0)
            size = rX - lX > uY - lY ? rX - lX : uY - lY;
        else size = 0f;

        return size;
    }

    public void CalcCenter(float buffer)
    {
        if (points == null || points.Count < 3) return;
        if (buffer == prevBuffer) return;
        float cX = 0f, cY = 0f;
        // Save a lot of time, calculate at the start.
        for (int p = 0; p < points.Count; p++)
        {
            float x = points[p].position.x, y = points[p].position.z;

            cX += x;
            cY += y;
        }

        central = new Vector3(cX / points.Count, 0f, cY / points.Count);
        pointPlots = new List<Vector3>();

        cX = 0f;
        cY = 0f;

        for (int p = 0; p < points.Count; p++)
        {
            points[p].LookAt(new Vector3(central.x, points[p].position.y, central.z));
            pointPlots.Add(points[p].position + points[p].forward * -buffer);
            cX += pointPlots[p].x;
            cY += pointPlots[p].z;
        }
        central = new Vector3(cX / pointPlots.Count, points[0].position.y, cY / pointPlots.Count);
        prevBuffer = buffer;
    }

    public bool TriPointWithin(Vector3 c, float buffer)
    {
        // No points to calculate
        if (points == null) Init();
        if (points == null || points.Count < 3) return false;

        CalcCenter(buffer);
        // Within the radius
        if (Vector3.Distance(central, c) > Size() + buffer) return false;

        LineData[] triLine = new LineData[3];

        for(int a = 0; a < points.Count; a++)
        {
            for (int b = 0; b < points.Count; b++)
            {
                if (a == b) continue;

                triLine[0] = new LineData(pointPlots[a], pointPlots[b], central);
                triLine[1] = new LineData(pointPlots[b], central, pointPlots[a]);
                triLine[2] = new LineData(central, pointPlots[a], pointPlots[b]);
                if (AnotherPointWithin(triLine, a, b)) continue;

                if (triLine[0].CorrectSide(c) &&
                    triLine[1].CorrectSide(c) &&
                    triLine[2].CorrectSide(c))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AnotherPointWithin(LineData[] triangle, int a, int b)
    {
        for (int p = 0; p < pointPlots.Count; p++)
        {
            if (p == a || p == b) continue;
            bool found = true;
            for(int l = 0; l < triangle.Length; l++)
            {
                if (!triangle[l].CorrectSide(pointPlots[p])) found = false;
            }
            if (found) return true;
        }

        return false;
    }

    public class LineData
    {
        public Vector2 point;
        public double slope;
        public double intercept;
        public bool lessThan;

        public LineData()
        {
            point = Vector2.zero;
            slope = 0;
            intercept = 0;
            lessThan = false;
        }

        public LineData(Vector3 a, Vector3 b, Vector3 c)
        {
            float x1 = a.x;
            float y1 = a.z;
            float x2 = b.x;
            float y2 = b.z;
            float x3 = c.x;
            float y3 = c.z;

            point = new Vector2(x1, y1);
            slope = (y2 - y1) / (x2 - x1);
            intercept = y1 - (slope * x1);

            lessThan = y3 < slope * x3 + intercept;
        }

        public bool CorrectSide(Vector3 a)
        {
            float x = a.x;
            float y = a.z;

            if (lessThan)
            {
                return y <= slope * x + intercept;
            }
            else
            {
                return y >= slope * x + intercept;
            }
        }
    }
}