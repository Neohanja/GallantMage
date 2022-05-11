using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterData : MonoBehaviour
{
    private static readonly float fallSpeed = 0.2f;

    public ClutterType itemType;
    public bool destructible;
    public int resourceCount;
    public float size;


    Vector3 fallDirection;
    bool dying = false;
    float fallTimer = 2f;
    float elapsedTimer = 0f;

    public int GiveResources(int strength, Transform attacker)
    {
        int given;
        if(strength <= resourceCount)
        {
            given = strength;
            resourceCount -= strength;
        }
        else
        {
            given = resourceCount;
            resourceCount = 0;
        }

        if (resourceCount <= 0)
        {
            fallDirection = attacker.forward;
            dying = true;
            elapsedTimer = 0f;
        }

        return given;
    }

    private void Update()
    {
        if(dying)
        {
            elapsedTimer += Time.deltaTime;
            if((int)itemType < 4)
            {
                transform.Rotate(fallDirection * fallSpeed);
            }
            if(elapsedTimer >= fallTimer)
            {
                Destroy(gameObject);
            }
        }
    }

    public ClutterData Copy()
    {
        ClutterData data = new ClutterData();
        data.itemType = itemType;
        data.resourceCount = resourceCount;
        data.size = size;
        data.destructible = destructible;
        return data;
    }
    
    public enum ClutterType
    {
        None,
        Birch,
        Willow,
        Pine,
        Rock,
        Mineral
    }
}