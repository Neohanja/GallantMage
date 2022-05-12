using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager AI_Engine { private set; get; }

    public static readonly int Player = 0;

    [Header("Game Actors")]
    public List<Actor> aiAgents;
    public bool noTowns;
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

        aiAgents.Add(new Actor(Actor.ActorType.Player, masterList.CopyList(), World.Map.GetRandomSpawn(),
            npcRacials[0], playerCam));
        CamControl.MainCam.SetTarget(aiAgents[0].actorObj);
    }

    public void AddPopulous(List<Vector3> newActors, Chunk home)
    {
        if (noTowns) return;
        foreach(Vector3 spawner in newActors)
        {
            int npcIndex = home.chunkRNG.Roll(0, npcRacials.Count - 1);
            aiAgents.Add(new Actor(Actor.ActorType.NPC, masterList.CopyList(), spawner, npcRacials[npcIndex]));
            aiAgents[aiAgents.Count - 1].actorObj.transform.SetParent(home.GetChunkTransform());
        }
    }

    public Vector3 RandomSpawn()
    {
        float x, y, z;

        do
        {
            x = Random.Range(0f, 240f);
            z = Random.Range(0f, 240f);
            y = World.Map.GetHeight(new Vector2(x, z));
        } while (y <= World.Map.seaLevel);

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
