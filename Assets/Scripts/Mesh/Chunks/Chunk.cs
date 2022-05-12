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
    public bool townExists { private set; get; }
    public BoxBounds townBounds;
    public RanGen chunkRNG;
    List<BuildingData> townBuildings;

    public Chunk(Vector2Int chunkCoord, ChunkData chunkInfo)
    {
        chunkRNG = new RanGen(RanGen.PullNumber(World.Map.seed, chunkCoord.x, chunkCoord.y));

        chunkData = chunkInfo;
        townExists = false;

        chunkObj = new GameObject("Chunk");
        chunkFilter = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();
        chunkCollider = chunkObj.AddComponent<MeshCollider>();

        if (World.Map != null)
        {
            chunkRender.material = World.Map.terrainMat;
            chunkObj.transform.SetParent(World.Map.transform);
            CreateWater();
        }

        // Make Chunk
        chunkMesh = new MeshData(chunkData.GetPoints(), World.Map.growth, World.Map.minHeight);
        chunkFilter.mesh = chunkMesh.GetMesh();
        chunkCollider.sharedMesh = chunkFilter.mesh;


        chunkObj.transform.position = new Vector3(chunkCoord.x, 0f, chunkCoord.y);
        chunkBounds = new BoxBounds(new Vector2(chunkCoord.x, chunkCoord.y), Vector2.one * (MeshData.MeshSize - 1));

        if (ClutterBuilder.Generator != null)
        {
            ClutterBuilder.Generator.BuildEnvironmentClutter(this);
        }
    }

    #region Random Number Gen for chunk Items
    public Vector2 RandomSpotInChunk(bool waterPlacement)
    {
        float x, y;

        do
        {
            x = RandomIndex(ChunkSize) + Percent();
            y = RandomIndex(ChunkSize) + Percent();
        } while (GetHeight(new Vector2(x, y)) <= SeaLevel || waterPlacement);

        return new Vector2(x, y);
    }
    public float Percent()
    {
        return chunkRNG.Percent();
    }
    public int Roll(int min, int max)
    {
        return chunkRNG.Roll(min, max);
    }
    public int RandomIndex(int count)
    {
        return chunkRNG.Roll(0, count - 1);
    }
    #endregion

    public void CreateWater()
    {
        waterObj = new GameObject("Water");
        waterFilter = waterObj.AddComponent<MeshFilter>();
        waterRender = waterObj.AddComponent<MeshRenderer>();
        waterObj.transform.SetParent(chunkObj.transform);
        waterRender.material = World.Map.waterMat;
        waterMesh = new MeshData(World.Map.seaLevel);
        waterFilter.mesh = waterMesh.GetMesh();
        waterRender.receiveShadows = false;
    }

    // To Do: Rewrite Town Building (and place in Town Builder to clean up chunks)
    #region Town Building
    public void PopTown()
    {
        if (TownBuilder.Helper == null) return;
        Vector2Int townStart, townSize;
        int tries = 0;

        do
        {
            if (tries >= MaxTries) return;
            tries++;

            int sizeX = chunkRNG.Roll(MinTownSize, MaxTownSize);
            int sizeY = chunkRNG.Roll(MinTownSize, MaxTownSize);
            townSize = new Vector2Int(sizeX, sizeY);
            int startX = chunkRNG.Roll(2, ChunkSize - 3 - sizeX);
            int startY = chunkRNG.Roll(2, ChunkSize - 3 - sizeY);
            townStart = new Vector2Int(startX, startY);
        } while (!IsAreaClear(townStart, townSize));

        FlattenLand(townStart - Vector2Int.one * 2, townSize + Vector2Int.one * 2);
        townBounds = new BoxBounds(townStart, townSize);
        townExists = true;
        townBuildings = TownBuilder.Helper.BuildTown(townBounds, chunkRNG, this);

        float xSpawn, zSpawn;
        do
        {
            xSpawn = chunkRNG.Roll(-townSize.x / 4, townSize.x / 4) + townBounds.Center.x;
            zSpawn = chunkRNG.Roll(-townSize.y / 4, townSize.y / 4) + townBounds.Center.y;
        } while (TownBuilder.Helper.InBuilding(new Vector2(xSpawn, zSpawn)));
        
        float ySpawn = GetHeight(new Vector2(xSpawn, zSpawn));
        Vector3 townCenter = new Vector3(xSpawn + 0.5f, ySpawn, zSpawn + 0.5f);
        World.Map.AddSpawnPoint(ChunkOffset + townCenter);

        TraceRoads();

        chunkFilter.mesh = chunkMesh.GetMesh();
        chunkCollider.sharedMesh = chunkFilter.mesh;
    }

    public void TraceRoads()
    {
        // return;
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
            int trailX = MathFun.Round(townBuildings[i].DoorLoc(true).x);
            int trailY = MathFun.Round(townBuildings[i].DoorLoc(true).y);
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
    #endregion


    public float GetHeight(Vector2 point)
    {
        if (point.x < 0 || point.x >= ChunkSize || point.y < 0 || point.y >= ChunkSize) return -256f;

        int iX = MathFun.Floor(point.x);
        int iY = MathFun.Floor(point.y);

        float midX = point.x - iX;
        float midY = point.y - iY;

        float a = MathFun.Lerp(chunkMesh.GetPoint(iX, iY), chunkMesh.GetPoint(iX + 1, iY), midX);
        float b = MathFun.Lerp(chunkMesh.GetPoint(iX, iY + 1), chunkMesh.GetPoint(iX + 1, iY + 1), midX);

        return MathFun.Lerp(a, b, midY);
    }

    public bool CheckViewDistance(float distance, Vector2 position)
    {
        bool checkDist = chunkBounds.PointWithinBounds(position, distance);

        if (checkDist && !chunkObj.activeSelf) chunkObj.SetActive(true);
        else if (!checkDist && chunkObj.activeSelf) chunkObj.SetActive(false);

        return checkDist;
    }

    public static float SeaLevel { get { return World.Map.seaLevel; } }
    public int ChunkSize { get { return MeshData.MeshSize - 1; } }
    public Transform GetChunkTransform() { return chunkObj.transform; }
    public Vector2 ChunkLocV2() { return new Vector2(chunkObj.transform.position.x, chunkObj.transform.position.z); }
    public Vector3 ChunkOffset { get { return chunkObj.transform.position; } }
}