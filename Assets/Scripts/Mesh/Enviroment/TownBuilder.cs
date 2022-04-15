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

    public List<BoxBounds> BuildTown(BoxBounds townBounds, RanGen townRNG, Chunk chunkID)
    {
        List<BoxBounds> townBuildings = new List<BoxBounds>();
        List<Vector3> spawnPoints = new List<Vector3>();
        Vector3 chunkOffset = chunkID.ChunkLocV3();

        int buildingCount = townRNG.Roll(MinBuildings, MaxBuildings);

        if(townCenter != null && townCenter.Count > 0)
        {
            int buildingID = townRNG.Roll(0, townCenter.Count - 1);
            BuildingSpawner potentialBuilding = townCenter[buildingID];
            BoxBounds bPoint = potentialBuilding.buildingSpawn.GetComponent<BuildingData>().buildingBounds.Copy();
            float xPos = townBounds.Center.x;
            float zPos = townBounds.Center.y;
            float yPos = chunkID.GetHeight(new Vector2(xPos, zPos));

            bPoint.MoveBuilding(new Vector2(xPos, zPos));

            Vector3 buildingFullLoc = chunkOffset + new Vector3(xPos, yPos, zPos);
            GameObject home = Instantiate(potentialBuilding.buildingSpawn, buildingFullLoc, Quaternion.identity, chunkID.GetChunkTransform());
            home.GetComponent<BuildingData>().buildingBounds = bPoint;
            home.GetComponent<BuildingData>().RecalcDoor(chunkID.ChunkOffset);
            townBuildings.Add(home.GetComponent<BuildingData>().buildingBounds.Copy());
            home.name = "Town Banner: " + potentialBuilding.buildingName;
        }

        for (int t = 0; t < MaxTries; t++)
        {
            if (townBuildings.Count > buildingCount) break;

            int buildingID = townRNG.Roll(0, buildings.Count - 1);
            BuildingSpawner potentialBuilding = buildings[buildingID];
            BoxBounds bPoint = potentialBuilding.buildingSpawn.GetComponent<BuildingData>().buildingBounds.Copy();            

            float x = townRNG.Roll(1, (int)townBounds.size.x - 1 - (int)bPoint.size.x) + townRNG.Percent();
            float z = townRNG.Roll(1, (int)townBounds.size.y - 1 - (int)bPoint.size.y) + townRNG.Percent();
            x += townBounds.start.x;
            z += townBounds.start.y;

            Vector2 buildingLoc = new Vector2(x, z);
            bPoint.MoveBuilding(buildingLoc);

            bool canPlace = true;
            foreach (BoxBounds pt in townBuildings)
            {
                if (pt.BoxOverlap(bPoint, BuildingDistance))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                float y = chunkID.GetHeight(buildingLoc);
                Vector3 buildingFullLoc = chunkOffset + new Vector3(x, y, z);
                Vector3 tCenter = new Vector3(townBounds.Center.x, y, townBounds.Center.y);

                GameObject home = Instantiate(potentialBuilding.buildingSpawn, buildingFullLoc, Quaternion.identity, chunkID.GetChunkTransform());
                home.GetComponent<BuildingData>().buildingBounds = bPoint;
                home.GetComponent<BuildingData>().RecalcDoor(chunkID.ChunkOffset);
                townBuildings.Add(home.GetComponent<BuildingData>().buildingBounds.Copy());
                home.name = "Building " + townBuildings.Count.ToString() + ": " + potentialBuilding.buildingName;
                home.transform.LookAt(chunkOffset + tCenter);

                if (potentialBuilding.isHouse)
                {
                    Vector3 spawnLoc = chunkOffset + new Vector3(bPoint.Center.x + 0.5f, 0.1f, bPoint.Center.y + 0.5f);

                    spawnPoints.Add(spawnLoc);
                }
            }
        }

        if (AIManager.AI_Engine != null) AIManager.AI_Engine.AddPopulous(spawnPoints, chunkID);

        return townBuildings;
    }
}


[System.Serializable]
public class BuildingSpawner
{
    public string buildingName;
    public GameObject buildingSpawn;
    public bool isHouse;
}