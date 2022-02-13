using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager AI_Engine { private set; get; }

    public GameObject Player;

    public List<GameObject> tempAgents;

    int target = 0;

    void Awake()
    {
        if (AI_Engine != null && AI_Engine != this) Destroy(gameObject);
        else AI_Engine = this;
    }

    // Start is called before the first frame update
    public void StartAI()
    {
        float x = Player.transform.position.x;
        float z = Player.transform.position.z;
        float y = MapManager.World.GetHeight(new Vector2(x + Chunk.HalfMap, z + Chunk.HalfMap));
        Player.transform.position = new Vector3(x, y, z);

        if (CamControl.MainCam != null) CamControl.MainCam.SetTarget(Player);

        foreach (GameObject agent in tempAgents)
        {
            x = agent.transform.position.x;
            z = agent.transform.position.z;
            y = MapManager.World.GetHeight(new Vector2(x + Chunk.HalfMap, z + Chunk.HalfMap));
            agent.transform.position = new Vector3(x, y, z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && CamControl.MainCam != null)
        {
            target++;
            if (target > tempAgents.Count) target = 0;
            if (target == 0) CamControl.MainCam.SetTarget(Player);
            else CamControl.MainCam.SetTarget(tempAgents[target - 1]);
        }
    }
}
