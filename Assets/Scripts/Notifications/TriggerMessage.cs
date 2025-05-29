using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private UnityEvent onMessageHidden;

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
        if (other.CompareTag("Player"))
        {
            // �������� ��� ������� ��� GameObject
            commentController.ShowComment(message, gameObject);

            if (notificationMoveToCentrController != null)
            {
                notificationMoveToCentrController.ShowNotification();
            }

            
             
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