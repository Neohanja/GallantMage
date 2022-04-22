using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string buildingName;
    public PolyBounds bBounds;
    public GameObject door;
    public BuildingType buildingType;
    public bool doNotEnter;

    public void Init()
    {
        bBounds = GetComponent<PolyBounds>();
        if (bBounds == null) bBounds = gameObject.AddComponent<PolyBounds>();
        
        bBounds.Init();
    }

    public bool BuildingWithin(BuildingData other, float buffer)
    {
        foreach(Vector3 p in other.GetPoints())
        {
            if (PointWithin(p, buffer)) return true;
        }

        foreach(Vector3 p in bBounds.GetPoints())
        {
            if (other.PointWithin(p, buffer)) return true;
        }

        return false;
    }

    public List<Vector3> GetPoints()
    {
        return bBounds.GetPoints();
    }

    public float Size { get { return bBounds.Size(); } }
    public Vector2 Center { get { return new Vector2(bBounds.central.x, bBounds.central.z); } }

    public bool PointWithin(Vector3 point, float buffer)
    {
        return bBounds.TriPointWithin(point, buffer);
    }

    public Vector2 DoorLoc(bool local)
    {
        if (local)
        {
            Vector3 v1 = transform.localPosition;
            if (door == null) return new Vector2(v1.x, v1.z);
            else return new Vector2(v1.x + door.transform.localPosition.x, v1.z + door.transform.localPosition.z);
        }
        else
        {
            if (door == null) return new Vector2(transform.position.x, transform.position.z);
            else return new Vector2(door.transform.position.x, door.transform.position.z);
        }
    }

    public enum BuildingType
    {
        Home,
        Smith,
        Store,
        Utility
    }
}