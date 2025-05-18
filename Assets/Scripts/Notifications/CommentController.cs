using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер для отображения комментариев/уведомлений в игре.
/// Позволяет показывать текстовые сообщения с возможностью скрытия по нажатию Space.
/// Интегрирован с системой паузы и контроллером уведомлений.
/// </summary>
public class CommentController : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("UI Text компонент для отображения комментария")]
    [SerializeField]
    private Text commentText;

    [Tooltip("Длительность отображения комментария (если не скрыт вручную)")]
    [SerializeField]
    private float displayDuration = 2f;

    [Header("Internal References")]
    private NotificationMoveToCentr notificationMoveToCentrController;
    private PauseController pauseController;

    [Header("State Tracking")]
    private bool isDisplaying = false;

    /// <summary>
    /// Флаг, указывающий отображается ли в данный момент комментарий
    /// </summary>
    public bool IsDisplaying => isDisplaying;

    /// <summary>
    /// Инициализация компонента при старте
    /// </summary>
    private void Start()
    {
        InitializeDependencies();
        SetupInitialState();
    }

    /// <summary>
    /// Поиск и кэширование необходимых зависимостей
    /// </summary>
    private void InitializeDependencies()
    {
        notificationMoveToCentrController = Object.FindFirstObjectByType<NotificationMoveToCentr>();
        pauseController = Object.FindFirstObjectByType<PauseController>();
    }

    /// <summary>
    /// Установка начального состояния UI
    /// </summary>
    private void SetupInitialState()
    {
        commentText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Обработка ввода пользователя каждый кадр
    /// </summary>
    private void Update()
    {
        HandleUserInput();
    }

    /// <summary>
    /// Обработка нажатия Space для скрытия комментария
    /// </summary>
    private void HandleUserInput()
    {
        if (isDisplaying && Input.GetKeyUp(KeyCode.Space))
        {
            HideComment();
        }
    }

    /// <summary>
    /// Публичный метод для отображения комментария
    /// </summary>
    /// <param name="comment">Текст комментария для отображения</param>
    public void ShowComment(string comment)
    {
        if (!isDisplaying)
        {
            DisplayComment(comment);
        }
    }

    /// <summary>
    /// Внутренний метод для отображения комментария
    /// </summary>
    /// <param name="comment">Текст комментария</param>
    private void DisplayComment(string comment)
    {
        isDisplaying = true;
        commentText.text = comment;
        commentText.gameObject.SetActive(true);

        // Запускаем корутину для автоматического скрытия
        StartCoroutine(AutoHideCommentAfterDelay());
    }

    /// <summary>
    /// Скрытие комментария с восстановлением игры
    /// </summary>
    private void HideComment()
    {
        pauseController.Resume();
        isDisplaying = false;
        notificationMoveToCentrController.HideNotification();
        commentText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Корутина для автоматического скрытия комментария через заданное время
    /// </summary>
    private IEnumerator AutoHideCommentAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (isDisplaying)
        {
            HideComment();
        }
    }
}