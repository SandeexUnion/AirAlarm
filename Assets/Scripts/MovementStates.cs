using UnityEngine;

public abstract class MovementState
{
    protected Moving context;

    public MovementState(Moving context) => this.context = context;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
}

public class GroundedState : MovementState
{
    public GroundedState(Moving context) : base(context) { }

    public override void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        context.moveDirection = context.transform.TransformDirection(inputDirection);

        if (context.moveDirection.magnitude > 0.1f && !context.isPlayingSteps)
        {
            context.steps.Play();
            context.isPlayingSteps = true;
        }
        else if (context.moveDirection.magnitude <= 0.1f && context.isPlayingSteps)
        {
            context.steps.Stop();
            context.isPlayingSteps = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
            context.GetComponent<CharacterController>().height = 0.25f;
        else
            context.GetComponent<CharacterController>().height = 0.5f;
    }
}

public class AirborneState : MovementState
{
    public AirborneState(Moving context) : base(context) { }

    public override void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        Vector3 currentDirection = context.moveDirection;
        currentDirection.x = context.transform.TransformDirection(inputDirection).x;
        currentDirection.z = context.transform.TransformDirection(inputDirection).z;
        currentDirection.y -= context.gravity * Time.deltaTime;

        context.moveDirection = currentDirection;
    }
}

public class CutsceneState : MovementState
{
    public CutsceneState(Moving context) : base(context) { }
    public override void Update() { }
}