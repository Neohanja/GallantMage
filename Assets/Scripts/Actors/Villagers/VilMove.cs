using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilMove : Movement
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void GetMovement()
    {
        base.GetMovement();
    }

    protected override void BuildStateMachine()
    {
        stateMachine.AddState(State.StateID.Idle, new Idle(stateMachine));
        stateMachine.AddState(State.StateID.Wander, new Wander(stateMachine));
    }
}
