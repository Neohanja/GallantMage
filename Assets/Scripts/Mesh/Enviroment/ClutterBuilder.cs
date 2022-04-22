using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterBuilder : MonoBehaviour
{
    public static ClutterBuilder Helper { get; private set; }
    static readonly int MaxTries = 100000;

    [Header("Clutter Objects")]
    public List<TreeStyle> trees;

    [Header("Tree Spawn")]
    public int MinTreeCount = 256;
    public int MaxTreeCount = 600;
    public float TreeDistance = 0.75f;

    void Awake()
    {
        if (Helper != null && Helper != this) Destroy(gameObject);
        else Helper = this;

        //In case I remove something and forget to remove it from here
        for(int t = trees.Count - 1; t >= 0; t--)
        {
            if(trees[t].treeSpawner == null) trees.RemoveAt(t);
        }
    }

    public void BuildTrees(Chunk chunkID)
    {
        if (trees == null || trees.Count == 0) return;
        Vector2 chunkOffset = new Vector2(chunkID.ChunkOffset.x, chunkID.ChunkOffset.z);

        int treePop = chunkID.chunkPRG.Roll(MinTreeCount, MaxTreeCount);
        List<Vector3> points = new List<Vector3>();

        for (int t = 0; t < MaxTries; t++)
        {
            if (points.Count >= treePop) break;

            float x = chunkID.chunkPRG.Roll(0, chunkID.ChunkSize) + chunkID.chunkPRG.Percent();
            float z = chunkID.chunkPRG.Roll(0, chunkID.ChunkSize) + chunkID.chunkPRG.Percent();
            TreeStyle thisTree = trees[chunkID.chunkPRG.Roll(0, trees.Count - 1)];
            float size = thisTree.treeSpawner.GetComponent<ClutterData>().size;

            Vector2 treeLoc = new Vector2(x, z);

            float y = chunkID.GetHeight(treeLoc);

            if (y > MapManager.World.seaLevel)
            {
                bool canPlace = true;
                foreach (Vector3 pt in points)
                {
                    if (Vector2.Distance(treeLoc, new Vector2(pt.x, pt.z)) <= TreeDistance + size)
                    {
                        canPlace = false;
                        break;
                    }

                }

                if (TownBuilder.Helper != null && 
                    TownBuilder.Helper.InBuilding(chunkOffset + treeLoc, TreeDistance * 2 + size))
                {
                    canPlace = false;
                }

                if (canPlace)
                {
                    Vector3 treeLocFull = new Vector3(x, y, z);
                    points.Add(treeLocFull);
                    Vector3 scale = (chunkID.chunkPRG.Percent() + 0.5f) * Vector3.one;

                    
                    GameObject tree = Instantiate(thisTree.treeSpawner, chunkID.ChunkOffset + treeLocFull, Quaternion.identity, chunkID.GetChunkTransform());
                    tree.name = "Tree " + points.Count.ToString() + ": " + thisTree.treeName;

                    tree.transform.localScale = scale;
                    tree.transform.Rotate(new Vector3(0, chunkID.chunkPRG.Roll(0, 360), 0));
                }
            }
        }
    }
}

[System.Serializable]
public class TreeStyle
{
    public string treeName;
    public GameObject treeSpawner;
}