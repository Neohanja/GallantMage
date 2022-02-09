using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "TerraGraph", menuName = "Terrain/Terrain Graph")]
public class TerrainGraph : NodeGraph 
{
	public MeshNode current;
}