using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterBuilder : MonoBehaviour
{
    public static ClutterBuilder Generator { get; private set; }
    static readonly int MaxTries = 100;

    [Header("Clutter Objects")]
    public List<ClutterStyle> clutter;

    [Header("Tree Spawn")]
    public int minEnvironmentalItems = 256;
    public int maxEnvironmentalItems = 600;

    [Header("Clutter In World")]
    List<ClutterPlacement> environmentEffects;
    int weightTotal = 0;
    int[] indyWeight;

    void Awake()
    {
        if (Generator != null && Generator != this) Destroy(gameObject);
        else Generator = this;

        //In case I remove something and forget to remove it from here
        for (int t = clutter.Count - 1; t >= 0; t--)
        {
            if (clutter[t].spawner == null) clutter.RemoveAt(t);
        }

        environmentEffects = new List<ClutterPlacement>();

        indyWeight = new int[clutter.Count];
        for (int l = 0; l < clutter.Count; l++)
        {
            indyWeight[l] = clutter[l].placementWeight;
            weightTotal += clutter[l].placementWeight;
        }
    }

    public void BuildEnvironmentClutter(Chunk chunkID)
    {
        if (clutter == null || clutter.Count == 0) return;

        int clutterPop = chunkID.Roll(minEnvironmentalItems, maxEnvironmentalItems);
        List<Vector3> points = new List<Vector3>();
        List<float> sizes = new List<float>();

        for (int i = 0; i < clutterPop; i++)
        {
            int weightID = chunkID.Roll(1, weightTotal);
            int clutterID = indyWeight.Length;
            for (int j = 0; j < indyWeight.Length; j++)
            {
                if (weightID <= indyWeight[j])
                {
                    clutterID = j;
                    break;
                }
                else
                {
                    weightID -= indyWeight[j];
                }
            }

            ClutterStyle item = clutter[clutterID];
            float mySize = item.spawner.GetComponent<ClutterData>().size;
            float myScale = chunkID.Percent() + 0.5f;
            myScale *= mySize;

            Vector2 spawnPoint;
            bool good = true;
            int tried = 0;
            do
            {
                spawnPoint = chunkID.RandomSpotInChunk(item.canPlaceInWater);

                tried++;

                for (int p = 0; p < points.Count; p++)
                {
                    Vector2 a = new Vector2(points[p].x, points[p].z);

                    if (Vector2.Distance(spawnPoint, a) < sizes[p] + mySize)
                    {
                        good = false;
                        break;
                    }
                }
            } while (!good && tried < MaxTries);
            if (!good) continue;
            float y = chunkID.GetHeight(spawnPoint);

            Vector3 location = new Vector3(spawnPoint.x, y, spawnPoint.y);
            location += chunkID.ChunkOffset;
            float yRot = chunkID.RandomIndex(360);
            float xRot = item.fullRotation ? chunkID.RandomIndex(360) : 0;
            float zRot = item.fullRotation ? chunkID.RandomIndex(260) : 0;
            Vector3 scale = myScale * Vector3.one;
            GameObject clutterItem = Instantiate(item.spawner, location,
                Quaternion.identity, chunkID.GetChunkTransform());

            clutterItem.name = clutterItem.GetComponent<ClutterData>().itemType.ToString();
            clutterItem.transform.localScale = scale;
            clutterItem.transform.Rotate(new Vector3(xRot, yRot, zRot));

            environmentEffects.Add(new ClutterPlacement(location, mySize, clutterItem));

            points.Add(location);
            sizes.Add(mySize);
        }

        points.Clear();
        sizes.Clear();
    }
}

public class ClutterPlacement
{
    public Vector3 point;
    public float size;
    public GameObject refernce;

    public ClutterPlacement(Vector3 location, float scale, GameObject item)
    {
        point = location;
        size = scale;
        refernce = item;
    }
}

[System.Serializable]
public class ClutterStyle
{
    public string clutterName;
    public int placementWeight;
    public bool fullRotation;
    public bool canPlaceInWater;
    public GameObject spawner;
}