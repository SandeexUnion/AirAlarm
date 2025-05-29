using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private UnityEvent onMessageHidden;

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
        if (other.CompareTag("Player"))
        {
            // Передаем сам триггер как GameObject
            commentController.ShowComment(message, gameObject);

            if (notificationMoveToCentrController != null)
            {
                notificationMoveToCentrController.ShowNotification();
            }

            
             
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