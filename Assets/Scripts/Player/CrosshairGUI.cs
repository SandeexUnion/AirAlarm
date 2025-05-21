using UnityEngine;

/// <summary>
/// Управляет отображением и поведением прицела (crosshair) в игре
/// </summary>
public class CrosshairGUI : MonoBehaviour
{
    [Header("Textures")]
    [Tooltip("Текстура обычного прицела")]
    [SerializeField] private Texture2D defaultCrosshair;
    [Tooltip("Текстура прицела при наведении на интерактивный объект")]
    [SerializeField] private Texture2D interactCrosshair;
    [Tooltip("Текстура прицела при наведении на закрытую дверь")]
    [SerializeField] private Texture2D lockedDoorCrosshair;

    [Header("Settings")]
    [Tooltip("Максимальная дистанция для взаимодействия")]
    [SerializeField] private float raycastDistance = 1.5f;
    [Tooltip("Показывать ли системный курсор мыши")]
    [SerializeField] private bool showSystemCursor = false;

    [Header("Interaction Tags")]
    [Tooltip("Теги интерактивных объектов")]
    [SerializeField] private string[] interactableTags = { "Interact", "InteractItem", "Door" };
    [Tooltip("Тег закрытой двери")]
    [SerializeField] private string lockedDoorTag = "LockedDoor";

    private Rect crosshairRect;
    private Camera mainCamera;
    public bool isLookingAtInteractable;
    public bool isLookingAtLockedDoor;

    private void Awake()
    {
        mainCamera = Camera.main;
        UpdateCursorVisibility();
    }

    private void Update()
    {
        CheckInteractableObjects();
    }

    private void OnGUI()
    {
        DrawCrosshair();
    }

    /// <summary>
    /// Проверяет, находится ли интерактивный объект в зоне взаимодействия
    /// </summary>
    private void CheckInteractableObjects()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            // Сначала проверяем на закрытую дверь
            isLookingAtLockedDoor = hit.collider.CompareTag(lockedDoorTag);
            // Затем проверяем другие интерактивные объекты
            isLookingAtInteractable = !isLookingAtLockedDoor && IsInteractableTag(hit.collider.tag);
        }
        else
        {
            isLookingAtInteractable = false;
            isLookingAtLockedDoor = false;
        }
    }

    /// <summary>
    /// Проверяет, является ли тег объекта интерактивным
    /// </summary>
    private bool IsInteractableTag(string tag)
    {
        foreach (string interactableTag in interactableTags)
        {
            if (tag == interactableTag)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Отрисовывает соответствующий прицел на экране
    /// </summary>
    private void DrawCrosshair()
    {
        // Приоритет отдается курсору закрытой двери
        Texture2D currentTexture = isLookingAtLockedDoor ? lockedDoorCrosshair :
                                (isLookingAtInteractable ? interactCrosshair : defaultCrosshair);

        if (currentTexture != null)
        {
            crosshairRect = new Rect(
                (Screen.width - currentTexture.width) * 0.5f,
                (Screen.height - currentTexture.height) * 0.5f,
                currentTexture.width,
                currentTexture.height
            );

            GUI.DrawTexture(crosshairRect, currentTexture);
        }
    }

    /// <summary>
    /// Обновляет видимость системного курсора
    /// </summary>
    private void UpdateCursorVisibility()
    {
        Cursor.visible = showSystemCursor;
        Cursor.lockState = showSystemCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// Устанавливает дистанцию взаимодействия
    /// </summary>
    public void SetRaycastDistance(float distance)
    {
        raycastDistance = Mathf.Max(0.1f, distance);
    }

    /// <summary>
    /// Включает/выключает отображение системного курсора
    /// </summary>
    public void ToggleSystemCursor(bool show)
    {
        showSystemCursor = show;
        UpdateCursorVisibility();
    }
}