using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// ����� ������� ������������, ������������ ��������� � ������ ����� ������
/// � ������� ��������� � ��� ��������� �������� �������
/// </summary>
public class NewCommentSystem : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform commentPanel;
    [SerializeField] private TMP_Text commentText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float bottomMargin = 20f; // ������ �� ������� ����

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private Coroutine currentAnimation;
    private bool isDisplaying = false;

    private void Awake()
    {
        // ������������ ������� ������
        CalculatePositions();
        commentPanel.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
    }

    private void CalculatePositions()
    {
        // ������������� ��������� Layout ����� ���������
        LayoutRebuilder.ForceRebuildLayoutImmediate(commentPanel);

        float panelHeight = commentPanel.rect.height;

        // ������� ������: ������ ���� + ������
        shownPosition = new Vector2(0f, bottomMargin);

        // ������� �������: ��������� �� �������
        hiddenPosition = new Vector2(0f, -panelHeight - bottomMargin);
    }

    /// <summary>
    /// ���������� ����������� � ������� ���������
    /// </summary>
    public void ShowComment(string message)
    {
        if (isDisplaying)
        {
            // ���� ��� ������������ ���������, ������� �������� ���
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            StartCoroutine(HideCommentRoutine(true, message));
        }
        else
        {
            commentText.text = message;
            currentAnimation = StartCoroutine(ShowCommentRoutine());
        }
    }

    private IEnumerator ShowCommentRoutine()
    {
        isDisplaying = true;

        // �������� ���������
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            float t = elapsedTime / fadeInDuration;
            commentPanel.anchoredPosition = Vector2.Lerp(hiddenPosition, shownPosition, t);
            canvasGroup.alpha = t;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        commentPanel.anchoredPosition = shownPosition;
        canvasGroup.alpha = 1f;

        // ���� ��������� �����
        yield return new WaitForSeconds(displayDuration);

        // �������������� �������
        currentAnimation = StartCoroutine(HideCommentRoutine(false));
    }

    private IEnumerator HideCommentRoutine(bool showNextAfter, string nextMessage = "")
    {
        // �������� ���������
        float elapsedTime = 0f;
        Vector2 startPos = commentPanel.anchoredPosition;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            float t = elapsedTime / fadeOutDuration;
            commentPanel.anchoredPosition = Vector2.Lerp(startPos, hiddenPosition, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        commentPanel.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        isDisplaying = false;

        if (showNextAfter && !string.IsNullOrEmpty(nextMessage))
        {
            commentText.text = nextMessage;
            currentAnimation = StartCoroutine(ShowCommentRoutine());
        }
        else
        {
            currentAnimation = null;
        }
    }

    /// <summary>
    /// �������� ����������� ����������
    /// </summary>
    public void HideImmediately()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        commentPanel.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        isDisplaying = false;
    }
}