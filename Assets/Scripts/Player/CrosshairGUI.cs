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
    [Tooltip("�������� ������� ��� ��������� �� �������� �����")]
    [SerializeField] private Texture2D lockedDoorCrosshair;

    [Header("Settings")]
    [Tooltip("������������ ��������� ��� ��������������")]
    [SerializeField] private float raycastDistance = 1.5f;
    [Tooltip("���������� �� ��������� ������ ����")]
    [SerializeField] private bool showSystemCursor = false;

    [Header("Interaction Tags")]
    [Tooltip("���� ������������� ��������")]
    [SerializeField] private string[] interactableTags = { "Interact", "InteractItem", "Door" };
    [Tooltip("��� �������� �����")]
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
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            // ������� ��������� �� �������� �����
            isLookingAtLockedDoor = hit.collider.CompareTag(lockedDoorTag);
            // ����� ��������� ������ ������������� �������
            isLookingAtInteractable = !isLookingAtLockedDoor && IsInteractableTag(hit.collider.tag);
        }
        else
        {
            isLookingAtInteractable = false;
            isLookingAtLockedDoor = false;
        }
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
        // ��������� �������� ������� �������� �����
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