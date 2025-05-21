using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationMoveToCentr : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform ������/����������� ��� �����������")]
    public RectTransform buttonRT;
    [Tooltip("Canvas, � �������� ����������� ����������� (��� ������� ������)")]
    public Canvas canvas;

    [Header("Animation Settings")]
    [SerializeField, Tooltip("������������ �������� � ��������")]
    private float animationDuration = 0.5f;

    private Vector2 startAnchoredPos;
    private Vector2 centerAnchoredPos;
    private Vector2 hiddenAnchoredPos;

    private PauseController pauseController;
    private Coroutine currentAnimation;

    public static NotificationMoveToCentr Instance { get; private set; }

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

    private void Start()
    {
        canvas = buttonRT.parent.GetComponent<Canvas>();
        pauseController = FindObjectOfType<PauseController>();
        CalculatePositions();
        buttonRT.anchoredPosition = hiddenAnchoredPos; // ����� ��������
    }

    private void CalculatePositions()
    {
        startAnchoredPos = buttonRT.anchoredPosition;
        centerAnchoredPos = new Vector2(0f, buttonRT.anchoredPosition.y);

        // ������������ ������� �� ������� � ������ ������ ������� � ������ �����������
        float screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        float notificationWidth = buttonRT.rect.width;
        hiddenAnchoredPos = new Vector2(-(screenWidth + notificationWidth), buttonRT.anchoredPosition.y);
    }

    public void ShowNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        // ���� ��� ���� ��������, ��������� � � ��������� ��������
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            buttonRT.anchoredPosition = hiddenAnchoredPos;
        }

        currentAnimation = StartCoroutine(MoveAnimation(hiddenAnchoredPos, centerAnchoredPos));
    }

    public void HideNotification()
    {
        if (buttonRT == null)
        {
            Debug.LogError("Button RectTransform is not assigned!", this);
            return;
        }

        // ��������� ������� �������� (���� ����) � ��������� �������
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(MoveAnimation(buttonRT.anchoredPosition, hiddenAnchoredPos));
    }

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

        // ���� ����������� � ������ � ������ ���� �� �����
        if (Vector2.Distance(buttonRT.anchoredPosition, centerAnchoredPos) < 0.1f)
        {
            pauseController?.Pause();
        }
    }
}