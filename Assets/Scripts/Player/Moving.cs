using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс, отвечающий за управление движением персонажа, включая перемещение, прыжки и ползания.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Moving : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float groundDistance = 2f;
    [SerializeField] public float walkingSpeed = 7.5f;
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [Header("Настройки ползания")]
    [SerializeField] public float crouchSpeed = 2f;
    [SerializeField] public float crouchHeight = 0.5f;
    [SerializeField] public float standHeight = 1.0f;


    [Header("Состояние ввода")]
    [Tooltip("Отключает все вводимые команды (включая движение)")]
    [HideInInspector] public bool isAllInputDisabled = false;

    [Header("Камера")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] public AudioSource steps;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("Состояние персонажа")]
    [Tooltip("Проверяет, находится ли персонаж на земле")]
    [HideInInspector] public bool isGrounded;
    [Tooltip("Проверяет, движется ли персонаж")]
    [HideInInspector] public bool isMoving;
    [Tooltip("Проверяет, crouching ли персонаж")]
    [HideInInspector] public bool isCrouching;
    [Tooltip("Проверяет, может ли персонаж двигаться")]
    [HideInInspector] public bool canMove = true;
    [Tooltip("Проверяет, воспроизводит ли персонаж звуки шагов")]
    [HideInInspector] public bool isPlayingSteps = false;

    public CharacterController characterController;
    private MovementState currentState;
    private float rotationX = 0;
    private Vector3 _moveDirection;

    /// <summary>
    /// Движение персонажа
    /// </summary>
    public Vector3 moveDirection
    {
        get => _moveDirection;
        set => _moveDirection = value;
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        IsCursorLock(true);
        SetState(new GroundedState(this));
        if (SceneManager.GetActiveScene().buildIndex != 0) // Не главное меню
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Обновление состояния каждый кадр
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
    /// Обработка состояния кат-сцены
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
        if (!isAllInputDisabled)
        {
            currentState?.Update();
            characterController.Move(_moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Отключение всех вводимых команд
    /// </summary>
    public void DisableAllInput()
    {
        isAllInputDisabled = true;
        canMove = false;
        _moveDirection = Vector3.zero; 

        
        characterController.enabled = false;

        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Включение всех вводимых команд
    /// </summary>
    public void EnableAllInput()
    {
        
        characterController.enabled = true;

        
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        isAllInputDisabled = false;
        canMove = true;

        
        rotationX = playerCamera.transform.localEulerAngles.x;
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
        if (!pauseMenu.isPauseMenuShowing && canMove && !isAllInputDisabled)
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
    /// Установка блокировки курсора
    /// </summary>
    /// <param name="isCursorLock">Блокировка курсора</param>
    public void IsCursorLock(bool isCursorLock)
    {
        // Игнорируем запросы блокировки, если UI активен (например, меню паузы)
        if (FindObjectOfType<PauseMenu>()?.isPauseMenuShowing == true)
            return;

        Cursor.lockState = isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLock;

        // Для отладки
        Debug.Log($"Cursor lock set to: {Cursor.lockState}, visible: {Cursor.visible}");
    }
}