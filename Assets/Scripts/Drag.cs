using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

// Класс для хранения параметров захвата объектов
[System.Serializable]
public class GrabObjectClass
{
    public bool m_FreezeRotation; // Замораживать ли вращение объекта
    public float m_PickupRange = 1f; // Диапазон захвата
    public float m_ThrowStrength = 10f; // Сила броска
    public float m_distance = 1f; // Расстояние для удержания
    public float m_maxDistanceGrab = 1f; // Максимальное расстояние захвата
}

// Класс для хранения параметров захвата предметов
[System.Serializable]
public class ItemGrabClass
{
    public bool m_FreezeRotation; // Замораживать ли вращение предмета
    public float m_ItemPickupRange = 2f; // Диапазон захвата предмета
    public float m_ItemThrow = 45f; // Сила броска предмета
    public float m_ItemDistance = 1f; // Расстояние для удержания предмета
    public float m_ItemMaxGrab = 2.5f; // Максимальное расстояние захвата предмета
}

// Класс для хранения параметров захвата дверей
[System.Serializable]
public class DoorGrabClass
{
    public float m_DoorPickupRange = 1f; // Диапазон захвата двери
    public float m_DoorThrow = 10f; // Сила броска двери
    public float m_DoorDistance = 20f; // Расстояние для удержания двери
    public float m_DoorMaxGrab = 1f; // Максимальное расстояние захвата двери
}

// Класс для хранения тегов
[System.Serializable]
public class TagsClass
{
    public string m_InteractTag = "Interact"; // Тег для объектов, с которыми можно взаимодействовать
    public string m_InteractItemsTag = "InteractItem"; // Тег для предметов, с которыми можно взаимодействовать
    public string m_DoorsTag = "Door"; // Тег для дверей
}

public class Drag : MonoBehaviour
{
    public PlayableDirector timeline; // Ссылка на временной директор
    public BlinkEffect bf; // Эффект моргания

    public GameObject playerCam; // Ссылка на камеру игрока
    public AudioManager audioManager; // Ссылка на менеджер звука
    public string GrabButton = "Grab"; // Кнопка захвата
    public string ThrowButton = "Throw"; // Кнопка броска
    public string UseButton = "Use"; // Кнопка использования
    public GrabObjectClass ObjectGrab = new GrabObjectClass(); // Параметры захвата объектов
    public ItemGrabClass ItemGrab = new ItemGrabClass(); // Параметры захвата предметов
    public DoorGrabClass DoorGrab = new DoorGrabClass(); // Параметры захвата дверей
    public TagsClass Tags = new TagsClass(); // Теги для взаимодействия
    public GameObject bed; // Ссылка на объект кровати

    private float PickupRange = 1f; // Текущий диапазон захвата
    private float ThrowStrength = 2f; // Текущая сила броска
    public float distance = 1f; // Расстояние для удержания
    private float maxDistanceGrab = 1f; // Максимальное расстояние захвата
    public Moving moving; // Ссылка на компонент перемещения

    public float reducedMouseSensitivity = 0.1f; // Пониженная чувствительность мыши

    public GameObject objectHeld; // Ссылка на текущий захваченный объект
    public bool isObjectHeld; // Флаг, указывающий, захвачен ли объект
    private bool tryPickupObject; // Флаг, указывающий, пытаюсь ли я захватить объект

    void Start()
    {
        // Инициализация переменных
        moving = GetComponent<Moving>(); // Получаем компонент перемещения
        isObjectHeld = false; // Устанавливаем флаг захвата в false
        tryPickupObject = false; // Устанавливаем флаг попытки захвата в false
        objectHeld = null; // Сбрасываем ссылку на захваченный объект
    }

    void FixedUpdate()
    {
        // Отображаем луч для отладки
        Debug.DrawRay(transform.position, Vector3.forward * maxDistanceGrab);

        // Проверяем, нажата ли кнопка захвата
        if (Input.GetButton(GrabButton))
        {
            if (!isObjectHeld) // Если объект не захвачен
            {
                tryPickObject(); // Пытаемся захватить объект
                tryPickupObject = true; // Устанавливаем флаг попытки захвата
            }
            else // Если объект уже захвачен
            {
                holdObject(); // Удерживаем объект
            }
        }
        else if (isObjectHeld) // Если объект захвачен и кнопка не нажата
        {
            DropObject(); // Сбрасываем объект
        }

        // Проверяем, нажата ли кнопка броска
        if (Input.GetButton(ThrowButton) && isObjectHeld)
        {
            isObjectHeld = false; // Устанавливаем флаг захвата в false
            objectHeld.GetComponent<Rigidbody>().useGravity = true; // Включаем гравитацию для объекта
            ThrowObject(); // Бросаем объект
        }

        // Проверяем, нажата ли кнопка использования
        if (Input.GetButton(UseButton))
        {
            isObjectHeld = false; // Устанавливаем флаг захвата в false
            tryPickObject(); // Пытаемся захватить объект
            tryPickupObject = false; // Сбрасываем флаг попытки захвата
            Use(); // Используем объект
        }
    }

    private IEnumerator WaitAndExecute()
    {
        // Жду 7 секунд перед выполнением действий
        yield return new WaitForSeconds(7f);

        // Открываю глаза объекта
        bf.CloseEyes = false;
        timeline.Play(); // Запускаю временную шкалу
        audioManager.PlayNextTrack(); // Воспроизводим следующий трек
    }

