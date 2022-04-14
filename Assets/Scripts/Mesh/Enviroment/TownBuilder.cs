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

    void Awake()
    {
        if (Helper != null && Helper != this) Destroy(gameObject);
        else Helper = this;
    }

    public List<BoxBounds> BuildTown(BoxBounds townBounds, RanGen townRNG, Chunk chunkID)
    {
        List<BoxBounds> townBuildings = new List<BoxBounds>();
        Vector3 chunkOffset = chunkID.ChunkLocV3();

        int buildingCount = townRNG.Roll(MinBuildings, MaxBuildings);

        for (int t = 0; t < MaxTries; t++)
        {
            if (townBuildings.Count >= buildingCount) break;

            int buildingID = townRNG.Roll(0, buildings.Count - 1);
            BuildingSpawner potentialBuilding = buildings[buildingID];
            BoxBounds bPoint = potentialBuilding.buildingSpawn.GetComponent<BuildingData>().buildingBounds.Copy();

            float x = townRNG.Roll(1, (int)townBounds.size.x - 1 - (int)bPoint.size.x) + townRNG.Percent();
            float z = townRNG.Roll(1, (int)townBounds.size.y - 1 - (int)bPoint.size.y) + townRNG.Percent();
            x += townBounds.start.x;
            z += townBounds.start.y;

            Vector2 buildingLoc = new Vector2(x, z);
            bPoint.start += buildingLoc;


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
                townBuildings.Add(bPoint);
                float y = chunkID.GetHeight(buildingLoc);
                Vector3 buildingFullLoc = chunkOffset + new Vector3(x, y, z);
                Vector3 tCenter = new Vector3(townBounds.Center.x, y, townBounds.Center.y);

                GameObject home = Instantiate(potentialBuilding.buildingSpawn, buildingFullLoc, Quaternion.identity, chunkID.GetChunkTransform());
                home.name = "Building " + townBuildings.Count.ToString() + ": " + potentialBuilding.buildingName;
                home.transform.LookAt(chunkOffset + tCenter);
            }
        }

        return townBuildings;
    }
}


[System.Serializable]
public class BuildingSpawner
{
    public string buildingName;
    public GameObject buildingSpawn;
}