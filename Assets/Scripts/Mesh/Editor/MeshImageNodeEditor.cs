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

        //if (texture != null) EditorGUI.DrawPreviewTexture(new Rect(15, 75, 175, 175), texture);

        base.OnBodyGUI();
        
        EditorGUILayout.LabelField(new GUIContent(texture));
    }
}
