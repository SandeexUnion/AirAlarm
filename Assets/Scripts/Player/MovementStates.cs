using UnityEngine;

/// <summary>
/// ����������� ������� ����� ��� ���� ��������� �������� ���������
/// </summary>
public abstract class MovementState
{
    protected readonly Moving context;

    /// <summary>
    /// ����������� �������� ������ ���������
    /// </summary>
    /// <param name="context">������ �� ���������� ��������</param>
    public MovementState(Moving context)
    {
        this.context = context;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
}

/// <summary>
/// ��������� ���������� �� ����� (������, ���, ����������)
/// </summary>
public class GroundedState : MovementState
{
    private const float CrouchHeight = 0.25f;
    private const float StandHeight = 0.5f;
    private const float MovementThreshold = 0.1f;
    private CharacterController characterController;

    public GroundedState(Moving context) : base(context)
    {
        characterController = context.GetComponent<CharacterController>();
    }

    public override void Update()
    {
        HandleCrouchInput();
        HandleMovementInput();
        HandleFootstepSounds();
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            context.isCrouching = !context.isCrouching;

            float targetScale = context.isCrouching ? context.crouchHeight : 1f;
            context.transform.localScale = new Vector3(1f, targetScale, 1f);

            //context.characterController.height = targetScale;
        }
    }
    /// <summary>
    /// ����������
    /// </summary>
    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        context.moveDirection = context.transform.TransformDirection(inputDirection);
    }

    /// <summary>
    /// ����� �����
    /// </summary>

    private void HandleFootstepSounds()
    {
        bool isMoving = context.moveDirection.magnitude > MovementThreshold;

        if (isMoving && !context.isPlayingSteps)
        {
            context.steps.Play();
            context.isPlayingSteps = true;
        }
        else if (!isMoving && context.isPlayingSteps)
        {
            context.steps.Stop();
            context.isPlayingSteps = false;
        }
    }

   
}

/// <summary>
/// ��������� ���������� � ������� (������, �������)
/// </summary>
public class AirborneState : MovementState
{
    public AirborneState(Moving context) : base(context) { }

    public override void Update()
    {
        Vector3 newDirection = CalculateMovement();
        newDirection.y = context.moveDirection.y - context.gravity * Time.deltaTime;
        context.moveDirection = newDirection;
    }

    private Vector3 CalculateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        return context.transform.TransformDirection(inputDirection);
    }
}

/// <summary>
/// ��������� �� ����� �������� (�������� �������������)
/// </summary>
public class CutsceneState : MovementState
{
    public CutsceneState(Moving context) : base(context) { }

    public override void Update() { }
}