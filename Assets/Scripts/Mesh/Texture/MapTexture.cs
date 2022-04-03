using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTexture
{
    public static Texture2D TextureByNormal(int size, int scale, Vector3[] norms, float[] hMap, float seaLevel, MapGradient colorKey)
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
                        float xSample = w + (x / (float)scale);
                        float ySample = h + (y / (float)scale);

                        if (GetPoint(hMap, xSample, ySample, size) <= seaLevel + 0.5f)
                        {
                            cols[index] = new Color(0.94f, 0.92f, 0.66f);
                        }
                        else
                        {
                            Vector3 height = LerpNorm(norms, xSample, ySample, size);
                            float angle = NormAngle(height);

                            if (angle > 0.1f)
                            {
                                cols[index] = new Color(0.55f, 0.35f, 0.03f);
                            }
                            else
                            {
                                cols[index] = new Color(0.04f, 0.72f, 0.03f);
                            }
                        }

                        index++;
                    }
                }
            }
        }

        Texture2D texture = new Texture2D(imgSize, imgSize);
        texture.SetPixels(cols);
        // texture.filterMode = FilterMode.Point;
        // texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static float NormAngle(Vector3 norm)
    {
        Vector3 forward = new Vector3(norm.x, 0, norm.z);
        return MathFun.Abs((Vector3.Angle(forward, norm)  - 90f) /180f);
    }

    public static Vector3 LerpNorm(Vector3[] norms, float x, float y, int size)
    {
        int iX = MathFun.Floor(x);
        int iY = MathFun.Floor(y);

        float gX = x - iX;
        float gY = y - iY;

        Vector3 a = MathFun.LerpV3(norms[iY * size + iX], norms[iY * size + iX + 1], gX);
        Vector3 b = MathFun.LerpV3(norms[(iY + 1) * size + iX], norms[(iY + 1) * size + iX + 1], gX);

        return MathFun.LerpV3(a, b, gY);
    }

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
