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

    [Header("Settings")]
    [Tooltip("Максимальная дистанция для взаимодействия")]
    [SerializeField] private float raycastDistance = 1.5f;
    [Tooltip("Показывать ли системный курсор мыши")]
    [SerializeField] private bool showSystemCursor = false;

    [Header("Interaction Tags")]
    [Tooltip("Теги интерактивных объектов")]
    [SerializeField] private string[] interactableTags = { "Interact", "InteractItem", "Door" };

    private Rect crosshairRect;
    public bool isLookingAtInteractable;
    private Camera mainCamera;

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
        isLookingAtInteractable = Physics.Raycast(ray, out RaycastHit hit, raycastDistance) &&
                                 IsInteractableTag(hit.collider.tag);
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
        Texture2D currentTexture = isLookingAtInteractable ? interactCrosshair : defaultCrosshair;

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