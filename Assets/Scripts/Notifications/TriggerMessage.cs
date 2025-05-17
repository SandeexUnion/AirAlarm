using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private CommentController commentController;
    private NotificationMoveToCentr notificationMoveToCentrController;

    private void Start()
    {
        // ���� �� ���������� � ����������, ��������� ����� �������������
        if (notificationMoveToCentrController == null)
        {
            notificationMoveToCentrController = Object.FindFirstObjectByType<NotificationMoveToCentr>();

            if (notificationMoveToCentrController == null)
            {
                Debug.LogError("NotificationMoveToCentr �� ������ �� �����!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            commentController.ShowComment(message);
            notificationMoveToCentrController.ShowNotification();
            Destroy(gameObject);
        }
    }
}