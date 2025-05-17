using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(CharacterController))]
public class Moving : MonoBehaviour
{
    public float groundDistance = 2f;
    public float walkingSpeed = 7.5f;
    public float gravity = 20.0f;
    public GameObject playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public Rigidbody rb;
    public AudioSource steps;
    public PlayableDirector playableDirector;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isPlayingSteps = false;

    private CharacterController characterController;
    private MovementState currentState;
    private Vector3 _moveDirection;
    private float rotationX = 0;
    public PauseMenu pauseMenu;

    public Vector3 moveDirection
    {
        get => _moveDirection;
        set => _moveDirection = value;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        IsCursorLock(true);
        SetState(new GroundedState(this));
    }

    void Update()
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

        Vector3 origin = transform.position + Vector3.forward * 0.25f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundDistance);

        currentState.Update();

        characterController.Move(_moveDirection * walkingSpeed * Time.deltaTime);

        if (!pauseMenu.isPauseMenuShowing && canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.isPauseMenuShowing)
                pauseMenu.HidePouseMenu();
            else
                pauseMenu.ShowPouseMenu();
        }
    }

    public void SetState(MovementState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void IsCursorLock(bool isCursorLock)
    {
        if (isCursorLock == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
