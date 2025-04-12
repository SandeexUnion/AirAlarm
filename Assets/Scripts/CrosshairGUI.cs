using UnityEngine;

public class CrosshairGUI : MonoBehaviour
{
    public Drag d; // Ссылка на компонент Drag (не используется в данном коде)
    public Texture2D m_crosshairTexture; // Текстура для стандартного прицела
    public Texture2D m_useTexture; // Текстура прицела, когда можно взаимодействовать
    public Texture2D m_usingTexture; // Текстура прицела, когда взаимодействие происходит
    public float RayLength = 3f; // Длина луча для проверки взаимодействия

    public static bool m_DefaultReticle; // Флаг для стандартного прицела
    public static bool m_UseReticle; // Флаг для прицела взаимодействия
    public bool m_ShowCursor = false; // Флаг для отображения курсора
    public bool m_UsingReticle; // Флаг, указывающий, используется ли прицел взаимодействия
    public bool m_bIsCrosshairVisible = true; // Флаг для видимости прицела
    private Rect m_crosshairRect; // Прямоугольник для размещения прицела
    private Ray playerAim; // Луч, используемый для определения направления взгляда игрока
    private Camera playerCam; // Ссылка на камеру игрока

    void Update()
    {
        playerCam = Camera.main; // Получаем основную камеру
        playerAim = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Создаем луч из центра экрана
        RaycastHit hit; // Переменная для хранения информации о столкновении

        // Проверяем, попадает ли луч в объекты
        if (Physics.Raycast(playerAim, out hit, RayLength))
        {
            // Если объект имеет тег "Interact", "InteractItem" или "Door", устанавливаем флаг для использования прицела
            if (hit.collider.gameObject.tag == "Interact" ||
                hit.collider.gameObject.tag == "InteractItem" ||
                hit.collider.gameObject.tag == "Door")
            {
                m_DefaultReticle = false; // Отключаем стандартный прицел
                m_UseReticle = true; // Включаем прицел взаимодействия
            }
        }
        else
        {
            m_DefaultReticle = true; // Включаем стандартный прицел, если ничего не попали
            m_UseReticle = false; // Отключаем прицел взаимодействия
        }

        // Управляем видимостью курсора
        if (m_ShowCursor)
        {
            Cursor.visible = true; // Показываем курсор
            Cursor.lockState = CursorLockMode.None; // Разблокируем курсор
        }
        else
        {
            Cursor.visible = false; // Скрываем курсор
            Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор в центре экрана
        }

        // Определяем прямоугольник для стандартного прицела
        if (m_DefaultReticle)
        {
            m_crosshairRect = new Rect((Screen.width - m_crosshairTexture.width) / 2,
                                        (Screen.height - m_crosshairTexture.height) / 2,
                                        m_crosshairTexture.width / 2,
                                        m_crosshairTexture.height / 2);
        }

        // Определяем прямоугольник для прицела взаимодействия
        if (m_UseReticle)
        {
            if (Input.GetMouseButton(0)) // Если нажата левая кнопка мыши
            {
                m_crosshairRect = new Rect((Screen.width - m_usingTexture.width) / 2,
                                            (Screen.height - m_usingTexture.height) / 2,
                                            m_usingTexture.width / 2,
                                            m_usingTexture.height / 2);
                m_UsingReticle = true; // Устанавливаем флаг, что прицел используется
            }
            else
            {
                m_crosshairRect = new Rect((Screen.width - m_useTexture.width) / 2,
                                            (Screen.height - m_useTexture.height) / 2,
                                            m_useTexture.width / 2,
                                            m_useTexture.height / 2);
            }
        }
    }

    void OnGUI()
    {
        if (m_bIsCrosshairVisible) // Если прицел видим
        {
            if (m_DefaultReticle)
            {
                GUI.DrawTexture(m_crosshairRect, m_crosshairTexture); // Рисуем стандартный прицел
            }
            if (m_UseReticle)
            {
                GUI.DrawTexture(m_crosshairRect, m_useTexture); // Рисуем прицел взаимодействия
            }
        }
    }
}
