using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public static CamControl MainCam { private set; get; }
    protected Vector3 offset;
    public Transform target;

    void Awake()
    {
        if (MainCam != null && MainCam != this) Destroy(gameObject);
        else MainCam = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = target.transform.position + offset;
            if (MapManager.World != null)
            {
                float x = transform.position.x;
                float z = transform.position.z;
                float y = MapManager.World.GetHeight(new Vector2(x, z));

                if (transform.position.y < y + offset.y)
                {
                    transform.position = new Vector3(x, y + offset.y, z);
                }
            }
            transform.LookAt(target.position + Vector3.up);
        }
    }

    public void SetTarget(GameObject actor)
    {
        target = actor.transform;
    }
}
