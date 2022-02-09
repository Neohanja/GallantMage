using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected StateID state;
    protected FSM stateMachine;
    protected float stateTimer;
    protected float elapsedTime;

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
