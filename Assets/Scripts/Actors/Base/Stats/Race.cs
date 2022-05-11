using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Racial Stats", menuName = "Gallant Mage/Racial Stats")]
public class Race : ScriptableObject
{
    public string raceName;
    public Mesh baseModel;
    public GameObject actorObj;
    public Stats racialStats;
    public Color raceColor;

    public float baseSpeed;
    public float baseRun;

    public string GetName(bool player)
    {
        if (player) return "Player";
        else return raceName;
    }
}
