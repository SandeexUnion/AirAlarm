using UnityEngine;

public class CrosshairGUI : MonoBehaviour
{
    public Drag d; // ������ �� ��������� Drag (�� ������������ � ������ ����)
    public Texture2D m_crosshairTexture; // �������� ��� ������������ �������
    public Texture2D m_useTexture; // �������� �������, ����� ����� �����������������
    public Texture2D m_usingTexture; // �������� �������, ����� �������������� ����������
    public float RayLength = 3f; // ����� ���� ��� �������� ��������������

    public static bool m_DefaultReticle; // ���� ��� ������������ �������
    public static bool m_UseReticle; // ���� ��� ������� ��������������
    public bool m_ShowCursor = false; // ���� ��� ����������� �������
    public bool m_UsingReticle; // ����, �����������, ������������ �� ������ ��������������
    public bool m_bIsCrosshairVisible = true; // ���� ��� ��������� �������
    private Rect m_crosshairRect; // ������������� ��� ���������� �������
    private Ray playerAim; // ���, ������������ ��� ����������� ����������� ������� ������
    private Camera playerCam; // ������ �� ������ ������

    void Update()
    {
        playerCam = Camera.main; // �������� �������� ������
        playerAim = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ������� ��� �� ������ ������
        RaycastHit hit; // ���������� ��� �������� ���������� � ������������

        // ���������, �������� �� ��� � �������
        if (Physics.Raycast(playerAim, out hit, RayLength))
        {
            // ���� ������ ����� ��� "Interact", "InteractItem" ��� "Door", ������������� ���� ��� ������������� �������
            if (hit.collider.gameObject.tag == "Interact" ||
                hit.collider.gameObject.tag == "InteractItem" ||
                hit.collider.gameObject.tag == "Door")
            {
                m_DefaultReticle = false; // ��������� ����������� ������
                m_UseReticle = true; // �������� ������ ��������������
            }
        }
        else
        {
            m_DefaultReticle = true; // �������� ����������� ������, ���� ������ �� ������
            m_UseReticle = false; // ��������� ������ ��������������
        }

        // ��������� ���������� �������
        if (m_ShowCursor)
        {
            Cursor.visible = true; // ���������� ������
            Cursor.lockState = CursorLockMode.None; // ������������ ������
        }
        else
        {
            Cursor.visible = false; // �������� ������
            Cursor.lockState = CursorLockMode.Locked; // ��������� ������ � ������ ������
        }

        // ���������� ������������� ��� ������������ �������
        if (m_DefaultReticle)
        {
            m_crosshairRect = new Rect((Screen.width - m_crosshairTexture.width) / 2,
                                        (Screen.height - m_crosshairTexture.height) / 2,
                                        m_crosshairTexture.width / 2,
                                        m_crosshairTexture.height / 2);
        }

        // ���������� ������������� ��� ������� ��������������
        if (m_UseReticle)
        {
            if (Input.GetMouseButton(0)) // ���� ������ ����� ������ ����
            {
                m_crosshairRect = new Rect((Screen.width - m_usingTexture.width) / 2,
                                            (Screen.height - m_usingTexture.height) / 2,
                                            m_usingTexture.width / 2,
                                            m_usingTexture.height / 2);
                m_UsingReticle = true; // ������������� ����, ��� ������ ������������
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
        if (m_bIsCrosshairVisible) // ���� ������ �����
        {
            if (m_DefaultReticle)
            {
                GUI.DrawTexture(m_crosshairRect, m_crosshairTexture); // ������ ����������� ������
            }
            if (m_UseReticle)
            {
                GUI.DrawTexture(m_crosshairRect, m_useTexture); // ������ ������ ��������������
            }
        }
    }
}
