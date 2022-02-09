using UnityEngine;
using XNode;

/// <summary>
/// Creates a basic Mesh Data Object to pass to the Chunk/World Map
/// Use as a start Node
/// </summary>
public class MeshNode : Node
{
	[Output(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] 
	public HeightValues points;

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
	{
		points = new HeightValues();

		return points;
	}

	public virtual MeshData GetMesh()
    {
		return new MeshData(points.GetPoints());
    }

	public virtual string GetString()
    {
		return "BaseNode";
    }
}