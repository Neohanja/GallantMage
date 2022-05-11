using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Map { private set; get; }

    [Header("Node Builder")]
    public TerrainGraph terrainGraph;

    [Header("Materials")]
    public Material terrainMat;
    public Material waterMat;
    Dictionary<Vector2Int, Chunk> chunkMap;
    List<Vector2Int> activeChunks;
    public List<Vector3> potentialSpawns;
    List<Vector2Int> queuedChunks;

    [Header("Game Settings")]
    public int seed;
    public int viewDistance;
    public float seaLevel;
    public float minHeight;
    public float growth;

    [Header("Editor Settings")]
    public Vector2 testOffset;
    public GameObject testMesh;
    public bool autoUpdate;

    void Awake()
    {
        if (Map != null && Map != this) Destroy(gameObject);
        else Map = this;
    }

    void Start()
    {
        if (UIManager.ActiveUI != null) seed = UIManager.ActiveUI.seed;
        if (testMesh != null) Destroy(testMesh);

        chunkMap = new Dictionary<Vector2Int, Chunk>();
        activeChunks = new List<Vector2Int>();
        potentialSpawns = new List<Vector3>();
        queuedChunks = new List<Vector2Int>();

        VerifyMap(new Vector2Int(0, 0), initMap: true);
        if (AIManager.AI_Engine != null) AIManager.AI_Engine.StartAI();
        if (UIManager.ActiveUI != null) UIManager.ActiveUI.DoneLoading();
    }

    void Update()
    {
        VerifyMap(AIManager.AI_Engine.GetLocation());

        if (queuedChunks.Count > 0) BuildQueue();
    }

    public void AddSpawnPoint(Vector3 location)
    {
        if(!potentialSpawns.Contains(location))
            potentialSpawns.Add(location);
    }

    public Vector3 GetRandomSpawn()
    {
        if(potentialSpawns.Count > 0)
        {
            return potentialSpawns[RanGen.PullNumber(seed, 1337) % potentialSpawns.Count];
        }
        else
        {
            float xPos = RanGen.PullNumber(seed, 10107) % ChunkSize - Chunk.HalfMap + 0.5f;
            float zPos = RanGen.PullNumber(seed, 10107, 8008) % ChunkSize - Chunk.HalfMap + 0.5f;
            float yPos = GetHeight(new Vector2(xPos, zPos));
            return new Vector3(xPos, yPos, zPos);
        }
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

    public void VerifyMap(Vector2 position, bool autoCorrect = true, bool initMap = false)
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
                AddChunk(new Vector2Int(x, y) + location, initMap);
            }
        }

        if (activeChunks.Count > 0)
        {
            for (int c = activeChunks.Count - 1; c >= 0; c--)
            {
                if (!chunkMap[activeChunks[c]].CheckViewDistance(viewDistance * ChunkSize * 2, position, false))
                {
                    activeChunks.RemoveAt(c);
                }
            }
        }
    }

    public void BuildQueue()
    {
        // Work on MultiThreading

        foreach(Vector2Int location in queuedChunks)
            AddChunk(location, true);
        queuedChunks.Clear();
    }

    public void AddChunk(Vector2Int location, bool buildNow)
    {
        if (!buildNow && !chunkMap.ContainsKey(location))
        {
            queuedChunks.Add(location);
            return;
        }

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
        if (Application.isPlaying) return;
        ChunkData chunkData = terrainGraph.ProcessNodes(testOffset, seed);
        MeshData chunkMesh = new MeshData(chunkData.GetPoints(), growth, minHeight);

        testMesh.GetComponent<MeshFilter>().mesh = chunkMesh.GetMesh();
    }

    int ChunkSize { get { return MeshData.MeshSize - 1; } }
}
