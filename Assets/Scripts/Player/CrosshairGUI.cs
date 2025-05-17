using UnityEngine;

public class CrosshairGUI : MonoBehaviour
{
    [Header("Textures")]
    public Texture2D defaultCrosshair;  // Обычный курсор
    public Texture2D interactCrosshair; // курсор при наведении

    [Header("Settings")]
    public float raycastDistance = 1.5f;  // Дистанция взаимодействия
    public bool showCursor = false;     // Показывать ли системный курсор

    private Rect crosshairRect;
    public bool isLookingAtInteractable;

    void Update()
    {
        // Проверяем, смотрим ли мы на интерактивный объект
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            isLookingAtInteractable = hit.collider.CompareTag("Interact") ||
                                      hit.collider.CompareTag("InteractItem") ||
                                      hit.collider.CompareTag("Door");
        }
        else
        {
            isLookingAtInteractable = false;
        }
    }

    void OnGUI()
    {
        // Выбираем нужную текстуру
        Texture2D currentTexture = isLookingAtInteractable ? interactCrosshair : defaultCrosshair;

        if (currentTexture != null)
        {
            // Рассчитываем позицию курсора по центру экрана
            crosshairRect = new Rect(
                (Screen.width - currentTexture.width) / 2,
                (Screen.height - currentTexture.height) / 2,
                currentTexture.width,
                currentTexture.height
            );

            // Рисуем текстуру
            GUI.DrawTexture(crosshairRect, currentTexture);
        }
    }
}