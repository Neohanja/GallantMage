using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    public Idle(FSM fsmController) : base(StateID.Idle, fsmController)
    {

    }

    public override StateID Determine()
    {
        if(elapsedTime >= stateTimer)
        {
            return StateID.Wander;
        }

        return base.Determine();
    }

    public override void Restart()
    {
        stateTimer = Random.Range(3f, 6f);

        base.Restart();
    }

    public override void Action()
    {
        base.Action();
    }
}
