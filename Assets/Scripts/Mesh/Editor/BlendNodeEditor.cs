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
        float prev = node.blend;

        node.blend = GUILayout.HorizontalSlider(node.blend, 0f, 1f, GUILayout.Width(75), GUILayout.Height(15));

        if (prev != node.blend) node.ProcessNode(true);
    }
}
