using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� ������� ��������/���������� ������
/// </summary>
[RequireComponent(typeof(Image))]
public class BlinkEffect : MonoBehaviour
{
    [Header("��������� �������")]
    [SerializeField, Range(0.1f, 2f), Tooltip("������������ ������� ����������/���������")]
    private float fadeDuration = 0.5f;

    [SerializeField, Range(0f, 1f), Tooltip("�������� ����� ��������� � ��������� ����")]
    private float blinkDelay = 0.1f;

    [Header("������")]
    [SerializeField, Tooltip("����������� ��� ������� ����������")]
    private Image fadeImage;

    [Header("���������")]
    [Tooltip("����, ����������� ����� �� ��������� ����� ���������")]
    public bool CloseEyes { get; set; }

    [Tooltip("����, ����������� ��� ������ � ��������")]
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
    /// ���������� ������ � ��������� ���������
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
    /// ��������� ������ ��������/����������
    /// </summary>
    public void Blink()
    {
        if (IsBlinking) return;

        currentBlinkCoroutine = StartCoroutine(BlinkSequence());
    }

    /// <summary>
    /// ������������������ ������� ��������
    /// </summary>
    private IEnumerator BlinkSequence()
    {
        IsBlinking = true;

        // ���� ����������
        yield return FadeToColor(CloseEyes ? opaqueBlack : transparentBlack);

        // �������� ��� ������� �������� ����
        if (CloseEyes)
        {
            yield return new WaitForSeconds(blinkDelay);

            // ���� ���������
            yield return FadeToColor(transparentBlack);
        }

        IsBlinking = false;
        currentBlinkCoroutine = null;
    }

    /// <summary>
    /// ������� ��������� �����
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
    /// ��������� ������������� ������ (��� ��������)
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