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
        if(target != null)
            transform.position = target.transform.position + offset;
    }

    public void SetTarget(GameObject actor)
    {
        target = actor.transform;
    }
}
