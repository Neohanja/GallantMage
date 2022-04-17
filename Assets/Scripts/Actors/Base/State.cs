using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected StateID state;
    protected FSM stateMachine;
    protected float stateTimer;
    protected float elapsedTime;

    protected Vector2 destination;    

    public State(StateID myState, FSM parent)
    {
        state = myState;
        stateMachine = parent;
    }

    public virtual StateID Determine()
    {
        return state;
    }

    public virtual void Action()
    {
        elapsedTime += Time.deltaTime;
    }

    public virtual void PickDestination(Vector2 point)
    {
        destination = point;
    }

    public Vector2 RecalcPath()
    {
        if (MapManager.World != null)
            return MapManager.World.GetDoorBetween(stateMachine.GetLocation(), destination);
        else return destination;
    }

    public float DistanceToDestination()
    {
        if (destination == null) return 0f;
        return Vector2.Distance(stateMachine.GetLocation(), destination);
    }

    public virtual void Restart()
    {
        elapsedTime = 0f;
    }

    public enum StateID
    {
        Idle,
        Wander
    }
}
