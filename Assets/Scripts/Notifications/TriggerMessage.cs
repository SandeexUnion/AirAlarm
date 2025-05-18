using UnityEngine;

/// <summary>
/// ����� ��� ������ ��������� ��� ����� ������ � �������
/// </summary>
public class TriggerMessage : MonoBehaviour
{
    [Header("��������� ���������")]
    [SerializeField, Tooltip("����� ��������� ��� ������")]
    private string message;
    
    [Header("������ �� ����������")]
    [SerializeField, Tooltip("���������� ��� ����������� ������������")]
    private CommentController commentController;
    
    private NotificationMoveToCentr notificationMoveToCentrController;

    private void Start()
    {
        // �������������� ����� ����������� �����������, ���� �� ���������� �������
        if (notificationMoveToCentrController == null)
        {
            notificationMoveToCentrController = FindObjectOfType<NotificationMoveToCentr>();

            // �������������� ���� ��������� �� ������
            if (notificationMoveToCentrController == null)
            {
                Debug.LogError("NotificationMoveToCentr �� ������ �� �����! ����������� �� ����� ��������.");
            }
        }

        // �������� ������� ����������� ������������
        if (commentController == null)
        {
            Debug.LogError("CommentController �� ��������! ��������� �� ����� ������������.");
        }
    }

    /// <summary>
    /// ����������� ��� ����� ������� � �������
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // ���������, ��� ����� �����
        if (other.CompareTag("Player"))
        {
            // ���������� ��������� � �����������
            if (commentController != null)
            {
                commentController.ShowComment(message);
            }

            if (notificationMoveToCentrController != null)
            {
                notificationMoveToCentrController.ShowNotification();
            }

            // ���������� ������, ����� ��������� ������������ ������ ���� ���
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ������������� ����� ��������� ��� ��������
    /// </summary>
    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }

    /// <summary>
    /// ��������� ���������� ������������ �������
    /// </summary>
    public void SetCommentController(CommentController controller)
    {
        commentController = controller;
    }
}