using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapGradient
{
    public string gradName;
    public bool blend;
    [Range(0f, 1f)]
    public float blendAmount;
    public TextureData[] gradient;

    public Color EvaluateCol(float percent)
    {
        int id = 0;

        while(id < gradient.Length - 1 && percent > gradient[id + 1].threshold)
        {
            id++;
        }

        if(id == gradient.Length - 1 && percent > gradient[id].threshold)
        {            
            percent = gradient[id].threshold;
        }
        else if (percent < gradient[id].threshold && id == 0)
        {
            percent = gradient[id].threshold;
        }

        if(blend && id < gradient.Length - 1)
        {
            float point = Mathf.InverseLerp(gradient[id].threshold, gradient[id + 1].threshold, percent);
            if (point > 1f - blendAmount)
            {
                point = Mathf.InverseLerp(1f - blendAmount, 1f, point);
                return Color.Lerp(gradient[id].terrainColor, gradient[id].terrainColor, point);
            }
            else return gradient[id].terrainColor;
        }
        else
        {
            return gradient[id].terrainColor;
        }
    }

    public void OnValidate()
    {
        if (gradient != null && gradient.Length > 0)
        {
            float min = gradient[0].threshold;
            for (int i = 1; i < gradient.Length; i++)
            {
                if (gradient[i].threshold > min) min = gradient[i].threshold;
                else gradient[i].threshold = min;
            }
        }
    }
}

[System.Serializable]
public class TextureData
{
    public string terrainFeature;
    [Range(-1f, 1f)]
    public float threshold;
    public Color terrainColor;
    public Texture2D terrainImage;
}
