using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base Stats")]
    public float speed;
    public float runBoost;
    public Color skinColor;

    public FSM stateMachine;
    protected bool running;
    protected Vector3 momentum;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();
    }

    protected virtual void GameLoop()
    {
        stateMachine.RunFSM();
        GetMovement();

        float modSpeed = speed;
        if (running) modSpeed *= runBoost;

        transform.position += momentum * modSpeed * Time.deltaTime;
        momentum.x = 0f;
        momentum.z = 0f;
    }

    protected virtual void Initialize()
    {
        GetComponentInChildren<MeshRenderer>().material.color = skinColor;
        
        stateMachine = new FSM(this);
        BuildStateMachine();
    }

    public virtual void MoveDirection(Vector2 direction)
    {
        direction.Normalize();

        momentum.x = direction.x;
        momentum.z = direction.y;
    }

    protected virtual void GetMovement()
    {
        
    }

    protected virtual void BuildStateMachine()
    {

    }
}
