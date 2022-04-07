using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flora : MonoBehaviour
{
    public static Flora TreeMaker { private set; get; }

    public List<TreeStyle> treeTypes;
    public List<TreePlant> plantedTrees;
    public List<GameObject> treePool;
    public List<int> activeTrees;

    public int maxTrees = 25;
    public float viewDistance = 100;
    public float updateDistance = 100f;
    public Vector3 lastUpdatePos;
    public Transform currentViewer;
    bool firstPush = false;

    void Awake()
    {
        if (TreeMaker != null && TreeMaker != this) Destroy(gameObject);
        else TreeMaker = this;
    }

    void Start()
    {
        treePool = new List<GameObject>();
        activeTrees = new List<int>();
        for(int i = 0; i < maxTrees; i++)
        {
            GameObject addTree = new GameObject("Tree " + i.ToString());
            addTree.AddComponent<MeshFilter>();
            addTree.AddComponent<MeshRenderer>();
            addTree.AddComponent<FloraData>();
            addTree.transform.SetParent(transform);
            treePool.Add(addTree);
        }
    }

    public void AddTreePoints(List<Vector3> point, Vector2 minMax, int tree = -1)
    {
        int seed = MapManager.World.seed;
        if (plantedTrees == null) plantedTrees = new List<TreePlant>();

        foreach(Vector3 loc in point)
        {
            int xLoc = MathFun.Floor(loc.x);
            int yLoc = MathFun.Floor(loc.z);

            int treeSelection = tree;
            if (treeSelection == -1) treeSelection = RanGen.PullNumber(seed, xLoc, yLoc, 4) % treeTypes.Count;
            float scale = RanGen.PullNumber(seed, xLoc, yLoc, 20) % 101 / 100f;
            scale = (minMax.y - minMax.x) * scale + minMax.x;

            plantedTrees.Add(new TreePlant(loc, scale, treeSelection));
        }
    }

    void Update()
    {
        if (currentViewer != CamControl.MainCam.target) currentViewer = CamControl.MainCam.target;

        if (currentViewer != null)
        {
            float dist = Vector3.Distance(currentViewer.position, lastUpdatePos);

            if (dist >= updateDistance || !firstPush)
            {
                CheckTreeVisibility();
                lastUpdatePos = currentViewer.position;
                firstPush = true;
            }
        }
    }

    public void CheckTreeVisibility()
    {
        if (maxTrees == 0 || plantedTrees == null) return;

        // List of distances of all potential trees
        Vector2[] distances = new Vector2[maxTrees];
        // The largest distance found so far
        float distToBeat = 0f;
        // What is the index (in distances) of the largest tree
        int dtbIndex = 0;
        // How many trees are we tracking so far?
        int dc = 0;

        // First, check the already active trees, to make sure we don't need to turn any off.
        // There may be a time when most or all trees are out of view distance.
        for(int a = activeTrees.Count - 1; a >= 0; a--)
        {
            // Get the index and distance of each active tree
            int checkTree = treePool[activeTrees[a]].GetComponent<FloraData>().treeIndex;
            float dist = plantedTrees[checkTree].GetDist(currentViewer);

            // If it is too far away, turn off the tree and remove it from the active pool
            if (dist > viewDistance)
            {
                treePool[activeTrees[a]].SetActive(false);
                activeTrees.RemoveAt(a);
            }
            // Otherwise, mark how far it is, so if "closer" trees need to be drawn instead.
            else
            {
                distances[dc] = new Vector2(dist, checkTree);
                if (dist > distToBeat)
                {
                    distToBeat = dist;
                    dtbIndex = dc;
                }
                dc++;
            }
        }

        for(int c = 0; c < plantedTrees.Count; c++)
        {
            float dist = plantedTrees[c].GetDist(currentViewer);
            // Skip this one if we can't see it or it is already active
            if (dist > viewDistance || TreeActive(c)) continue;
            // Don't have the "max trees" in scene yet
            if(dc < maxTrees)
            {
                distances[dc] = new Vector2(dist, c);
                if (dist > distToBeat)
                {
                    distToBeat = dist;
                    dtbIndex = dc;
                }
                dc++;
            }
            // Otherwise, check if there is a tree further than this one, and replace that one in the database
            else if(dist < distToBeat)
            {
                distances[dtbIndex] = new Vector2(dist, c);
                // Reset these values
                distToBeat = 0f;
                dtbIndex = 0;
                // Find the largest distance
                for (int rC = 0; rC < maxTrees; rC++)
                {
                    if(distances[rC].x > distToBeat)
                    {
                        distances[rC].x = distToBeat;
                        dtbIndex = rC;
                    }
                }
            }
        }

        // Now, make sure each tree has a spot
        // If we found less trees in distance than max trees, 
        // need to account for that too
        for(int rB = 0; rB < maxTrees && rB < dc; rB++)
        {
            // Don't rebuild the tree if it is in already.
            if (TreeActive((int)distances[rB].y)) continue;

            int treeIndex = plantedTrees[(int)distances[rB].y].tree;
            treePool[rB].SetActive(true);
            treePool[rB].GetComponent<MeshRenderer>().materials = treeTypes[treeIndex].materials;
            treePool[rB].GetComponent<MeshFilter>().mesh = treeTypes[treeIndex].model;
            treePool[rB].transform.position = plantedTrees[(int)distances[rB].y].point;
            treePool[rB].transform.localScale = Vector3.one * plantedTrees[(int)distances[rB].y].scale;
            treePool[rB].GetComponent<FloraData>().treeIndex = (int)distances[rB].y;

            if (!activeTrees.Contains(rB)) activeTrees.Add(rB);
        }
    }

    public bool TreeActive(int index)
    {
        foreach(GameObject cI in treePool)
        {
            if (cI.GetComponent<FloraData>().treeIndex == index && cI.activeSelf) return true;
            if (!cI.activeSelf) cI.GetComponent<FloraData>().treeIndex = -1;
        }

        return false;
    }
}

[System.Serializable]
public class TreePlant
{
    public Vector3 point;
    public float scale;
    public int tree;

    public TreePlant(Vector3 loc, float size, int style)
    {
        point = loc;
        scale = size;
        tree = style;
    }

    public float GetDist(Transform target)
    {
        Vector2 me = new Vector2(point.x, point.z);
        Vector2 them = new Vector2(target.position.x, target.position.z);
        return Vector2.Distance(me, them);
    }
}