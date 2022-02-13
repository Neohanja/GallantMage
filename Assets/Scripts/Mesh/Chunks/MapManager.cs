using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager World { private set; get; }

    [Header("Node Builder")]
    public TerrainGraph terrainGraph;

    [Header("Materials")]
    public Material terrainMat;
    Dictionary<Vector2Int, Chunk> chunkMap;

    [Header("Game Settings")]
    public int seed;
    public int viewDistance;

    [Header("Editor Settings")]
    public Vector2 testOffset;
    public GameObject testMesh;
    public bool autoUpdate;

    void Awake()
    {
        if (World != null && World != this) Destroy(gameObject);
        else World = this;
    }

    void Start()
    {
        Destroy(testMesh);
        chunkMap = new Dictionary<Vector2Int, Chunk>();
        VerifyMap(new Vector2Int(0, 0));
        if (AIManager.AI_Engine != null) AIManager.AI_Engine.StartAI();
    }

    public float GetHeight(Vector2 point)
    {
        int chunkX = MathFun.Floor(point.x / ChunkSize);
        int chunkY = MathFun.Floor(point.y / ChunkSize);

        if (!chunkMap.ContainsKey(new Vector2Int(chunkX, chunkY))) return 0f;

        float xPos = point.x - chunkX * ChunkSize;
        float yPos = point.y - chunkY * ChunkSize;

        return chunkMap[new Vector2Int(chunkX, chunkY)].GetHeight(new Vector2(xPos, yPos));
    }

    public void VerifyMap(Vector2Int location)
    {
        for(int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                AddChunk(new Vector2Int(x, y) + location);
            }
        }
    }

    public void AddChunk(Vector2Int location)
    {
        if (chunkMap.ContainsKey(location)) return;

        ChunkData newChunk = terrainGraph.ProcessNodes(location * ChunkSize, seed);
        chunkMap.Add(location, new Chunk(location * ChunkSize, newChunk));
    }

    public void CompileNodeGraph()
    {
        MeshData chunkMesh = new MeshData(terrainGraph.ProcessNodes(testOffset, seed).GetPoints());

        testMesh.GetComponent<MeshFilter>().mesh = chunkMesh.GetMesh();
    }

    int ChunkSize { get { return MeshData.MeshSize - 1; } }
}
