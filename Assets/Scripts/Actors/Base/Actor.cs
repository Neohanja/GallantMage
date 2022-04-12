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

    // Collision Stuff
    Rigidbody actorRB;
    CapsuleCollider actorCollider;

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

        // Collision for Raycasting and stuff. Since that seems suddenly important
        actorCollider = actorObj.AddComponent<CapsuleCollider>();
        actorRB = actorObj.AddComponent<Rigidbody>();
        actorCollider.center = new Vector3(0, 0.5f, 0);
        actorCollider.radius = 0.25f;
        actorCollider.height = 1f;
        actorRB.constraints = RigidbodyConstraints.FreezeRotation;// | RigidbodyConstraints.FreezePositionY;

        actorObj.transform.position = spawnLoc;
    }

    public enum ActorType
    {
        Player,
        NPC,
        Mob
    }
}
