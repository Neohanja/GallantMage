using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    GameObject chunkObj;
    MeshFilter chunkFilter;
    MeshRenderer chunkRender;
    MeshData chunkMesh;

    ChunkData chunkData;
    Vector2 chunkCoord;

    public Chunk(Vector2 chunkCoord, ChunkData chunkInfo)
    {
        chunkData = chunkInfo;

        chunkObj = new GameObject("Chunk");
        chunkFilter = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();

        if (MapManager.World != null)
        {
            chunkRender.material = MapManager.World.terrainMat;
            chunkObj.transform.SetParent(MapManager.World.transform);
        }

        MeshData meshData = new MeshData(chunkData.GetPoints());
        chunkFilter.mesh = meshData.GetMesh();

        chunkObj.transform.position = new Vector3(chunkCoord.x - ChunkSize / 2, 0f, chunkCoord.y - ChunkSize / 2);
    }

    int ChunkSize { get { return MeshData.MeshSize - 1; } }
}
