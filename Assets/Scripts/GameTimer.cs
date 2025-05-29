using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float totalTime = 30f; // Общее время
    [SerializeField] private Image pieImage; // Изображение для таймера (должно быть с FillMethod = Radial360)

    [Header("Цвета")]
    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color endColor = Color.red;

    private float currentTime;

    void Start()
    {
        currentTime = totalTime;

        // Настраиваем изображение
        if (pieImage != null)
        {
            pieImage.type = Image.Type.Filled;
            pieImage.fillMethod = Image.FillMethod.Radial360;
            pieImage.fillOrigin = (int)Image.Origin360.Top;
            pieImage.fillClockwise = false;
        }
    }

    void Update()
    {
        // Уменьшаем время
        currentTime -= Time.deltaTime;

        // Обновляем отображение
        UpdateTimer();

        // Проверка окончания времени
        if (currentTime <= 0)
        {
            currentTime = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void UpdateTimer()
    {
        if (pieImage == null) return;

        // Вычисляем прогресс (от 1 до 0)
        float progress = currentTime / totalTime;

        // Устанавливаем заполнение
        pieImage.fillAmount = progress;

        // Меняем цвет
        pieImage.color = Color.Lerp(endColor, startColor, progress);
    }
}