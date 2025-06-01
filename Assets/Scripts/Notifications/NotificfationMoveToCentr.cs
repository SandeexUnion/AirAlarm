using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер анимации перемещения уведомления в центр экрана и обратно
/// с автоматической паузой игры при показе уведомления.
/// </summary>
public class NotificationMoveToCentr : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform кнопки/уведомления для перемещения")]
    public RectTransform buttonRT;                          // Трансформ уведомления для анимации
    [Tooltip("Canvas, к которому принадлежит уведомление (для расчета границ)")]
    public Canvas canvas;                                   // Родительский канвас

    [Header("Animation Settings")]
    [SerializeField, Tooltip("Длительность анимации в секундах")]
    private float animationDuration = 0.5f;                 // Длительность анимации перемещения

    private Vector2 startAnchoredPos;                       // Начальная позиция уведомления
    private Vector2 centerAnchoredPos;                      // Центральная позиция на экране
    private Vector2 hiddenAnchoredPos;                      // Позиция за пределами экрана

    private PauseController pauseController;                // Контроллер паузы игры
    private Coroutine currentAnimation;                     // Текущая выполняемая анимация

    /// <summary>
    /// Статический экземпляр для реализации синглтона
    /// </summary>
    public static NotificationMoveToCentr Instance { get; private set; }

    /// <summary>
    /// Инициализация синглтона при загрузке объекта
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
    /// Начальная инициализация компонента
    /// </summary>
    private void Start()
    {
        canvas = buttonRT.parent.GetComponent<Canvas>();
        pauseController = FindObjectOfType<PauseController>();
        CalculatePositions();
        buttonRT.anchoredPosition = hiddenAnchoredPos; // Сразу скрываем
    }

    /// <summary>
    /// Вычисляет ключевые позиции для анимации
    /// </summary>
    private void CalculatePositions()
    {
        startAnchoredPos = buttonRT.anchoredPosition;
        centerAnchoredPos = new Vector2(0f, buttonRT.anchoredPosition.y);

        // Рассчитываем позицию за экраном с учетом ширины канваса и самого уведомления
        float screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        float notificationWidth = buttonRT.rect.width;
        hiddenAnchoredPos = new Vector2(-(screenWidth + notificationWidth), buttonRT.anchoredPosition.y);
    }

    #region Управление уведомлением

    /// <summary>
    /// Показывает уведомление с анимацией перемещения в центр
    /// </summary>
    public void ShowNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        // Если уже есть анимация, прерываем её и мгновенно скрываем
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            buttonRT.anchoredPosition = hiddenAnchoredPos;
        }

        currentAnimation = StartCoroutine(MoveAnimation(hiddenAnchoredPos, centerAnchoredPos));
    }

    /// <summary>
    /// Скрывает уведомление с анимацией перемещения за экран
    /// </summary>
    public void HideNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        // Прерываем текущую анимацию (если есть) и запускаем скрытие
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(MoveAnimation(buttonRT.anchoredPosition, hiddenAnchoredPos));
    }

    #endregion

    #region Анимация перемещения

    /// <summary>
    /// Корутина для плавного перемещения уведомления между позициями
    /// </summary>
    /// <param name="from">Начальная позиция</param>
    /// <param name="to">Конечная позиция</param>
    private IEnumerator MoveAnimation(Vector2 from, Vector2 to)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            buttonRT.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        buttonRT.anchoredPosition = to;
        currentAnimation = null;

        // Если уведомление в центре — ставим игру на паузу
        if (Vector2.Distance(buttonRT.anchoredPosition, centerAnchoredPos) < 0.1f)
        {
            pauseController?.Pause();
        }
    }

    #endregion
}