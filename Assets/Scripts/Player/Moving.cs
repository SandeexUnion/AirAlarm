using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Контроллер движения персонажа, управляющий передвижением, камерой и состояниями
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Moving : MonoBehaviour
{
    [Header("Параметры движения")]
    [SerializeField] private float groundDistance = 2f;
    [SerializeField] public float walkingSpeed = 7.5f;
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [Header("Параметры приседания")]
    [SerializeField] public float crouchSpeed = 2f;
    [SerializeField] public float crouchHeight = 0.5f;
    [SerializeField] public float standHeight = 1.0f;
    



    [Header("Ссылки на компоненты")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] public AudioSource steps;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("Состояния движения")]
    [Tooltip("Находится ли персонаж на земле")]
    [HideInInspector] public bool isGrounded;

    [Tooltip("Двигается ли персонаж в данный момент")]
    [HideInInspector] public bool isMoving;

    [Tooltip("Сидит ли персонаж в данный момент")]
    [HideInInspector] public bool isCrouching;

    [Tooltip("Может ли персонаж двигаться")]
    [HideInInspector] public bool canMove = true;

    [Tooltip("Проигрываются ли звуки шагов")]
    [HideInInspector] public bool isPlayingSteps = false;

    public CharacterController characterController;
    private MovementState currentState;
    private float rotationX = 0;
    private Vector3 _moveDirection;

    /// <summary>
    /// Направление движения персонажа
    /// </summary>
    public Vector3 moveDirection
    {
        get => _moveDirection;
        set => _moveDirection = value;
    }

    /// <summary>
    /// Инициализация компонентов
    /// </summary>
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        IsCursorLock(true);
        SetState(new GroundedState(this));
    }

    /// <summary>
    /// Обновление логики движения каждый кадр
    /// </summary>
    private void Update()
    {
        HandleCutsceneState();
        CheckGroundStatus();
        UpdateMovement();
        HandleCameraRotation();
        HandlePauseInput();
        CheckCrouchObstruction();
    }

    /// <summary>
    /// Обработка состояния во время катсцены
    /// </summary>
    private void HandleCutsceneState()
    {
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            canMove = false;
            isMoving = false;
            SetState(new CutsceneState(this));
        }
        else if (currentState is CutsceneState)
        {
            canMove = true;
            isMoving = true;
            SetState(new GroundedState(this));
        }
    }

    /// <summary>
    /// Проверка, находится ли персонаж на земле
    /// </summary>
    private void CheckGroundStatus()
    {
        Vector3 origin = transform.position + Vector3.forward * 0.25f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundDistance);
    }

    /// <summary>
    /// Обновление движения персонажа
    /// </summary>
    private void UpdateMovement()
    {
        currentState?.Update();
        characterController.Move(_moveDirection * walkingSpeed * Time.deltaTime);
    }

    private void CheckCrouchObstruction()
    {
        if (!isCrouching && Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (Physics.Raycast(transform.position, Vector3.up, standHeight - crouchHeight + 0.1f))
            {
                isCrouching = true;
            }
        }
    }

    /// <summary>
    /// Обработка вращения камеры
    /// </summary>
    private void HandleCameraRotation()
    {
        if (!pauseMenu.isPauseMenuShowing && canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    /// <summary>
    /// Обработка ввода для паузы
    /// </summary>
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.isPauseMenuShowing)
                pauseMenu.HidePauseMenu();
            else
                pauseMenu.ShowPauseMenu();
        }
    }

    /// <summary>
    /// Установка нового состояния движения
    /// </summary>
    /// <param name="newState">Новое состояние</param>
    public void SetState(MovementState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// Управление состоянием курсора
    /// </summary>
    /// <param name="isCursorLock">Заблокировать ли курсор</param>
    public void IsCursorLock(bool isCursorLock)
    {
        Cursor.lockState = isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLock;
    }
}