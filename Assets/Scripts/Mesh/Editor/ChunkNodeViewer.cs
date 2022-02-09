using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkNodeViewer : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Chunk editorChunk = (Chunk)target;

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
