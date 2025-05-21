using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ���������� �������� ���������, ����������� �������������, ������� � �����������
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Moving : MonoBehaviour
{
    [Header("��������� ��������")]
    [SerializeField] private float groundDistance = 2f;
    [SerializeField] public float walkingSpeed = 7.5f;
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [Header("��������� ����������")]
    [SerializeField] public float crouchSpeed = 2f;
    [SerializeField] public float crouchHeight = 0.5f;
    [SerializeField] public float standHeight = 1.0f;
    



    [Header("������ �� ����������")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] public AudioSource steps;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("��������� ��������")]
    [Tooltip("��������� �� �������� �� �����")]
    [HideInInspector] public bool isGrounded;

    [Tooltip("��������� �� �������� � ������ ������")]
    [HideInInspector] public bool isMoving;

    [Tooltip("����� �� �������� � ������ ������")]
    [HideInInspector] public bool isCrouching;

    [Tooltip("����� �� �������� ���������")]
    [HideInInspector] public bool canMove = true;

    [Tooltip("������������� �� ����� �����")]
    [HideInInspector] public bool isPlayingSteps = false;

    public CharacterController characterController;
    private MovementState currentState;
    private float rotationX = 0;
    private Vector3 _moveDirection;

    /// <summary>
    /// ����������� �������� ���������
    /// </summary>
    public Vector3 moveDirection
    {
        get => _moveDirection;
        set => _moveDirection = value;
    }

    /// <summary>
    /// ������������� �����������
    /// </summary>
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        IsCursorLock(true);
        SetState(new GroundedState(this));
    }

    /// <summary>
    /// ���������� ������ �������� ������ ����
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
    /// ��������� ��������� �� ����� ��������
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
    /// ��������, ��������� �� �������� �� �����
    /// </summary>
    private void CheckGroundStatus()
    {
        Vector3 origin = transform.position + Vector3.forward * 0.25f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundDistance);
    }

    /// <summary>
    /// ���������� �������� ���������
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
    /// ��������� �������� ������
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
    /// ��������� ����� ��� �����
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
    /// ��������� ������ ��������� ��������
    /// </summary>
    /// <param name="newState">����� ���������</param>
    public void SetState(MovementState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// ���������� ���������� �������
    /// </summary>
    /// <param name="isCursorLock">������������� �� ������</param>
    public void IsCursorLock(bool isCursorLock)
    {
        Cursor.lockState = isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLock;
    }
}