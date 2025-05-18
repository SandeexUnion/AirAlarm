using UnityEngine;

/// <summary>
/// ��������� ������������ � ���������� ������� (crosshair) � ����
/// </summary>
public class CrosshairGUI : MonoBehaviour
{
    [Header("Textures")]
    [Tooltip("�������� �������� �������")]
    [SerializeField] private Texture2D defaultCrosshair;
    [Tooltip("�������� ������� ��� ��������� �� ������������� ������")]
    [SerializeField] private Texture2D interactCrosshair;

    [Header("Settings")]
    [Tooltip("������������ ��������� ��� ��������������")]
    [SerializeField] private float raycastDistance = 1.5f;
    [Tooltip("���������� �� ��������� ������ ����")]
    [SerializeField] private bool showSystemCursor = false;

    [Header("Interaction Tags")]
    [Tooltip("���� ������������� ��������")]
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
    /// ���������, ��������� �� ������������� ������ � ���� ��������������
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
    /// ���������, �������� �� ��� ������� �������������
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
    /// ������������ ��������������� ������ �� ������
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
    /// ��������� ��������� ���������� �������
    /// </summary>
    private void UpdateCursorVisibility()
    {
        Cursor.visible = showSystemCursor;
        Cursor.lockState = showSystemCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// ������������� ��������� ��������������
    /// </summary>
    public void SetRaycastDistance(float distance)
    {
        raycastDistance = Mathf.Max(0.1f, distance);
    }

    /// <summary>
    /// ��������/��������� ����������� ���������� �������
    /// </summary>
    public void ToggleSystemCursor(bool show)
    {
        showSystemCursor = show;
        UpdateCursorVisibility();
    }
}