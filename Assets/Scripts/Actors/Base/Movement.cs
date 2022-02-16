using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base Stats")]
    public float speed;
    public float runBoost;

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
        AttachToGround();
        momentum.x = 0f;
        momentum.z = 0f;
    }

    protected virtual void AttachToGround()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        float y = MapManager.World.GetHeight(new Vector2(x, z));
        transform.position = new Vector3(x, y, z);
    }

    protected virtual void Initialize()
    {
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
