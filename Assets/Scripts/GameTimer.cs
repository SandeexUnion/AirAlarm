using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Таймер игры, отслеживающий оставшееся время и визуализирующий его
/// с помощью круглого заполняющегося индикатора с изменением цвета.
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float totalTime = 30f; // Общее время для отсчета
    [SerializeField] private Image pieImage;        // Изображение для кругового таймера (FillMethod = Radial360)

    [Header("Цвета")]
    [SerializeField] private Color startColor = Color.green; // Начальный цвет индикатора
    [SerializeField] private Color endColor = Color.red;    // Конечный цвет индикатора

    private float currentTime; // Текущее оставшееся время

    /// <summary>
    /// Инициализация компонента при старте
    /// </summary>
    void Start()
    {
        currentTime = totalTime;

        // Настраиваем изображение для кругового отображения
        if (pieImage != null)
        {
            pieImage.type = Image.Type.Filled;
            pieImage.fillMethod = Image.FillMethod.Radial360;
            pieImage.fillOrigin = (int)Image.Origin360.Top;
            pieImage.fillClockwise = false;
        }
    }

    /// <summary>
    /// Обновление состояния таймера каждый кадр
    /// </summary>
    void Update()
    {
        // Уменьшаем время с учетом реального времени
        currentTime -= Time.deltaTime;

        // Обновляем визуальное представление
        UpdateTimer();

        // Проверка окончания времени
        if (currentTime <= 0)
        {
            currentTime = 0;
            // Перезагружаем текущую сцену при истечении времени
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Обновление визуального представления таймера
    /// </summary>
    void UpdateTimer()
    {
        // Выходим если нет ссылки на изображение
        if (pieImage == null) return;

        // Вычисляем прогресс (от 1 до 0)
        float progress = currentTime / totalTime;

        // Устанавливаем заполнение индикатора
        pieImage.fillAmount = progress;

        // Интерполируем цвет от начального к конечному
        pieImage.color = Color.Lerp(endColor, startColor, progress);
    }
}