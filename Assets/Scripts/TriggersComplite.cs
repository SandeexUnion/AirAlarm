using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���������� ��������� ���������� ������, �������������� ������/���������
/// � ����������� ������������ ��������������� ���������.
/// </summary>
public class TriggersComplite : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CommentController commentController; // ���������� ����������� ������������
    [SerializeField] private TriggerMessage triggerMessage;      // ���������� ���������

    [Header("Settings")]
    [SerializeField] private bool isWinTrigger = true;           // ���� �������� ������
    [SerializeField] private string winMessage = "������� �������!"; // ��������� ��� ������
    [SerializeField] private string failMessage = "���������� ��� ���"; // ��������� ��� ���������

    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorInLevel = true;      // ����������� ������ � ������
    [SerializeField] private bool unlockCursorInMenu = true;     // �������������� ������ � ����

    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;                // ������ �� ������ ������

    /// <summary>
    /// ���������� ����� ������� � �������
    /// </summary>
    /// <param name="other">��������� ��������� �������</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������������� ��������������� ��������� � ����������� �� ���� ��������
            string message = isWinTrigger ? winMessage : failMessage;
            triggerMessage.SetMessage(message);

            // ������������� ��� ��� ����������� ���������
            triggerMessage.gameObject.tag = isWinTrigger ? "Completed" : "Failure";

            // ���������� ��������� ����� ������� ������������
            commentController.ShowComment(message, gameObject);
        }
    }

    /// <summary>
    /// ��������� ��������� ������� � ����������� ���������� �������
    /// </summary>
    /// <param name="sceneNum">����� ����� ��� ��������</param>
    public void LoadLevel(int sceneNum)
    {
        // ���������� �������� ��� �������� �������� ����
        if (sceneNum == 0) // ������� ����
        {
            if (unlockCursorInMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else // ������� �������
        {
            if (lockCursorInLevel)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // �������� ��������� �����
        SceneManager.LoadScene(sceneNum);
    }
}