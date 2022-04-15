using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string buildingName;
    public BoxBounds buildingBounds;
    public GameObject door;

    public void RecalcDoor(Vector3 chunkOffset)
    {
        if (door == null) return;
        Vector2 doorPos = new Vector2(door.transform.position.x - chunkOffset.x, door.transform.position.z - chunkOffset.z);
        buildingBounds.door = doorPos;
    }
}