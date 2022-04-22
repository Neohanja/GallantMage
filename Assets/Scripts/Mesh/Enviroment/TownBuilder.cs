using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moving town building here to ensure cleaner code and easy fixing/changing
public class TownBuilder : MonoBehaviour
{
    static readonly int MaxTries = 100000;
    public static TownBuilder Helper { private set; get; }
    [Header("Town Parameters")]
    public int MinBuildings = 5;
    public int MaxBuildings = 15;
    public int MinPop = 8;
    public int MaxPop = 30;
    public float BuildingDistance = 2;
    [Header("Town Buildings")]
    public List<BuildingSpawner> buildings;
    public List<BuildingSpawner> townCenter;
    [Header("WorldBuildings")]
    public List<BuildingData> towns;

    void Awake()
    {
        if (Helper != null && Helper != this) Destroy(gameObject);
        else Helper = this;

        //In case I remove something and forget to remove it from here
        for (int b = buildings.Count - 1; b >= 0; b--)
        {
            if (buildings[b].buildingSpawn == null) buildings.RemoveAt(b);
        }
        for(int t = townCenter.Count -1; t >= 0; t--)
        {
            if (townCenter[t].buildingSpawn == null) townCenter.RemoveAt(t);
        }
    }

    public List<BuildingData> BuildTown(BoxBounds townBounds, RanGen townRNG, Chunk chunkID)
    {
        List<BuildingData> townBuildings = new List<BuildingData>();
        List<Vector3> spawnPoints = new List<Vector3>();
        Vector3 chunkOffset = chunkID.ChunkOffset;
        
        // Build the buildings first
        GameObject[] modelHomes = new GameObject[buildings.Count];
        for(int i = 0; i < buildings.Count; i++)
        {
            modelHomes[i] = Instantiate(buildings[i].buildingSpawn);
            modelHomes[i].GetComponent<BuildingData>().Init();
        }

        int buildingCount = townRNG.Roll(MinBuildings, MaxBuildings);

        if(townCenter != null && townCenter.Count > 0)
        {
            int buildingID = townRNG.Roll(0, townCenter.Count - 1);
            BuildingSpawner potentialBuilding = townCenter[buildingID];
            float xPos = townBounds.Center.x;
            float zPos = townBounds.Center.y;
            float yPos = chunkID.GetHeight(new Vector2(xPos, zPos));

            Vector3 buildingFullLoc = chunkOffset + new Vector3(xPos, yPos, zPos);
            GameObject home = Instantiate(potentialBuilding.buildingSpawn, buildingFullLoc, Quaternion.identity, chunkID.GetChunkTransform());
            home.GetComponent<BuildingData>().Init();
            townBuildings.Add(home.GetComponent<BuildingData>());
            home.name = "Town Banner: " + potentialBuilding.buildingName;
        }

        for (int t = 0; t < MaxTries; t++)
        {
            if (townBuildings.Count > buildingCount) break;

            int buildingID = townRNG.Roll(0, buildings.Count - 1);
            BuildingSpawner potentialBuilding = buildings[buildingID];
            BuildingData modelHome = modelHomes[buildingID].GetComponent<BuildingData>();

            float x = townRNG.Roll((int)(modelHome.Size * 0.5f), (int)townBounds.size.x - 1 - (int)(modelHome.Size * 0.5f)) + townRNG.Percent();
            float z = townRNG.Roll((int)(modelHome.Size * 0.5f), (int)townBounds.size.y - 1 - (int)(modelHome.Size * 0.5f)) + townRNG.Percent();
            x += townBounds.start.x;
            z += townBounds.start.y;

            Vector2 buildingLoc = new Vector2(x, z);
            float y = chunkID.GetHeight(buildingLoc);
            Vector3 tCenter = chunkOffset + new Vector3(townBounds.Center.x, y, townBounds.Center.y);

            modelHome.transform.position = chunkOffset + new Vector3(x, y, z);
            modelHome.transform.LookAt(tCenter);
            modelHome.bBounds.CalcCenter(0f);
            
            bool canPlace = true;
            foreach (BuildingData pt in townBuildings)
            {
                if (pt.BuildingWithin(modelHome, BuildingDistance))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                Vector3 buildingFullLoc = chunkOffset + new Vector3(x, y, z);
                
                GameObject home = Instantiate(potentialBuilding.buildingSpawn, buildingFullLoc, Quaternion.identity, chunkID.GetChunkTransform());
                BuildingData buildHome = home.GetComponent<BuildingData>();
                buildHome.Init();
                townBuildings.Add(buildHome);
                home.name = "Building " + townBuildings.Count.ToString() + ": " + potentialBuilding.buildingName;
                home.transform.LookAt(tCenter);

                if (potentialBuilding.isHouse)
                {
                    Vector3 spawnLoc = new Vector3(buildHome.Center.x + 0.5f, y + 0.2f, buildHome.Center.y + 0.5f);

                    spawnPoints.Add(spawnLoc);
                }
            }
        }

        if (AIManager.AI_Engine != null) AIManager.AI_Engine.AddPopulous(spawnPoints, chunkID);
        AddBuildingsToList(townBuildings);

        for(int i = 0; i < modelHomes.Length; i++)
        {
            Destroy(modelHomes[i]);
        }

        return townBuildings;
    }

    public void AddBuildingsToList(List<BuildingData> newHomes)
    {
        if (towns == null) towns = new List<BuildingData>();

        foreach (BuildingData newBuilding in newHomes)
        {
            towns.Add(newBuilding);
        }
    }

    public bool InBuilding(Vector2 a, float buffer = 0f)
    {
        foreach(BuildingData bBounds in towns)
        {
            if (Vector2.Distance(a, bBounds.Center) > bBounds.Size + buffer) continue;
            if (bBounds.PointWithin(new Vector3(a.x, 0f, a.y), buffer)) return true;
        }

        return false;
    }

    public int BuildingID(Vector2 a, float buffer = 0f)
    {
        for(int i = 0; i < towns.Count; i++)
        {
            if (towns[i].PointWithin(new Vector3(a.x, 0f, a.y), buffer)) return i;
        }
        return -1;
    }

    public Vector2 BuildingPath(Vector2 a, Vector2 b)
    {
        int aID = BuildingID(a);
        int bID = BuildingID(b);

        if (aID == bID) return b;
        if (aID >= 0) return towns[aID].DoorLoc(false);
        if (bID >= 0) return towns[bID].DoorLoc(false);

        // It *should* be one of the 3 conditions, but if not
        // (for some reason), then return the destination
        return b;
    }
}


[System.Serializable]
public class BuildingSpawner
{
    public string buildingName;
    public GameObject buildingSpawn;
    public bool isHouse;
}