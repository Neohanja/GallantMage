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
    Vector3[] norms;

    public MeshData()
    {
        Init();
    }

    public MeshData(float waterLevel)
    {
        WaterLevel(waterLevel);        
    }

    public MeshData(float[] points, float growth, float minHeight)
    {
        Init();
        RemapPoints(points, growth, minHeight);
    }

    void WaterLevel(float seaLevel)
    {
        int chunkSize = MeshSize - 1;
        int SqrCount = chunkSize * chunkSize;
        verts = new Vector3[SqrCount * 2];
        tris = new int[SqrCount * 3];
        uvMap = new Vector2[SqrCount * 2];
        int tIndex = 0, vIndex = 0;
        
        for (int y = 0; y < chunkSize; y+=2)
        {
            for (int x = 0; x < chunkSize; x += 2)
            {
                verts[vIndex] = new Vector3(x, seaLevel, y);
                verts[vIndex + 1] = new Vector3(x, seaLevel, y + 2);
                verts[vIndex + 2] = new Vector3(x + 2, seaLevel, y + 2);
                verts[vIndex + 3] = new Vector3(x + 2, seaLevel, y);

                uvMap[vIndex] = new Vector2(x, y);
                uvMap[vIndex + 1] = new Vector2(x, y + 1);
                uvMap[vIndex + 2] = new Vector2(x + 1, y + 1);
                uvMap[vIndex + 3] = new Vector2(x + 1, y);

                tris[tIndex] = vIndex;
                tris[tIndex + 1] = vIndex + 1;
                tris[tIndex + 2] = vIndex + 2;

                tris[tIndex + 3] = vIndex + 2;
                tris[tIndex + 4] = vIndex + 3;
                tris[tIndex + 5] = vIndex;

                tIndex += 6;
                vIndex += 4;
            }
        }
    }

    void Init()
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

    void CalculateNormals()
    {
        norms = new Vector3[verts.Length];
        int triCount = tris.Length / 3;

        for(int i = 0; i < triCount; i++)
        {
            int tIndex = i * 3;
            int vIndexA = tris[tIndex];
            int vIndexB = tris[tIndex + 1];
            int vIndexC = tris[tIndex + 2];

            Vector3 pointA = verts[vIndexA];
            Vector3 pointB = verts[vIndexB];
            Vector3 pointC = verts[vIndexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            Vector3 result = Vector3.Cross(sideAB, sideAC).normalized;

            norms[vIndexA] += result;
            norms[vIndexB] += result;
            norms[vIndexC] += result;
        }

        for(int i = 0;i< norms.Length; i++)
        {
            norms[i].Normalize();
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

    public void RemapPoints(float[] remap, float growth, float minHeight)
    {
        for (int y = 0; y < MeshSize; y++)
        {
            for (int x = 0; x < MeshSize; x++)
            {
                int vIndex = x + y * MeshSize;
                verts[vIndex].y = remap[vIndex] * growth + minHeight;
            }
        }
    }

    public Vector3[] GetNorms()
    {
        CalculateNormals();

        return norms;
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

        CalculateNormals();
        mesh.normals = norms;

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

    public float GetPoint(int x, int y)
    {
        return vals[x + y * MeshData.MeshSize] * MapManager.World.growth + MapManager.World.minHeight;
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