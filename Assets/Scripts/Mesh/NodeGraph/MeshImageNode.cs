using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


public class MeshImageNode : Node 
{
	[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
	public Texture2D entry;

	public Texture GetValue()
	{
		return GetInputValue("entry", entry);
	}
}