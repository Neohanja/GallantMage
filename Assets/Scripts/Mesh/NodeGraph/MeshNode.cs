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

	protected bool rebuildImage;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		points = new HeightValues();
		if (image == null)
		{
			image = new Texture2D(MeshData.MeshSize, MeshData.MeshSize);
			rebuildImage = true;
		}	

		switch (port.fieldName)
        {
			case "points":
				ProcessNode();
				return points;
			case "image":
				if (rebuildImage)
				{
					ProcessNode(true);
					rebuildImage = false;
				}
				return image;
			default:
				return null;
		}
	}

	public virtual void ProcessNode(bool addImage = false)
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
		ProcessNode(true);
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

	public override void OnRemoveConnection(NodePort port)
	{
		if (port.fieldName == "image") rebuildImage = true;
	}

	protected virtual void DoThisOrWeBreakStuff()
	{

	}

	protected void OnValidate()
	{
		rebuildImage = true;
		DoThisOrWeBreakStuff();
	}
}