using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSM
{
    public Movement movement;
    public State.StateID currentState;
    public int stateCount = 0;

    protected Dictionary<State.StateID, State> stateDrive;

    public FSM(Movement parent)
    {
        movement = parent;
    }

    public void RunFSM()
    {
        if (stateDrive == null) return;

        State.StateID checkState = stateDrive[currentState].Determine();
        if (checkState != currentState)
        {
            if(stateDrive.ContainsKey(checkState))
            {
                currentState = checkState;
                stateDrive[currentState].Restart();
            }
        }
        stateDrive[currentState].Action();
    }

    public void AddState(State.StateID newState, State classFunction)
    {
        if (stateDrive == null)
        {
            stateDrive = new Dictionary<State.StateID, State>();
            currentState = newState;
        }

        if(!stateDrive.ContainsKey(newState))
        {
            stateDrive.Add(newState, classFunction);
            stateCount++;
        }
        else
        {
            Debug.Log(newState.ToString() + " exists for " + movement.transform.name + ".");
        }
    }
}
