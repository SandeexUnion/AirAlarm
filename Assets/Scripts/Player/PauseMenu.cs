using UnityEngine;

/// <summary>
/// ���������� ���� �����, ����������� ������������/�������� ������ �����
/// � ����������������� � ��������� ���������� ������ � ���������.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("�������� ����������")]
    [SerializeField] private GameObject menuPanel;          // ������ ���� �����
    [SerializeField] private PauseController pauseController; // ���������� ����� ����
    [SerializeField] private Moving moving;                // ���������� �������� ������

    [Header("��������� ����")]
    [Tooltip("����, ����������� ������������ �� � ������ ������ ���� �����")]
    public bool isPauseMenuShowing;                        // ������� ��������� ��������� ����

    /// <summary>
    /// ������������� ��� ������
    /// </summary>
    private void Start()
    {
        // �������� ���� ����� ��� ������
        menuPanel.SetActive(false);
        isPauseMenuShowing = false;
    }

    /// <summary>
    /// ���������� ������ ���� (��������� ��� ���������� ���������� �����������)
    /// </summary>
    private void Update()
    {
        // ����� �������� ������ �������� ����� ��� ��������/�������� �����
    }

    #region ���������� ���������� ���� �����

    /// <summary>
    /// ���������� ���� ����� � ������ ���� �� �����
    /// </summary>
    public void ShowPauseMenu()
    {
        isPauseMenuShowing = true;
        menuPanel.SetActive(true);

        // ������������ ������ ��� �����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseController.Pause();
    }

    /// <summary>
    /// �������� ���� ����� � ������������ ����
    /// </summary>
    public void HidePauseMenu()
    {
        isPauseMenuShowing = false;
        menuPanel.SetActive(false);

        // ��������� ������ ��� �������� � ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseController.Resume();
    }

    #endregion

    #region ����������� ������ ����

    /// <summary>
    /// ���������� ������ "����������"
    /// </summary>
    public void OnContinueButtonClicked()
    {
        HidePauseMenu();
    }

    /// <summary>
    /// ���������� ������ "� ������� ����"
    /// </summary>
    public void OnMainMenuButtonClicked()
    {
        // ����� ������ ���� ���������� �������� �������� ����
        Debug.Log("Loading main menu...");
    }

    /// <summary>
    /// ���������� ������ "�����"
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}