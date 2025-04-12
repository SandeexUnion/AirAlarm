using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Moving))]
public class Moving : MonoBehaviour
{
    public float groundDistance = 2f; // Расстояние для определения, на земле ли игрок
    bool isGrounded; // Флаг, указывающий, находится ли игрок на земле
    public float walkingSpeed = 7.5f; // Скорость ходьбы игрока
    public float gravity = 20.0f; // Сила гравитации
    public GameObject playerCamera; // Ссылка на камеру игрока
    public float lookSpeed = 2.0f; // Скорость поворота камеры
    public float lookXLimit = 45.0f; // Ограничение по вертикальному углу поворота камеры
    public bool isMoving; // Флаг, указывающий, движется ли игрок
    public Rigidbody rb; // Ссылка на компонент Rigidbody
    public AudioSource steps; // Ссылка на аудиоисточник для звука шагов
    public Animator anim; // Ссылка на аниматор
    CharacterController characterController; // Ссылка на компонент CharacterController
    Vector3 moveDirection = Vector3.zero; // Направление движения игрока
    float rotationX = 0; // Угол поворота по оси X
    Drag drag = new Drag(); // Ссылка на класс Drag (не используется в этом контексте)
    private bool isPlayingSteps = false; // Флаг, указывающий, воспроизводится ли звук шагов
    public bool canMove = true; // Флаг, разрешающий движение
    public PlayableDirector playableDirector; // Ссылка на PlayableDirector

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Получаем компонент CharacterController
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор
        Cursor.visible = false; // Скрываем курсор
    }

    void Update()
    {
        if (playableDirector.state == PlayState.Playing)
        {
            canMove = false; // Запрещаем движение, если Timeline воспроизводится
            isMoving = false; // Устанавливаем isMoving в false
        }
        else
        {
            isMoving = true;
            canMove = true; // Разрешаем движение, если Timeline не воспроизводится
        }
        // Определяем, на земле ли игрок
        Vector3 origin = transform.position + Vector3.forward * 0.25f; // Начальная точка для луча
        isGrounded = Physics.Raycast(origin, Vector3.down, groundDistance); // Проверяем, касается ли игрок земли
        Debug.DrawRay(origin, Vector3.down * groundDistance, isGrounded ? Color.green : Color.red); // Отображаем луч для отладки

        if (isGrounded)
        {
            // Получаем ввод от игрока
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontal, 0f, vertical); // Создаем вектор направления движения
            moveDirection = transform.TransformDirection(moveDirection); // Преобразуем вектор в мировые координаты

            // Проверка на движение и воспроизведение звука шагов
            if (moveDirection.magnitude > 0.1f && !isPlayingSteps)
            {
                steps.Play(); // Воспроизводим звук шагов
                isPlayingSteps = true; // Устанавливаем флаг, что звук шагов воспроизводится
            }
            else if (moveDirection.magnitude <= 0.1f && isPlayingSteps)
            {
                steps.Stop(); // Останавливаем звук шагов
                isPlayingSteps = false; // Сбрасываем флаг, когда движение прекращается
            }
        }

        // Применяем гравитацию, если игрок не на земле
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime; // Уменьшаем высоту движения на значение гравитации
        }

        characterController.Move(moveDirection * Time.deltaTime); // Перемещаем игрока

        // Обработка поворота камеры и игрока
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; // Поворачиваем камеру по оси Y
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Ограничиваем угол поворота
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Применяем поворот к камере
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // Поворачиваем игрока по оси Y
        }

        // Изменение высоты персонажа при нажатии клавиш
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            characterController.height /= 2; // Уменьшаем высоту персонажа
        }
        else
        {
            characterController.height = 0.5f; // Устанавливаем стандартную высоту
        }
    }
}
