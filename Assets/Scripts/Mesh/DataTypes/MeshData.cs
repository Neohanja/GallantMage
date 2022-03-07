using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    public static int MeshSize = 241;

    Vector3[] verts;
    int[] tris;
    Vector2[] uvMap;

    public MeshData()
    {
        Init();
    }

    public MeshData(float waterLevel)
    {
        Init(true);
        float[] points = new float[MeshSize * MeshSize];
        for(int i = 0; i < points.Length; i++)
        {
            points[i] = waterLevel;
        }
        RemapPoints(points);
    }

    public MeshData(float[] points)
    {
        Init();
        RemapPoints(points);
    }

    void Init(bool isWater = false)
    {
        verts = new Vector3[MeshSize * MeshSize];
        tris = new int[(MeshSize - 1) * (MeshSize - 1) * 6];
        uvMap = new Vector2[MeshSize * MeshSize];

        int tIndex = 0;
        for (int y = 0; y < MeshSize; y++)
        {
            for (int x = 0; x < MeshSize; x++)
            {
                int vIndex = x + y * MeshSize;
                verts[vIndex] = new Vector3(x, 0, y);
                if (isWater)
                    uvMap[vIndex] = new Vector2(x % 2, y % 2);
                else
                    uvMap[vIndex] = new Vector2(x / (float)MeshSize, y / (float)MeshSize);

                if (x < MeshSize - 1 && y < MeshSize - 1)
                {
                    tris[tIndex] = vIndex;
                    tris[tIndex + 1] = vIndex + MeshSize;
                    tris[tIndex + 2] = vIndex + MeshSize + 1;

                    tris[tIndex + 3] = vIndex + MeshSize + 1;
                    tris[tIndex + 4] = vIndex + 1;
                    tris[tIndex + 5] = vIndex;
                    tIndex += 6;
                }
            }
        }
    }

    public int Index(int x, int y)
    {
        return x + y * MeshSize;
    }

    public float[] HeightList()
    {
        float[] map = new float[MeshSize * MeshSize];

        for (int y = 0; y < MeshSize; y++)
        {
            for (int x = 0; x < MeshSize; x++)
            {
                int vIndex = x + y * MeshSize;
                map[vIndex] = verts[vIndex].y;
            }
        }

        return map;
    }

    public void RemapPoints(float[] remap)
    {
        for (int y = 0; y < MeshSize; y++)
        {
            for (int x = 0; x < MeshSize; x++)
            {
                int vIndex = x + y * MeshSize;
                verts[vIndex].y = remap[vIndex];
            }
        }
    }

    public void RemapPoints(List<Vector3> remap)
    {
        foreach(Vector3 point in remap)
        {
            if (point.x < 0 || point.x >= MeshSize || point.y < 0 || point.y >= MeshSize) continue;
            int x = Mathf.FloorToInt(point.x);
            int y = Mathf.FloorToInt(point.z);
            verts[Index(x, y)].y = point.y;
        }
    }

    public void RemapPoint(int x, int y, float val)
    {
        if (x < 0 || x >= MeshSize || y < 0 || y >= MeshSize) return;
        verts[Index(x, y)].y = val;
    }

    public Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uvMap;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;
    }
}

[System.Serializable]
public class ChunkData
{
    private float[] vals;

    public ChunkData()
    {
        vals = new float[MeshData.MeshSize * MeshData.MeshSize];
        for (int i = 0; i < vals.Length; i++)
        {
            vals[i] = 0f;
        }
    }

    public ChunkData(float[] transfer)
    {
        vals = transfer;
    }

    public float[] GetPoints()
    {
        return vals;
    }

    public float[] GetPointsRaw()
    {
        float[] rawVals = new float[vals.Length];

        for(int i = 0; i < vals.Length; i++)
        {
            rawVals[i] = Mathf.InverseLerp(32, 288, vals[i]);// (vals[i] - 32) / 256f;
        }

        return rawVals;
    }

    public float GetPoint(int x, int y)
    {
        return vals[x + y * MeshData.MeshSize];
    }

    public float[] CopyPoints()
    {
        float[] newVals = new float[vals.Length];
        for(int i = 0; i < vals.Length; i++)
        {
            newVals[i] = vals[i];
        }
        return newVals;
    }

    public void SetPoints(float[] points)
    {
        vals = points;
    }
}