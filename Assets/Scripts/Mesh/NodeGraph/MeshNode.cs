using UnityEngine;
using XNode;

/// <summary>
/// Creates a basic Mesh Data Object to pass to the Chunk/World Map
/// Use as a start Node
/// </summary>
public class MeshNode : Node
{
	[Output(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
	public Texture2D image;
	[Output(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] 
	public HeightValues points;
	[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
	public HeightValues entry;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		points = new HeightValues();
		image = new Texture2D(MeshData.MeshSize, MeshData.MeshSize);
		ProcessNode();

		switch (port.fieldName)
        {
			case "points":				
				return points;
			case "image":
				return image;
			default:
				return null;
		}
	}

	public virtual void ProcessNode()
    {
		
    }

	public virtual HeightValues GetMesh()
    {
		points = new HeightValues();
		ProcessNode();
		return points;
    }

	public virtual Texture2D GetImage()
	{
		image = new Texture2D(MeshData.MeshSize, MeshData.MeshSize);
		ProcessNode();
		return image;
	}

	public virtual bool LastNode()
    {
		return false;
    }

	public virtual string GetString()
    {
		return "BaseNode";
    }
}