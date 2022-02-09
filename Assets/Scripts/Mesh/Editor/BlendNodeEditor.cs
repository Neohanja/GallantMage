using UnityEngine;
using UnityEditor;
using XNode;

[CustomNodeEditor(typeof(BlendNode))]
public class BlendNodeEditor : XNodeEditor.NodeEditor
{
    public override void OnBodyGUI()
    {
        BlendNode node = (BlendNode)target;

        base.OnBodyGUI();
    }
}
