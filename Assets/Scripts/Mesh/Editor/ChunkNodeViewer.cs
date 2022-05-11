using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class ChunkNodeViewer : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        World editorChunk = (World)target;

        if (DrawDefaultInspector())
        {
            if(editorChunk.autoUpdate)
            {
                editorChunk.CompileNodeGraph();
            }
        }

        if (GUILayout.Button("Compile Node Graph"))
        {
            editorChunk.CompileNodeGraph();
        }
    }
}
