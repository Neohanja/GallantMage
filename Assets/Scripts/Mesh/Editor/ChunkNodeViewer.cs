using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class ChunkNodeViewer : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        MapManager editorChunk = (MapManager)target;

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
