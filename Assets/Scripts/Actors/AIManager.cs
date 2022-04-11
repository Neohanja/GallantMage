using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager AI_Engine { private set; get; }

    public static readonly int Player = 0;

    [Header("Game Actors")]
    public int testNpcCount;
    public int testMobCount;
    public List<Actor> aiAgents;
    int target;

    [Header("Master Lists")]
    public List<Race> npcRacials;
    public List<Race> mobRacials;
    public Stats masterList;

    [Header("Materials")]
    public Material actorMat;

    void Awake()
    {
        if (AI_Engine != null && AI_Engine != this) Destroy(gameObject);
        else AI_Engine = this;
    }

    public Vector2 GetLocation(int actorIndex = 0)
    {
        Vector3 pos = aiAgents[actorIndex].actorObj.transform.position;

        return new Vector2(pos.x, pos.z);
    }

    // Start is called before the first frame update
    public void StartAI()
    {
        aiAgents = new List<Actor>();

        GameObject playerCam = GetComponentInChildren<Camera>().gameObject;

        aiAgents.Add(new Actor(Actor.ActorType.Player, masterList.CopyList(), MapManager.World.GetRandomSpawn(),
            npcRacials[0], playerCam));
        CamControl.MainCam.SetTarget(aiAgents[0].actorObj);

        for (int i = 0; i < testNpcCount; i++)
        {
            int npcIndex = Random.Range(0, npcRacials.Count);
            aiAgents.Add(new Actor(Actor.ActorType.NPC, masterList.CopyList(), RandomSpawn(), npcRacials[npcIndex]));
        }

        for (int i = 0; i < testMobCount; i++)
        {
            int mobIndex = Random.Range(0, mobRacials.Count);
            aiAgents.Add(new Actor(Actor.ActorType.Mob, masterList.CopyList(), RandomSpawn(), mobRacials[mobIndex]));
        }
    }

    public void AddPopulous(List<Vector3> newActors)
    {
        foreach(Vector3 spawner in newActors)
        {
            int npcIndex = Random.Range(0, npcRacials.Count);
            aiAgents.Add(new Actor(Actor.ActorType.NPC, masterList.CopyList(), spawner, npcRacials[npcIndex]));
        }
    }

    public Vector3 RandomSpawn()
    {
        float x, y, z;

        do
        {
            x = Random.Range(-120f, 120f);
            z = Random.Range(-120f, 120f);
            y = MapManager.World.GetHeight(new Vector2(x, z));
        } while (y <= MapManager.World.seaLevel);

        return new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && CamControl.MainCam != null)
        {
            target++;
            if (target >= aiAgents.Count) target = 0;
            CamControl.MainCam.SetTarget(aiAgents[target].actorObj);
        }
    }
}
