using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslandGen
{
    public static float[,] FalloffMap(Vector2Int chunkID, int chunkSize, int mapSize)
    {
        float[,] map = new float[chunkSize, chunkSize];

        Vector2 corner = new Vector2(0, 0);

        if (chunkID.x == 0) corner.x = chunkSize;
        if (chunkID.y == 0) corner.y = chunkSize;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                if (chunkID.x < 0 || chunkID.y < 0 || chunkID.x > mapSize || chunkID.y > mapSize)
                {
                    map[x, y] = 0f;
                }
                else if (chunkID.x == 0 || chunkID.y == 0 || chunkID.x == mapSize || chunkID.y == mapSize)
                {
                    if (chunkID.x > 0 && chunkID.x < mapSize) corner.x = x;
                    if (chunkID.y > 0 && chunkID.y < mapSize) corner.y = y;
                    map[x, y] = 1f - (Vector2.Distance(corner, new Vector2(x, y)) / chunkSize);
                }
                else
                {
                    map[x, y] = 1f;
                }

                map[x, y] = MathFun.Clamp(0.2f, 1f, map[x, y]);
            }
        }

        return map;
    }
}
