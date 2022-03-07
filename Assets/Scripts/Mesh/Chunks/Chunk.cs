using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    //Chunk Terrain
    GameObject chunkObj;
    MeshFilter chunkFilter;
    MeshRenderer chunkRender;
    MeshData chunkMesh;

    //Water Terrain
    GameObject waterObj;
    MeshFilter waterFilter;
    MeshRenderer waterRender;
    MeshData waterMesh;

    ChunkData chunkData;
    BoxBounds chunkBounds;
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
            CreateWater();
        }

        MeshData meshData = new MeshData(chunkData.GetPoints(), MapManager.World.growth, MapManager.World.minHeight);
        chunkFilter.mesh = meshData.GetMesh();

        chunkRender.material.mainTexture = 
            MapTexture.TextureByHeight(MeshData.MeshSize, MapManager.World.colorScale,
            chunkData.GetPoints(), MapManager.World.heightGrad);

        chunkObj.transform.position = new Vector3(chunkCoord.x - HalfMap, 0f, chunkCoord.y - HalfMap);

        chunkBounds = new BoxBounds(new Vector2(chunkCoord.x - HalfMap, chunkCoord.y - HalfMap), Vector2.one * (MeshData.MeshSize - 1));
    }

    public void CreateWater()
    {
        waterObj = new GameObject("Water");
        waterFilter = waterObj.AddComponent<MeshFilter>();
        waterRender = waterObj.AddComponent<MeshRenderer>();
        waterObj.transform.SetParent(chunkObj.transform);
        waterRender.material = MapManager.World.waterMat;
        waterMesh = new MeshData(MapManager.World.seaLevel);
        waterFilter.mesh = waterMesh.GetMesh();
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

    public bool CheckViewDistance(float distance, Vector2 position, bool correctPos = true)
    {
        if(correctPos)
        {
            position.x += HalfMap;
            position.y += HalfMap;
        }

        bool checkDist = chunkBounds.PointWithinBounds(position, distance);

        if (checkDist && !chunkObj.activeSelf) chunkObj.SetActive(true);
        else if (!checkDist && chunkObj.activeSelf) chunkObj.SetActive(false);

        return checkDist;
    }

    int ChunkSize { get { return MeshData.MeshSize - 1; } }
}