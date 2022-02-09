using UnityEngine;
using XNode;

public class MeshFinalNode : MeshNode
{
    public override string GetString()
    {
        return "EndNode";
    }

    public override void ProcessNode(bool addImage = false)
    {
        points = GetInputValue("entry", entry);
    }

    public override bool LastNode()
    {
        return true;
    }
}
