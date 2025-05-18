using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Класс для управления анимацией перемещения уведомления в центр экрана и обратно
/// </summary>
public class NotificationMoveToCentr : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform кнопки/уведомления для перемещения")]
    public RectTransform buttonRT;

    [Header("Animation Settings")]
    [SerializeField, Tooltip("Смещение для скрытия уведомления за пределами экрана")]
    private float hiddenOffset = 2000f;

    // Позиции для анимации
    private Vector2 startAnchoredPos;
    private Vector2 centerAnchoredPos;
    private Vector2 hiddenAnchoredPos;

    // Ссылка на контроллер паузы
    private PauseController pauseController;

    // Таймер для анимации
    private float time;

    // Singleton instance
    public static NotificationMoveToCentr Instance { get; private set; }

    /// <summary>
    /// Инициализация singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"Duplicate {nameof(NotificationMoveToCentr)} instance found. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Начальная инициализация позиций и скрытие уведомления
    /// </summary>
    private void Start()
    {
        pauseController = Object.FindFirstObjectByType<PauseController>();
        CalculatePositions();

        // Сразу прячем уведомление за пределами экрана
        buttonRT.anchoredPosition = hiddenAnchoredPos;
    }

    /// <summary>
    /// Вычисляет стартовую, центральную и скрытую позиции уведомления
    /// </summary>
    private void CalculatePositions()
    {
        // Сохраняем начальную позицию из редактора
        startAnchoredPos = buttonRT.anchoredPosition;

        // Центральная позиция (по X = 0, Y остается как в начальной позиции)
        centerAnchoredPos = new Vector2(0f, buttonRT.anchoredPosition.y);

        // Позиция за экраном (слева)
        hiddenAnchoredPos = new Vector2(-hiddenOffset, buttonRT.anchoredPosition.y);
    }

    /// <summary>
    /// Запускает анимацию показа уведомления (из скрытого состояния в центр)
    /// </summary>
    public void ShowNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Moving(hiddenAnchoredPos, centerAnchoredPos));
    }

    /// <summary>
    /// Запускает анимацию скрытия уведомления (из центра в скрытое состояние)
    /// </summary>
    public void HideNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Moving(centerAnchoredPos, hiddenAnchoredPos));
    }

    /// <summary>
    /// Корутина для плавного перемещения уведомления между позициями
    /// </summary>
    /// <param name="from">Начальная позиция</param>
    /// <param name="to">Конечная позиция</param>
    private IEnumerator Moving(Vector2 from, Vector2 to)
    {
        time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;
            buttonRT.anchoredPosition = Vector2.Lerp(from, to, time);
            yield return null;
        }

        // Гарантируем точное попадание в конечную позицию
        buttonRT.anchoredPosition = to;

        // Если уведомление достигло центра - ставим игру на паузу
        if (Vector2.Distance(buttonRT.anchoredPosition, centerAnchoredPos) < 3f)
        {
            pauseController?.Pause();
        }
    }

    /// <summary>
    /// Перезагружает текущую сцену после анимации скрытия уведомления
    /// </summary>
    public void RestartScene()
    {
        StartCoroutine(HideAndRestart());
    }

    /// <summary>
    /// Корутина для скрытия уведомления с последующей перезагрузкой сцены
    /// </summary>
    private IEnumerator HideAndRestart()
    {
        yield return StartCoroutine(Moving(centerAnchoredPos, hiddenAnchoredPos));

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}