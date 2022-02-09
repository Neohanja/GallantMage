using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Movement
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void GetMovement()
    {
        float side = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) running = true;
        else running = false;

        momentum = transform.forward * forward + transform.right * side;
    }
}
