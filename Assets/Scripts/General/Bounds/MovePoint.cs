using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    public float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        float side = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        transform.position += (Vector3.forward * forward + Vector3.right * side) * speed * Time.deltaTime;
    }
}
