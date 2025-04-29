// CrosshairGUI.cs
using UnityEngine;

public class CrosshairGUI : MonoBehaviour
{
    [Header("Textures")]
    public Texture2D m_crosshairTexture;
    public Texture2D m_useTexture;
    public Texture2D m_usingTexture;

    [Header("Settings")]
    public float RayLength = 3f;
    public bool m_ShowCursor = false;
    public bool m_bIsCrosshairVisible = true;

    public static bool m_DefaultReticle;
    public static bool m_UseReticle;
    public bool m_UsingReticle;

    private Rect m_crosshairRect;
    private Camera playerCam;

    void Update()
    {
        playerCam = Camera.main;
        Ray playerAim = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(playerAim, out hit, RayLength))
        {
            string tag = hit.collider.gameObject.tag;
            m_DefaultReticle = !(tag == "Interact" || tag == "InteractItem" || tag == "Door");
            m_UseReticle = !m_DefaultReticle;
        }
        else
        {
            m_DefaultReticle = true;
            m_UseReticle = false;
        }

        Cursor.visible = m_ShowCursor;
        Cursor.lockState = m_ShowCursor ? CursorLockMode.None : CursorLockMode.Locked;

        UpdateCrosshairRect();
    }

    private void UpdateCrosshairRect()
    {
        Texture2D texture = m_DefaultReticle ? m_crosshairTexture :
                          (Input.GetMouseButton(0) ? m_usingTexture : m_useTexture);

        if (texture != null)
        {
            m_crosshairRect = new Rect(
                (Screen.width - texture.width) / 2,
                (Screen.height - texture.height) / 2,
                texture.width / 2,
                texture.height / 2
            );
        }
    }

    void OnGUI()
    {
        if (m_bIsCrosshairVisible)
        {
            if (m_DefaultReticle && m_crosshairTexture != null)
            {
                GUI.DrawTexture(m_crosshairRect, m_crosshairTexture);
            }
            else if (m_UseReticle && m_useTexture != null)
            {
                GUI.DrawTexture(m_crosshairRect, m_useTexture);
            }
        }
    }
}
