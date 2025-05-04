using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotificationMoveToCentr : MonoBehaviour
{
    public RectTransform buttonRT;
    [SerializeField] private float hiddenOffset = 2000f; // ����������� ��������

    private Vector2 startAnchoredPos;
    private Vector2 centerAnchoredPos;
    private Vector2 hiddenAnchoredPos;
    private PauseController pauseController;
    private float time;

    public static NotificationMoveToCentr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        pauseController = Object.FindFirstObjectByType<PauseController>();
        CalculatePositions();
        // ����� ������ �����������
        buttonRT.anchoredPosition = hiddenAnchoredPos;
    }

    private void CalculatePositions()
    {
        // ���������� anchoredPosition ������ localPosition
        startAnchoredPos = buttonRT.anchoredPosition;

        // ����������� �������
        centerAnchoredPos = new Vector2(0, buttonRT.anchoredPosition.y);

        // ������� �� ������� (�����)
        hiddenAnchoredPos = new Vector2(-hiddenOffset, buttonRT.anchoredPosition.y);
    }

    public void ShowNotification()
    {
        StartCoroutine(Moving(hiddenAnchoredPos, centerAnchoredPos));
    }

    public void HideNotification()
    {
        StartCoroutine(Moving(centerAnchoredPos, hiddenAnchoredPos));
    }

    IEnumerator Moving(Vector2 from, Vector2 to)
    {
        time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;
            buttonRT.anchoredPosition = Vector2.Lerp(from, to, time);
            yield return null;
        }

        buttonRT.anchoredPosition = to;

        // ����� ��� ���������� ������
        if (Vector2.Distance(buttonRT.anchoredPosition, centerAnchoredPos) < 3f)
        {
            pauseController?.Pause();
        }
    }

    public void RestartScene()
    {
        StartCoroutine(HideAndRestart());
    }

    IEnumerator HideAndRestart()
    {
        yield return StartCoroutine(Moving(centerAnchoredPos, hiddenAnchoredPos));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}