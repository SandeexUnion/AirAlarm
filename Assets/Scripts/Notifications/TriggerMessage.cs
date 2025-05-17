using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private CommentController commentController;
    private NotificationMoveToCentr notificationMoveToCentrController;

    private void Start()
    {
        // ≈сли не установлен в инспекторе, попробуем найти автоматически
        if (notificationMoveToCentrController == null)
        {
            notificationMoveToCentrController = Object.FindFirstObjectByType<NotificationMoveToCentr>();

            if (notificationMoveToCentrController == null)
            {
                Debug.LogError("NotificationMoveToCentr не найден на сцене!");
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