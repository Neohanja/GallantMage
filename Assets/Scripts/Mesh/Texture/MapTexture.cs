using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTexture
{
    public static Texture2D TextureByHeight(int size, int scale, float[] heightMap, MapGradient colorKey)
    {
        int imgSize = (size - 1) * scale;
        Color[] cols = new Color[imgSize * imgSize];
        int index = 0;

        for (int h = 0; h < size - 1; h++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int w = 0; w < size - 1; w++)
                {
                    for (int x = 0; x < scale; x++)
                    {
                        //int index = (h * scale + y) * (size * scale) + w * scale + x;
                        float xSample = w + (x / (float)scale);
                        float ySample = h + (y / (float)scale);
                        float height = GetPoint(heightMap, xSample, ySample, size);
                        cols[index] = colorKey.EvaluateCol(height);
                        index++;
                    }
                }
            }
        }

        Texture2D texture = new Texture2D(imgSize, imgSize);
        texture.SetPixels(cols);
        //texture.filterMode = FilterMode.Point;
        //texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static float GetPoint(float[] heightMap, float x, float y, int size)
    {
        int iX = MathFun.Floor(x);
        int iY = MathFun.Floor(y);

        float gX = x - iX;
        float gY = y - iY;

        float a = MathFun.Lerp(heightMap[iY * size + iX], heightMap[iY * size + iX + 1], gX);
        float b = MathFun.Lerp(heightMap[(iY + 1) * size + iX], heightMap[(iY + 1) * size + iX + 1], gX);

        return MathFun.Lerp(a, b, gY);
    }
}
