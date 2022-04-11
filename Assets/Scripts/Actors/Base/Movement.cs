using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base Stats")]
    public float speed;
    public float runBoost;
    public float actorHeight = 2f;

    public FSM stateMachine;
    protected bool running;
    protected Vector3 momentum;
    protected Rigidbody locomotion;

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
        if (UIManager.ActiveUI != null && UIManager.ActiveUI.inUI) return;
        stateMachine.RunFSM();
        GetMovement();

        float modSpeed = speed;
        if (running) modSpeed *= runBoost;

        if (transform.position.y < MapManager.World.seaLevel)
        {
            locomotion.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            AttachToGround();
        }
        else locomotion.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position += momentum * modSpeed * Time.deltaTime;        
        momentum.x = 0f;
        momentum.z = 0f;
        if (locomotion != null) locomotion.velocity = new Vector3(0f, locomotion.velocity.y, 0f);

        
    }

    protected virtual void AttachToGround()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        float y = MapManager.World.GetHeight(new Vector2(x, z));

        if (y < MapManager.World.seaLevel - (actorHeight * 0.5f))
            y = MapManager.World.seaLevel - (actorHeight * 0.5f);
        transform.position = new Vector3(x, y, z);
    }

    protected virtual void Initialize()
    {
        locomotion = GetComponent<Rigidbody>();
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
