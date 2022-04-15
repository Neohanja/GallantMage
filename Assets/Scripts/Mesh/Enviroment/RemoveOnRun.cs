using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnRun : MonoBehaviour
{
    public bool removeItem;
    public bool removeVisual;

    private void Start()
    {
        if (removeItem)
        {
            Destroy(gameObject);
        }
        else if (removeVisual)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
