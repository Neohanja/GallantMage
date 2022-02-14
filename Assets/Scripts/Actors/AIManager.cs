using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager AI_Engine { private set; get; }

    [Header("Game Actors")]
    public int npcCount;
    public int mobCount;
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

    // Start is called before the first frame update
    public void StartAI()
    {
        aiAgents = new List<Actor>();

        float x = Random.Range(-120f, 120f);
        float y = Random.Range(-120f, 120f);

        aiAgents.Add(new Actor(Actor.ActorType.Player, masterList.CopyList(), new Vector2(x, y), npcRacials[0]));
        CamControl.MainCam.SetTarget(aiAgents[0].actorObj);

        for (int i = 0; i < npcCount; i++)
        {
            x = Random.Range(-120f, 120f);
            y = Random.Range(-120f, 120f);
            int npcIndex = Random.Range(0, npcRacials.Count);
            aiAgents.Add(new Actor(Actor.ActorType.NPC, masterList.CopyList(), new Vector2(x, y), npcRacials[npcIndex]));
        }

        for (int i = 0; i < mobCount; i++)
        {
            x = Random.Range(-120f, 120f);
            y = Random.Range(-120f, 120f);
            int mobIndex = Random.Range(0, mobRacials.Count);
            aiAgents.Add(new Actor(Actor.ActorType.Mob, masterList.CopyList(), new Vector2(x, y), mobRacials[mobIndex]));
        }
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
