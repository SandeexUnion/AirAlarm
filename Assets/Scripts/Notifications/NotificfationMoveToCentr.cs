using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotificationMoveToCentr : MonoBehaviour
{
    public RectTransform buttonRT;
    [SerializeField] private float hiddenOffset = 2000f; // Оптимальное значение

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
        // Сразу прячем уведомление
        buttonRT.anchoredPosition = hiddenAnchoredPos;
    }

    private void CalculatePositions()
    {
        // Используем anchoredPosition вместо localPosition
        startAnchoredPos = buttonRT.anchoredPosition;

        // Центральная позиция
        centerAnchoredPos = new Vector2(0, buttonRT.anchoredPosition.y);

        // Позиция за экраном (слева)
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

        // Пауза при достижении центра
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