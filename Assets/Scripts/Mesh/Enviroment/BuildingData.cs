using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string buildingName;
    public BoxBounds buildingBounds;
    public GameObject door;
    public BuildingType buildingType;
    public bool doNotEnter;

    public Vector2 DoorLoc(bool local)
    {
        if (local)
        {
            float x = transform.localPosition.x + door.transform.localPosition.x;
            float z = transform.localPosition.z + door.transform.localPosition.z;
            return new Vector2(x, z);
        }
        
        return new Vector2(door.transform.position.x, door.transform.position.z);
    }

    public enum BuildingType
    {
        Home,
        Smith,
        Store,
        Utility
    }
}