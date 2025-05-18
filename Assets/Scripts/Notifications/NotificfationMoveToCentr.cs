using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ����� ��� ���������� ��������� ����������� ����������� � ����� ������ � �������
/// </summary>
public class NotificationMoveToCentr : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform ������/����������� ��� �����������")]
    public RectTransform buttonRT;

    [Header("Animation Settings")]
    [SerializeField, Tooltip("�������� ��� ������� ����������� �� ��������� ������")]
    private float hiddenOffset = 2000f;

    // ������� ��� ��������
    private Vector2 startAnchoredPos;
    private Vector2 centerAnchoredPos;
    private Vector2 hiddenAnchoredPos;

    // ������ �� ���������� �����
    private PauseController pauseController;

    // ������ ��� ��������
    private float time;

    // Singleton instance
    public static NotificationMoveToCentr Instance { get; private set; }

    /// <summary>
    /// ������������� singleton
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
    /// ��������� ������������� ������� � ������� �����������
    /// </summary>
    private void Start()
    {
        pauseController = Object.FindFirstObjectByType<PauseController>();
        CalculatePositions();

        // ����� ������ ����������� �� ��������� ������
        buttonRT.anchoredPosition = hiddenAnchoredPos;
    }

    /// <summary>
    /// ��������� ���������, ����������� � ������� ������� �����������
    /// </summary>
    private void CalculatePositions()
    {
        // ��������� ��������� ������� �� ���������
        startAnchoredPos = buttonRT.anchoredPosition;

        // ����������� ������� (�� X = 0, Y �������� ��� � ��������� �������)
        centerAnchoredPos = new Vector2(0f, buttonRT.anchoredPosition.y);

        // ������� �� ������� (�����)
        hiddenAnchoredPos = new Vector2(-hiddenOffset, buttonRT.anchoredPosition.y);
    }

    /// <summary>
    /// ��������� �������� ������ ����������� (�� �������� ��������� � �����)
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
    /// ��������� �������� ������� ����������� (�� ������ � ������� ���������)
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
    /// �������� ��� �������� ����������� ����������� ����� ���������
    /// </summary>
    /// <param name="from">��������� �������</param>
    /// <param name="to">�������� �������</param>
    private IEnumerator Moving(Vector2 from, Vector2 to)
    {
        time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;
            buttonRT.anchoredPosition = Vector2.Lerp(from, to, time);
            yield return null;
        }

        // ����������� ������ ��������� � �������� �������
        buttonRT.anchoredPosition = to;

        // ���� ����������� �������� ������ - ������ ���� �� �����
        if (Vector2.Distance(buttonRT.anchoredPosition, centerAnchoredPos) < 3f)
        {
            pauseController?.Pause();
        }
    }

    /// <summary>
    /// ������������� ������� ����� ����� �������� ������� �����������
    /// </summary>
    public void RestartScene()
    {
        StartCoroutine(HideAndRestart());
    }

    /// <summary>
    /// �������� ��� ������� ����������� � ����������� ������������� �����
    /// </summary>
    private IEnumerator HideAndRestart()
    {
        yield return StartCoroutine(Moving(centerAnchoredPos, hiddenAnchoredPos));

        // ������������� ������� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}