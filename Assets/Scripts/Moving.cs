using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Moving))]
public class Moving : MonoBehaviour
{
    public float groundDistance = 2f; // ���������� ��� �����������, �� ����� �� �����
    bool isGrounded; // ����, �����������, ��������� �� ����� �� �����
    public float walkingSpeed = 7.5f; // �������� ������ ������
    public float gravity = 20.0f; // ���� ����������
    public GameObject playerCamera; // ������ �� ������ ������
    public float lookSpeed = 2.0f; // �������� �������� ������
    public float lookXLimit = 45.0f; // ����������� �� ������������� ���� �������� ������
    public bool isMoving; // ����, �����������, �������� �� �����
    public Rigidbody rb; // ������ �� ��������� Rigidbody
    public AudioSource steps; // ������ �� ������������� ��� ����� �����
    public Animator anim; // ������ �� ��������
    CharacterController characterController; // ������ �� ��������� CharacterController
    Vector3 moveDirection = Vector3.zero; // ����������� �������� ������
    float rotationX = 0; // ���� �������� �� ��� X
    Drag drag = new Drag(); // ������ �� ����� Drag (�� ������������ � ���� ���������)
    private bool isPlayingSteps = false; // ����, �����������, ��������������� �� ���� �����
    public bool canMove = true; // ����, ����������� ��������
    public PlayableDirector playableDirector; // ������ �� PlayableDirector

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // �������� ��������� CharacterController
        Cursor.lockState = CursorLockMode.Locked; // ��������� ������
        Cursor.visible = false; // �������� ������
    }

    void Update()
    {
        if (playableDirector.state == PlayState.Playing)
        {
            canMove = false; // ��������� ��������, ���� Timeline ���������������
            isMoving = false; // ������������� isMoving � false
        }
        else
        {
            isMoving = true;
            canMove = true; // ��������� ��������, ���� Timeline �� ���������������
        }
        // ����������, �� ����� �� �����
        Vector3 origin = transform.position + Vector3.forward * 0.25f; // ��������� ����� ��� ����
        isGrounded = Physics.Raycast(origin, Vector3.down, groundDistance); // ���������, �������� �� ����� �����
        Debug.DrawRay(origin, Vector3.down * groundDistance, isGrounded ? Color.green : Color.red); // ���������� ��� ��� �������

        if (isGrounded)
        {
            // �������� ���� �� ������
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontal, 0f, vertical); // ������� ������ ����������� ��������
            moveDirection = transform.TransformDirection(moveDirection); // ����������� ������ � ������� ����������

            // �������� �� �������� � ��������������� ����� �����
            if (moveDirection.magnitude > 0.1f && !isPlayingSteps)
            {
                steps.Play(); // ������������� ���� �����
                isPlayingSteps = true; // ������������� ����, ��� ���� ����� ���������������
            }
            else if (moveDirection.magnitude <= 0.1f && isPlayingSteps)
            {
                steps.Stop(); // ������������� ���� �����
                isPlayingSteps = false; // ���������� ����, ����� �������� ������������
            }
        }

        // ��������� ����������, ���� ����� �� �� �����
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime; // ��������� ������ �������� �� �������� ����������
        }

        characterController.Move(moveDirection * Time.deltaTime); // ���������� ������

        // ��������� �������� ������ � ������
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; // ������������ ������ �� ��� Y
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // ������������ ���� ��������
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // ��������� ������� � ������
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // ������������ ������ �� ��� Y
        }

        // ��������� ������ ��������� ��� ������� ������
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            characterController.height /= 2; // ��������� ������ ���������
        }
        else
        {
            characterController.height = 0.5f; // ������������� ����������� ������
        }
    }
}