    private void tryPickObject()
    {
        // Создаю луч для определения объекта, на который я смотрю
        Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit; // Переменная для хранения информации о столкновении

        // Выполняю лучевой запрос
        if (Physics.Raycast(playerAim, out hit))
        {
            objectHeld = hit.collider.gameObject; // Сохраняю ссылку на объект, на который я навел

            // Вычисляю расстояние до объекта
            float distanceToPlayer = Vector3.Distance(objectHeld.transform.position, playerCam.transform.position);

            // Проверяю, в пределах ли я диапазона захвата двери
            if (distanceToPlayer > DoorGrab.m_DoorPickupRange)
            {
                return; // Если слишком далеко, выхожу из метода
            }

            // Проверяю, имеет ли объект тег взаимодействия
            if (hit.collider.tag == Tags.m_InteractTag)
            {
                // Специальная проверка для объекта "Old_wood_bed"
                if (hit.collider.gameObject.name == "Old_wood_bed")
                {
                    CrosshairGUI.m_DefaultReticle = true; // Меняю прицел
                    CrosshairGUI.m_UseReticle = false; // Убираю использование прицела
                    bf.CloseEyes = true; // Закрываю глаза
                    hit.collider.gameObject.tag = "Untagged"; // Убираю тег у объекта
                    StartCoroutine(WaitAndExecute()); // Запускаю корутину
                }

                // Проверка для захвата объектов
                if (CrosshairGUI.m_UseReticle)
                {
                    isObjectHeld = true; // Устанавливаю флаг захвата в true
                    objectHeld.GetComponent<Rigidbody>().useGravity = false; // Отключаю гравитацию

                    // Если нужно, замораживаю вращение объекта
                    if (ObjectGrab.m_FreezeRotation)
                    {
                        objectHeld.GetComponent<Rigidbody>().freezeRotation = true;
                    }

                    // Устанавливаю параметры захвата
                    PickupRange = ObjectGrab.m_PickupRange;
                    ThrowStrength = ObjectGrab.m_ThrowStrength;
                    distance = ObjectGrab.m_PickupRange / 2; // Устанавливаю расстояние для удержания
                    maxDistanceGrab = ObjectGrab.m_maxDistanceGrab; // Устанавливаю максимальное расстояние захвата
                }
            }

            // Проверка для захвата предметов
            if (hit.collider.tag == Tags.m_InteractTag && tryPickupObject)
            {
                isObjectHeld = true; // Устанавливаю флаг захвата в true
                objectHeld.GetComponent<Rigidbody>().useGravity = true; // Включаю гравитацию

                // Если нужно, замораживаю вращение предмета
                if (ItemGrab.m_FreezeRotation)
                {
                    objectHeld.GetComponent<Rigidbody>().freezeRotation = true;
                }

                // Устанавливаю параметры захвата предмета
                PickupRange = ItemGrab.m_ItemPickupRange;
                ThrowStrength = ItemGrab.m_ItemThrow;
                distance = ItemGrab.m_ItemDistance;
                maxDistanceGrab = ItemGrab.m_ItemMaxGrab;
            }

            // Проверка для захвата дверей
            if (hit.collider.tag == Tags.m_DoorsTag && tryPickupObject)
            {
                // Проверка расстояния до двери
                if (distanceToPlayer <= DoorGrab.m_DoorPickupRange) // Замените maxDistanceGrab на DoorGrab.m_DoorPickupRange
                {
                    if (!isObjectHeld) // Убедитесь, что объект еще не захвачен
                    {
                        isObjectHeld = true; // Устанавливаю флаг захвата в true
                        objectHeld.GetComponent<Rigidbody>().useGravity = true; // Включаю гравитацию
                        objectHeld.GetComponent<Rigidbody>().freezeRotation = false; // Размораживаю вращение

                        // Устанавливаю параметры для захвата двери
                        PickupRange = DoorGrab.m_DoorPickupRange;
                        ThrowStrength = DoorGrab.m_DoorThrow;
                        distance = DoorGrab.m_DoorDistance;
                        maxDistanceGrab = DoorGrab.m_DoorMaxGrab;
                    }
                }
            }
        }
    }

    private void holdObject()
    {
        // Создаю луч для определения направления удержания
        Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.LogWarning("hold1");
        Vector3 nextPos = playerCam.transform.position + playerAim.direction * distance; // Вычисляю следующую позицию
        Vector3 currPos = objectHeld.transform.position; // Получаю текущую позицию объекта
        Debug.DrawRay(playerAim.origin, playerAim.direction * PickupRange, Color.red); // Отображаю луч для отладки
        objectHeld.GetComponent<Rigidbody>().linearVelocity = (nextPos - currPos) * 10; // Устанавливаю скорость для удержания
        Debug.LogWarning("hold2");
        // Проверяю, не вышел ли объект за пределы максимального расстояния захвата
        if (Vector3.Distance(objectHeld.transform.position, playerCam.transform.position) > maxDistanceGrab)
        {
            DropObject(); // Сбрасываю объект, если он слишком далеко
            Debug.LogWarning("hold3");
        }
    }

    private void DropObject()
    {
        // Сбрасываю объект
        isObjectHeld = false; // Устанавливаю флаг захвата в false
        tryPickupObject = false; // Сбрасываю флаг попытки захвата
        objectHeld.GetComponent<Rigidbody>().useGravity = true; // Включаю гравитацию для объекта
        objectHeld.GetComponent<Rigidbody>().freezeRotation = false; // Размораживаю вращение
        objectHeld = null; // Сбрасываю ссылку на объект
    }

    private void ThrowObject()
    {
        // Бросаю объект
        objectHeld.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * ThrowStrength); // Применяю силу броска
        objectHeld.GetComponent<Rigidbody>().freezeRotation = false; // Размораживаю вращение
        objectHeld = null; // Сбрасываю ссылку на объект
    }

    private void Use()
    {
        // Использую объект
        objectHeld.SendMessage("UseObject", SendMessageOptions.DontRequireReceiver); // Вызываю метод UseObject на объекте
        objectHeld = null; // Сбрасываю ссылку на объект
    }
}
