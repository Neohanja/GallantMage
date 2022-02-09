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
	[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
	public HeightValues entry;

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		switch(port.fieldName)
        {
			case "points":
				points = new HeightValues();
				ProcessNode();
				return points;				
		}
		return null;
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

	public virtual bool LastNode()
    {
		return false;
    }

	public virtual string GetString()
    {
		return "BaseNode";
    }
}