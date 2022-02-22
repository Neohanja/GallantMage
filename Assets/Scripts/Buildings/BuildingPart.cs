using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPart : MonoBehaviour
{
    public string partName;
    public bool requiresFoundation;
    public PartType partType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum PartType
    {
        Foudation,
        Stairs,
        Wall,
        Fence,
        Floor,
        Roof,
        WallDecor,
        FloorDecor,
        DoorFrame,
        Door
    }
}
