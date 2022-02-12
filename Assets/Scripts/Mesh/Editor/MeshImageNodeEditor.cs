using UnityEngine;
using UnityEditor;
using XNode;

[CustomNodeEditor(typeof(MeshImageNode))]
public class MeshImageNodeEditor : XNodeEditor.NodeEditor
{
    public override void OnBodyGUI()
    {
        if (target == null)
        {
            Debug.LogWarning("Target Does not exist for Mesh Image Node.");
            return;
        }

        MeshImageNode imageNode = (MeshImageNode)target;

        Texture texture = imageNode.GetValue();

        base.OnBodyGUI();

        EditorGUILayout.LabelField(new GUIContent(texture), GUILayout.Width(175), GUILayout.Height(175));
    }
}
