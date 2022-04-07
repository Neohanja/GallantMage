using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Actor
{
    public GameObject actorObj;

    //Base Stats
    public ActorType actorType;
    public Stats stats;
    protected Movement actorMovement;

    // Mesh Stuff
    MeshFilter model;
    MeshRenderer renderProperties;

    public Actor(ActorType aType, Stats list, Vector3 spawnLoc, Race racials, GameObject attachSpecial = null)
    {
        stats = list;
        stats.ModStats(racials.racialStats);
        actorType = aType;

        actorObj = new GameObject(actorType.ToString());
        if (attachSpecial != null) attachSpecial.transform.SetParent(actorObj.transform);

        switch(actorType)
        {
            case ActorType.Player:
                actorMovement = actorObj.AddComponent<PlayerMove>();
                break;
            case ActorType.NPC:
                actorMovement = actorObj.AddComponent<VilMove>();
                break;
            case ActorType.Mob:
                actorMovement = actorObj.AddComponent<MobMove>();
                break;
            default:
                actorMovement = actorObj.AddComponent<Movement>();
                break;
        }
        
        actorMovement.speed = racials.baseSpeed;
        actorMovement.runBoost = racials.baseRun;

        model = actorObj.AddComponent<MeshFilter>();
        renderProperties = actorObj.AddComponent<MeshRenderer>();

        model.mesh = racials.baseModel;
        renderProperties.material = AIManager.AI_Engine.actorMat;
        renderProperties.material.color = racials.raceColor;

        actorObj.transform.position = spawnLoc;
    }

    public enum ActorType
    {
        Player,
        NPC,
        Mob
    }
}
