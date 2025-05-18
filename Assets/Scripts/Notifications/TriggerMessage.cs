using UnityEngine;

/// <summary>
/// Класс для вывода сообщения при входе игрока в триггер
/// </summary>
public class TriggerMessage : MonoBehaviour
{
    [Header("Настройки сообщения")]
    [SerializeField, Tooltip("Текст сообщения для показа")]
    private string message;
    
    [Header("Ссылки на компоненты")]
    [SerializeField, Tooltip("Контроллер для отображения комментариев")]
    private CommentController commentController;
    
    private NotificationMoveToCentr notificationMoveToCentrController;

    private void Start()
    {
        // Автоматический поиск контроллера уведомлений, если не установлен вручную
        if (notificationMoveToCentrController == null)
        {
            notificationMoveToCentrController = FindObjectOfType<NotificationMoveToCentr>();

            // Предупреждение если компонент не найден
            if (notificationMoveToCentrController == null)
            {
                Debug.LogError("NotificationMoveToCentr не найден на сцене! Уведомления не будут работать.");
            }
        }

        // Проверка наличия контроллера комментариев
        if (commentController == null)
        {
            Debug.LogError("CommentController не назначен! Сообщения не будут отображаться.");
        }
    }

    /// <summary>
    /// Срабатывает при входе объекта в триггер
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел игрок
        if (other.CompareTag("Player"))
        {
            // Показываем сообщение и уведомление
            if (commentController != null)
            {
                commentController.ShowComment(message);
            }

            if (notificationMoveToCentrController != null)
            {
                notificationMoveToCentrController.ShowNotification();
            }

            // Уничтожаем объект, чтобы сообщение показывалось только один раз
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Устанавливает новое сообщение для триггера
    /// </summary>
    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }

    /// <summary>
    /// Назначает контроллер комментариев вручную
    /// </summary>
    public void SetCommentController(CommentController controller)
    {
        commentController = controller;
    }
}