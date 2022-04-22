using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterData : MonoBehaviour
{
    public TreeType treeType;
    public float size;
    
    public enum TreeType
    {
        None,
        Birch,
        Willow,
        Pine
    }
}