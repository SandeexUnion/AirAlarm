using UnityEngine;

/// <summary>
/// Абстрактный базовый класс для всех состояний движения персонажа
/// </summary>
public abstract class MovementState
{
    protected readonly Moving context;

    /// <summary>
    /// Конструктор базового класса состояния
    /// </summary>
    /// <param name="context">Ссылка на контроллер движения</param>
    public MovementState(Moving context)
    {
        this.context = context;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
}

/// <summary>
/// Состояние нахождения на земле (ходьба, бег, приседание)
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
    /// Ползанье
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
    /// Управление
    /// </summary>
    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        context.moveDirection = context.transform.TransformDirection(inputDirection);
    }

    /// <summary>
    /// Звуки шагов
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
/// Состояние нахождения в воздухе (прыжок, падение)
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
/// Состояние во время катсцены (движение заблокировано)
/// </summary>
public class CutsceneState : MovementState
{
    public CutsceneState(Moving context) : base(context) { }

    public override void Update() { }
}