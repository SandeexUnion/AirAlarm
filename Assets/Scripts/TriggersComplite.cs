using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер триггеров завершения уровня, обрабатывающий победу/поражение
/// и управляющий отображением соответствующих сообщений.
/// </summary>
public class TriggersComplite : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CommentController commentController; // Контроллер отображения комментариев
    [SerializeField] private TriggerMessage triggerMessage;      // Контроллер сообщений

    [Header("Settings")]
    [SerializeField] private bool isWinTrigger = true;           // Флаг триггера победы
    [SerializeField] private string winMessage = "Уровень пройден!"; // Сообщение при победе
    [SerializeField] private string failMessage = "Попробуйте еще раз"; // Сообщение при поражении

    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorInLevel = true;      // Блокировать курсор в уровне
    [SerializeField] private bool unlockCursorInMenu = true;     // Разблокировать курсор в меню

    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;                // Ссылка на таймер уровня

    /// <summary>
    /// Обработчик входа объекта в триггер
    /// </summary>
    /// <param name="other">Коллайдер вошедшего объекта</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Устанавливаем соответствующее сообщение в зависимости от типа триггера
            string message = isWinTrigger ? winMessage : failMessage;
            triggerMessage.SetMessage(message);

            // Устанавливаем тег для последующей обработки
            triggerMessage.gameObject.tag = isWinTrigger ? "Completed" : "Failure";

            // Показываем сообщение через систему комментариев
            commentController.ShowComment(message, gameObject);
        }
    }

    /// <summary>
    /// Загружает указанный уровень с управлением состоянием курсора
    /// </summary>
    /// <param name="sceneNum">Номер сцены для загрузки</param>
    public void LoadLevel(int sceneNum)
    {
        // Управление курсором при загрузке главного меню
        if (sceneNum == 0) // Главное меню
        {
            if (unlockCursorInMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else // Игровой уровень
        {
            if (lockCursorInLevel)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Загрузка указанной сцены
        SceneManager.LoadScene(sceneNum);
    }
}