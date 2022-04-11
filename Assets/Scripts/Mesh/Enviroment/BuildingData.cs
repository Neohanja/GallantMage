using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string buildingName;
}

[System.Serializable]
public class BuildingSpawner
{
    public string buildingName;
    public Mesh model;
    public Material[] materials;
}