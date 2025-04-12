using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    public string message; // Сообщение, которое будет показано при соприкосновении
    public CommentController commentController; // Ссылка на CommentController

    void Start()
    {
        // Найти объект CommentController в сцене
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Проверяем, что триггер активирован игроком
        {
            commentController.ShowComment(message); // Показываем сообщение
            Destroy(gameObject);
        }
    }
}
