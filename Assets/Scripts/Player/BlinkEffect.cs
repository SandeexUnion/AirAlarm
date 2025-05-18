using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер эффекта моргания/затемнения экрана
/// </summary>
[RequireComponent(typeof(Image))]
public class BlinkEffect : MonoBehaviour
{
    [Header("Настройки эффекта")]
    [SerializeField, Range(0.1f, 2f), Tooltip("Длительность эффекта затемнения/появления")]
    private float fadeDuration = 0.5f;

    [SerializeField, Range(0f, 1f), Tooltip("Задержка между закрытием и открытием глаз")]
    private float blinkDelay = 0.1f;

    [Header("Ссылки")]
    [SerializeField, Tooltip("Изображение для эффекта затемнения")]
    private Image fadeImage;

    [Header("Состояние")]
    [Tooltip("Флаг, указывающий нужно ли закрывать глаза полностью")]
    public bool CloseEyes { get; set; }

    [Tooltip("Флаг, указывающий что эффект в процессе")]
    public bool IsBlinking { get; private set; }

    private Coroutine currentBlinkCoroutine;
    private Color transparentBlack = new Color(0, 0, 0, 0);
    private Color opaqueBlack = new Color(0, 0, 0, 1);

    private void Awake()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }

        ResetEffect();
    }

    /// <summary>
    /// Сбрасывает эффект в начальное состояние
    /// </summary>
    public void ResetEffect()
    {
        fadeImage.color = transparentBlack;
        CloseEyes = false;
        IsBlinking = false;

        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
        }
    }

    /// <summary>
    /// Запускает эффект моргания/затемнения
    /// </summary>
    public void Blink()
    {
        if (IsBlinking) return;

        currentBlinkCoroutine = StartCoroutine(BlinkSequence());
    }

    /// <summary>
    /// Последовательность эффекта моргания
    /// </summary>
    private IEnumerator BlinkSequence()
    {
        IsBlinking = true;

        // Фаза затемнения
        yield return FadeToColor(CloseEyes ? opaqueBlack : transparentBlack);

        // Задержка для эффекта закрытых глаз
        if (CloseEyes)
        {
            yield return new WaitForSeconds(blinkDelay);

            // Фаза появления
            yield return FadeToColor(transparentBlack);
        }

        IsBlinking = false;
        currentBlinkCoroutine = null;
    }

    /// <summary>
    /// Плавное изменение цвета
    /// </summary>
    private IEnumerator FadeToColor(Color targetColor)
    {
        Color startColor = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = targetColor;
    }

    /// <summary>
    /// Мгновенно устанавливает эффект (без анимации)
    /// </summary>
    public void SetEffectImmediate(bool shouldCloseEyes)
    {
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
        }

        CloseEyes = shouldCloseEyes;
        fadeImage.color = shouldCloseEyes ? opaqueBlack : transparentBlack;
        IsBlinking = false;
    }
}