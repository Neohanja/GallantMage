using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
    public Movement listener;

    private void OnTriggerStay(Collider other)
    {
        if(listener == null) listener = GetComponentInParent<Movement>();

        ClutterData environment = other.GetComponentInParent<ClutterData>();
        if(environment != null)
        {
            listener.MeleeRange(environment.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (listener == null) listener = GetComponentInParent<Movement>();

        ClutterData environment = other.GetComponentInParent<ClutterData>();
        if (environment != null)
        {
            listener.MeleeRange(null);
        }
    }
}
