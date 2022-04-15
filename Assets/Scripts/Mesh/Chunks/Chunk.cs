using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    static readonly int MaxTries = 100000;
    // Towns
    static readonly float MaxTownAltitude = 15f;
    static readonly int MinTownSize = 15;
    static readonly int MaxTownSize = 30;

    //Chunk Terrain
    GameObject chunkObj;
    MeshFilter chunkFilter;
    MeshRenderer chunkRender;
    MeshData chunkMesh;
    MeshCollider chunkCollider;

    //Water Terrain
    GameObject waterObj;
    MeshFilter waterFilter;
    MeshRenderer waterRender;
    MeshData waterMesh;

    // Data and Randomness
    ChunkData chunkData;
    BoxBounds chunkBounds;
    public bool townExists;
    public BoxBounds townBounds;
    public RanGen chunkPRG;
    List<BoxBounds> townBuildings;

    public Chunk(Vector2 chunkCoord, ChunkData chunkInfo)
    {
        chunkPRG = new RanGen(RanGen.PullNumber(MapManager.World.seed, MathFun.Floor(chunkCoord.x), MathFun.Floor(chunkCoord.y)));

        chunkData = chunkInfo;
        townExists = false;

        chunkObj = new GameObject("Chunk");
        chunkFilter = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();
        chunkCollider = chunkObj.AddComponent<MeshCollider>();

        if (MapManager.World != null)
        {
            chunkRender.material = MapManager.World.terrainMat;
            chunkObj.transform.SetParent(MapManager.World.transform);
            CreateWater();
        }

        // Make Chunk
        chunkMesh = new MeshData(chunkData.GetPoints(), MapManager.World.growth, MapManager.World.minHeight);
        chunkFilter.mesh = chunkMesh.GetMesh();
        chunkCollider.sharedMesh = chunkFilter.mesh;
        chunkObj.transform.position = new Vector3(chunkCoord.x - HalfMap, 0f, chunkCoord.y - HalfMap);
        chunkBounds = new BoxBounds(new Vector2(chunkCoord.x - HalfMap, chunkCoord.y - HalfMap), Vector2.one * (MeshData.MeshSize - 1));

        // Populate Chunks
        PopTown();

        if(ClutterBuilder.Helper != null)
        {
            ClutterBuilder.Helper.BuildTrees(this);
        }
    }

    public Transform GetChunkTransform() { return chunkObj.transform; }
    public Vector2 ChunkLocV2() { return new Vector2(chunkObj.transform.position.x, chunkObj.transform.position.z); }
    public Vector3 ChunkLocV3() { return chunkObj.transform.position; }

    public void CreateWater()
    {
        waterObj = new GameObject("Water");
        waterFilter = waterObj.AddComponent<MeshFilter>();
        waterRender = waterObj.AddComponent<MeshRenderer>();
        waterObj.transform.SetParent(chunkObj.transform);
        waterRender.material = MapManager.World.waterMat;
        waterMesh = new MeshData(MapManager.World.seaLevel);
        waterFilter.mesh = waterMesh.GetMesh();
        waterRender.receiveShadows = false;
    }

    public void PopTown()
    {
        if (TownBuilder.Helper == null) return;
        Vector2Int townStart, townSize;
        int tries = 0;

        do
        {
            if (tries >= MaxTries) return;
            tries++;

            int sizeX = chunkPRG.Roll(MinTownSize, MaxTownSize);
            int sizeY = chunkPRG.Roll(MinTownSize, MaxTownSize);
            townSize = new Vector2Int(sizeX, sizeY);
            int startX = chunkPRG.Roll(2, ChunkSize - 3 - sizeX);
            int startY = chunkPRG.Roll(2, ChunkSize - 3 - sizeY);
            townStart = new Vector2Int(startX, startY);
        } while(!IsAreaClear(townStart, townSize));

        FlattenLand(townStart - Vector2Int.one * 2, townSize + Vector2Int.one * 2);
        townBounds = new BoxBounds(townStart, townSize);
        townExists = true;
        townBuildings = TownBuilder.Helper.BuildTown(townBounds, chunkPRG, this);

        float xSpawn, zSpawn;
        do
        {
            xSpawn = chunkPRG.Roll(-townSize.x / 4, townSize.x / 4) + townBounds.Center.x;
            zSpawn = chunkPRG.Roll(-townSize.y / 4, townSize.y / 4) + townBounds.Center.y;
        } while (InBuilding(new Vector2(xSpawn, zSpawn), 0.5f));

        float ySpawn = GetHeight(new Vector2(xSpawn, zSpawn));
        Vector3 townCenter = new Vector3(xSpawn, ySpawn, zSpawn);
        MapManager.World.AddSpawnPoint(ChunkOffset + townCenter);
        TraceRoads();

        chunkFilter.mesh = chunkMesh.GetMesh();
        chunkCollider.sharedMesh = chunkFilter.mesh;
    }

    public void TraceRoads()
    {
        int centerX = MathFun.Round(townBounds.Center.x);
        int centerY = MathFun.Round(townBounds.Center.y);

        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                chunkMesh.PaintPoint(centerX + x, centerY + y, new Color(1f, 1f, 1f, 1f));
            }
        }

        for (int i = 1; i < townBuildings.Count; i++)
        {
            int trailX = MathFun.Round(townBuildings[i].door.x);
            int trailY = MathFun.Round(townBuildings[i].door.y);
            while (trailX != centerX || trailY != centerY)
            {
                chunkMesh.PaintPoint(trailX, trailY, new Color(1f, 1f, 1f, 1f));
                if (trailX < centerX)
                {
                    chunkMesh.PaintPoint(trailX, trailY + 1, new Color(1f, 1f, 1f, 1f));
                    chunkMesh.PaintPoint(trailX, trailY - 1, new Color(1f, 1f, 1f, 1f));
                    trailX++;
                }
                else if (trailX > centerX)
                {
                    chunkMesh.PaintPoint(trailX, trailY + 1, new Color(1f, 1f, 1f, 1f));
                    chunkMesh.PaintPoint(trailX, trailY - 1, new Color(1f, 1f, 1f, 1f));
                    trailX--;
                }
                if (trailY < centerY)
                {
                    chunkMesh.PaintPoint(trailX + 1, trailY, new Color(1f, 1f, 1f, 1f));
                    chunkMesh.PaintPoint(trailX - 1, trailY, new Color(1f, 1f, 1f, 1f));
                    trailY++;
                }
                else if (trailY > centerY)
                {

                    chunkMesh.PaintPoint(trailX + 1, trailY, new Color(1f, 1f, 1f, 1f));
                    chunkMesh.PaintPoint(trailX - 1, trailY, new Color(1f, 1f, 1f, 1f));
                    trailY--;
                }
            }
        }
    }

    public bool InBuilding(Vector2 point, float distTolerance)
    {
        foreach(BoxBounds building in townBuildings)
        {
            if (building.PointWithinBounds(point, distTolerance)) return true;
        }

        return false;
    }

    public void FlattenLand(Vector2Int start, Vector2Int size)
    {
        int count = 0;
        float avg = 0f;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2 point = new Vector2(start.x + x, start.y + y);
                avg += GetHeight(point);
                count++;
            }
        }

        avg /= count;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2 point = new Vector2(start.x + x, start.y + y);
                chunkMesh.RemapPoint(MathFun.Round(point.x), MathFun.Round(point.y), avg);
            }
        }

        chunkFilter.mesh = chunkMesh.GetMesh();
        chunkCollider.sharedMesh = chunkFilter.mesh;
    }

    public bool IsAreaClear(Vector2 point, Vector2 size)
    {
        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (GetHeight(point + new Vector2(x, y)) <= SeaLevel)
                    return false;
                if (GetHeight(point + new Vector2(x, y)) >= MaxTownAltitude)
                    return false;
            }
        }

        return true;
    }

    public float GetHeight(Vector2 point)
    {
        if (point.x < 0 || point.x >= ChunkSize || point.y < 0 || point.y >= ChunkSize) return -256f;

        int iX = MathFun.Floor(point.x);
        int iY = MathFun.Floor(point.y);

        float midX = point.x - iX;
        float midY = point.y - iY;

        float a = MathFun.Lerp(chunkMesh.GetPoint(iX, iY), chunkMesh.GetPoint(iX + 1, iY), midX);
        //float a = MathFun.Lerp(chunkData.GetPoint(iX, iY), chunkData.GetPoint(iX + 1, iY), midX);
        float b = MathFun.Lerp(chunkMesh.GetPoint(iX, iY + 1), chunkMesh.GetPoint(iX + 1, iY + 1), midX);
        //float b = MathFun.Lerp(chunkData.GetPoint(iX, iY + 1), chunkData.GetPoint(iX + 1, iY + 1), midX);

        return MathFun.Lerp(a, b, midY);
    }

    public static float HalfMap
    {
        get
        {
            return (MeshData.MeshSize - 1) / 2f;
        }
    }

    public static float SeaLevel { get { return MapManager.World.seaLevel; } }

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

    public int ChunkSize { get { return MeshData.MeshSize - 1; } }

    public Vector3 ChunkOffset { get { return chunkObj.transform.position; } }
}