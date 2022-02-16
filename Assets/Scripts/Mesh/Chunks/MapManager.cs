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
    public Material waterMat;
    Dictionary<Vector2Int, Chunk> chunkMap;
    public List<Vector2Int> activeChunks;

    [Header("Game Settings")]
    public int seed;
    public int viewDistance;
    public float seaLevel;

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
        activeChunks = new List<Vector2Int>();
        VerifyMap(new Vector2Int(0, 0));
        if (AIManager.AI_Engine != null) AIManager.AI_Engine.StartAI();
    }

    void Update()
    {
        VerifyMap(AIManager.AI_Engine.GetLocation());
    }

    public float GetHeight(Vector2 point, bool autoCorrect = true)
    {
        if (autoCorrect)
        {
            point.x += Chunk.HalfMap;
            point.y += Chunk.HalfMap;
        }

        int chunkX = MathFun.Floor(point.x / ChunkSize);
        int chunkY = MathFun.Floor(point.y / ChunkSize);

        if (!chunkMap.ContainsKey(new Vector2Int(chunkX, chunkY))) return 0f;

        float xPos = point.x - chunkX * ChunkSize;
        float yPos = point.y - chunkY * ChunkSize;

        return chunkMap[new Vector2Int(chunkX, chunkY)].GetHeight(new Vector2(xPos, yPos));
    }

    public void VerifyMap(Vector2 position, bool autoCorrect = true)
    {
        if(autoCorrect)
        {
            position.x += Chunk.HalfMap;
            position.y += Chunk.HalfMap;
        }

        Vector2Int location = new Vector2Int(MathFun.Floor(position.x / ChunkSize), MathFun.Floor(position.y / ChunkSize));

        for(int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                AddChunk(new Vector2Int(x, y) + location);
            }
        }

        if (activeChunks.Count > 0)
        {
            for (int c = activeChunks.Count - 1; c >= 0; c--)
            {
                if (!chunkMap[activeChunks[c]].CheckViewDistance(viewDistance * ChunkSize, position, false))
                {
                    activeChunks.RemoveAt(c);
                }
            }
        }
    }

    public void AddChunk(Vector2Int location)
    {
        if (!activeChunks.Contains(location))
        {
            activeChunks.Add(location);
        }
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
