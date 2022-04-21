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
        if (elapsedTime >= stateTimer || DistanceToDestination() <= 0.2f)
        {
            return StateID.Idle;
        }

        return base.Determine();
    }

    public override void Restart()
    {
        destination = stateMachine.GetLocation();
        destination.x += Random.Range(-15f, 15f);
        destination.y += Random.Range(-15f, 15f);

        movement = RecalcPath();

        stateTimer = 300f;

        base.Restart();
    }

    public override void Action()
    {
        if (Vector2.Distance(stateMachine.GetLocation(), movement) <= 0.2f || elapsedTime % 0.25f == 0)
        {
            movement = RecalcPath();
        }

        stateMachine.movement.MoveDirection(movement);

        base.Action();
    }
}
