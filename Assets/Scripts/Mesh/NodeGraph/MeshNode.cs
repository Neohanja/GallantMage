using UnityEngine;
using XNode;

/// <summary>
/// Creates a basic Mesh Data Object to pass to the Chunk/World Map
/// Use as a start Node
/// </summary>
public abstract class MeshNode : Node
{
	[Output(typeConstraint = TypeConstraint.Strict)]
	public ChunkData points;
	[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
	public ChunkData entry;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		points = new ChunkData();

		switch (port.fieldName)
        {
			case "points":
				ProcessNode();
				return points;
			default:
				return null;
		}
	}

	public virtual void ProcessNode()
    {
		
    }

	public virtual void SetOffset(Vector2 newOffset, int newSeed) { }

	public virtual ChunkData GetMesh()
    {
		points = new ChunkData();
		ProcessNode();
		return points;
    }

	public virtual bool LastNode()
    {
		return false;
    }

	public virtual string GetString()
    {
		return "MeshNode";
    }

	public TerrainGraph GetGraph
	{
		get
		{
			if (graph is TerrainGraph)
				return graph as TerrainGraph;
			else
			{
				Debug.LogWarning("Node placed in incorrect graph type");
				return default;
			}
		}
	}
}