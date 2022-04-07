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
    Vector2 chunkIndex;
    RanGen chunkPRG;

    public Chunk(Vector2 chunkCoord, ChunkData chunkInfo)
    {
        chunkPRG = new RanGen(RanGen.PullNumber(MapManager.World.seed, MathFun.Floor(chunkCoord.x), MathFun.Floor(chunkCoord.y)));
        chunkIndex = chunkCoord;

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
        if(Flora.TreeMaker != null)
        {
            BuildTreeScatter();
        }

        MeshData meshData = new MeshData(chunkData.GetPoints(), MapManager.World.growth, MapManager.World.minHeight);
        chunkFilter.mesh = meshData.GetMesh();

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

    public void BuildTreeScatter()
    {
        int treePop = chunkPRG.Roll(225, 650);
        int maxTries = 100000;
        List<Vector3> points = new List<Vector3>();

        for(int t = 0; t < maxTries; t++)
        {
            if (points.Count >= treePop) break;

            float x = chunkPRG.Roll(0, ChunkSize) + chunkPRG.Percent();
            float z = chunkPRG.Roll(0, ChunkSize) + chunkPRG.Percent();

            Vector2 treeLoc = new Vector2(x, z);

            float y = GetHeight(treeLoc);

            if(y > MapManager.World.seaLevel)
            {
                bool canPlace = true;
                foreach(Vector3 pt in points)
                {
                    if (Vector2.Distance(treeLoc, new Vector2(pt.x, pt.z)) <= 0.75f)
                    {
                        canPlace = false;
                        break;
                    }
                }

                if(canPlace)
                {
                    points.Add(new Vector3(x + chunkIndex.x - HalfMap, y, z + chunkIndex.y - HalfMap));
                }
            }
        }

        Flora.TreeMaker.AddTreePoints(points, new Vector2(0.5f, 1.5f));
    }

    public float GetHeight(Vector2 point)
    {
        if (point.x < 0 || point.x >= ChunkSize || point.y < 0 || point.y >= ChunkSize) return -256f;

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