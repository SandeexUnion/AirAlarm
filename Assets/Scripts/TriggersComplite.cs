using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggersComplite : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CommentController commentController;
    [SerializeField] private TriggerMessage triggerMessage;

    [Header("Settings")]
    [SerializeField] private bool isWinTrigger = true;
    [SerializeField] private string winMessage = "Уровень пройден!";
    [SerializeField] private string failMessage = "Попробуйте еще раз";

    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorInLevel = true;
    [SerializeField] private bool unlockCursorInMenu = true;
    [Header("Timer")]
    [SerializeField] private GameTimer gameTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Устанавливаем соответствующее сообщение
            string message = isWinTrigger ? winMessage : failMessage;
            triggerMessage.SetMessage(message);

            // Показываем сообщение через TriggerMessage
            triggerMessage.gameObject.tag = isWinTrigger ? "Completed" : "Failure";
            commentController.ShowComment(message, gameObject);
        }
    }



    public void LoadLevel(int sceneNum)
    {
        // Управление курсором
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

        SceneManager.LoadScene(sceneNum);
    }
}