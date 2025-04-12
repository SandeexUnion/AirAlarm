using UnityEngine;
using UnityEngine.UI; // Для использования UI
using TMPro; // Если используете TextMeshPro
using System.Collections;

public class CommentController : MonoBehaviour
{
    public Text commentText; // Для обычного текста
    // public TextMeshProUGUI commentText; // Для TextMeshPro
    public float displayDuration = 2f; // Время отображения комментария
    private bool isDisplaying = false; // Флаг для отслеживания состояния отображения

    public bool IsDisplaying => isDisplaying; // Свойство для доступа к флагу

    void Start()
    {
        commentText.gameObject.SetActive(false); // Скрыть текст в начале
    }

    public void ShowComment(string comment)
    {
        if (!isDisplaying) // Проверяем, отображается ли текст в данный момент
        {
            StartCoroutine(DisplayComment(comment));
        }
    }

    private IEnumerator DisplayComment(string comment)
    {
        isDisplaying = true; // Устанавливаем флаг отображения
        commentText.text = comment; // Установить текст комментария
        commentText.gameObject.SetActive(true); // Показать текст
        yield return new WaitForSeconds(displayDuration); // Ждать заданное время
        commentText.gameObject.SetActive(false); // Скрыть текст
        isDisplaying = false; // Сбрасываем флаг отображения
    }
}


