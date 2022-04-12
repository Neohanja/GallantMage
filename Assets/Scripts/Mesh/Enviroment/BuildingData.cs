using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string buildingName;
    public BoxBounds buildingBounds;
}

[System.Serializable]
public class BuildingSpawner
{
    public string buildingName;
    public GameObject buildingSpawn;
}