using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraData : MonoBehaviour
{
    public int treeIndex;
}

[System.Serializable]
public class TreeStyle
{
    public string treeName;
    public Mesh model;
    public bool blocksMovement;
    public Material[] materials;
}