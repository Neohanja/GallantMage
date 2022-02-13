using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    GameObject chunkObj;
    MeshFilter chunkFilter;
    MeshRenderer chunkRender;
    MeshData chunkMesh;

    ChunkData chunkData;
    Vector2 chunkCoord;

    public Chunk(Vector2 chunkCoord, ChunkData chunkInfo)
    {
        chunkData = chunkInfo;

        chunkObj = new GameObject("Chunk");
        chunkFilter = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();

        if (MapManager.World != null)
        {
            chunkRender.material = MapManager.World.terrainMat;
            chunkObj.transform.SetParent(MapManager.World.transform);
        }

        MeshData meshData = new MeshData(chunkData.GetPoints());
        chunkFilter.mesh = meshData.GetMesh();

        chunkObj.transform.position = new Vector3(chunkCoord.x - HalfMap, 0f, chunkCoord.y - HalfMap);
    }

    public float GetHeight(Vector2 point)
    {
        if (point.x < 0 || point.x >= ChunkSize || point.y < 0 || point.y >= ChunkSize) return 0f;

        int iX = MathFun.Floor(point.x);
        int iY = MathFun.Floor(point.y);

        float midX = point.x - iX;
        float midY = point.y - iY;

        float a = MathFun.Lerp(chunkData.GetPoint(iX, iY), chunkData.GetPoint(iX + 1, iY), midX);
        float b = MathFun.Lerp(chunkData.GetPoint(iX, iY + 1), chunkData.GetPoint(iX + 1, iY + 1), midX);

        return MathFun.Lerp(a, b, midY);
    }

    public static float HalfMap
    {
        get
        {
            return (MeshData.MeshSize - 1) / 2f;
        }
    }

    int ChunkSize { get { return MeshData.MeshSize - 1; } }
}
