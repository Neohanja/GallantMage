using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : State
{
    Vector2 movement;

    public Wander(FSM fsmController) : base(StateID.Wander, fsmController)
    {

    }

    public override StateID Determine()
    {
        if (elapsedTime >= stateTimer)
        {
            return StateID.Idle;
        }

        return base.Determine();
    }

    public override void Restart()
    {
        movement = Noise.RandomWalk(new Vector2(Random.Range(0, 100), Random.Range(0, 100)), Random.Range(0, 100), Random.Range(0, 5000));
        stateTimer = Random.Range(2f, 5f);

        base.Restart();
    }

    public override void Action()
    {
        stateMachine.movement.MoveDirection(movement);

        base.Action();
    }
}
