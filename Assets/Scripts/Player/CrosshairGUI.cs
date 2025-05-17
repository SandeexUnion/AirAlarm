using UnityEngine;

public class CrosshairGUI : MonoBehaviour
{
    [Header("Textures")]
    public Texture2D defaultCrosshair;  // ������� ������
    public Texture2D interactCrosshair; // ������ ��� ���������

    [Header("Settings")]
    public float raycastDistance = 1.5f;  // ��������� ��������������
    public bool showCursor = false;     // ���������� �� ��������� ������

    private Rect crosshairRect;
    public bool isLookingAtInteractable;

    void Update()
    {
        // ���������, ������� �� �� �� ������������� ������
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
        // �������� ������ ��������
        Texture2D currentTexture = isLookingAtInteractable ? interactCrosshair : defaultCrosshair;

        if (currentTexture != null)
        {
            // ������������ ������� ������� �� ������ ������
            crosshairRect = new Rect(
                (Screen.width - currentTexture.width) / 2,
                (Screen.height - currentTexture.height) / 2,
                currentTexture.width,
                currentTexture.height
            );

            // ������ ��������
            GUI.DrawTexture(crosshairRect, currentTexture);
        }
    }
}