using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� ��� ����������� ������������/����������� � ����.
/// ��������� ���������� ��������� ��������� � ������������ ������� �� ������� Space.
/// ������������ � �������� ����� � ������������ �����������.
/// </summary>
public class CommentController : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("UI Text ��������� ��� ����������� �����������")]
    [SerializeField]
    private Text commentText;

    [Tooltip("������������ ����������� ����������� (���� �� ����� �������)")]
    [SerializeField]
    private float displayDuration = 2f;

    [Header("Internal References")]
    private NotificationMoveToCentr notificationMoveToCentrController;
    private PauseController pauseController;

    [Header("State Tracking")]
    private bool isDisplaying = false;

    /// <summary>
    /// ����, ����������� ������������ �� � ������ ������ �����������
    /// </summary>
    public bool IsDisplaying => isDisplaying;

    /// <summary>
    /// ������������� ���������� ��� ������
    /// </summary>
    private void Start()
    {
        InitializeDependencies();
        SetupInitialState();
    }

    /// <summary>
    /// ����� � ����������� ����������� ������������
    /// </summary>
    private void InitializeDependencies()
    {
        notificationMoveToCentrController = Object.FindFirstObjectByType<NotificationMoveToCentr>();
        pauseController = Object.FindFirstObjectByType<PauseController>();
    }

    /// <summary>
    /// ��������� ���������� ��������� UI
    /// </summary>
    private void SetupInitialState()
    {
        commentText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ��������� ����� ������������ ������ ����
    /// </summary>
    private void Update()
    {
        HandleUserInput();
    }

    /// <summary>
    /// ��������� ������� Space ��� ������� �����������
    /// </summary>
    private void HandleUserInput()
    {
        if (isDisplaying && Input.GetKeyUp(KeyCode.Space))
        {
            HideComment();
        }
    }

    /// <summary>
    /// ��������� ����� ��� ����������� �����������
    /// </summary>
    /// <param name="comment">����� ����������� ��� �����������</param>
    public void ShowComment(string comment)
    {
        if (!isDisplaying)
        {
            DisplayComment(comment);
        }
    }

    /// <summary>
    /// ���������� ����� ��� ����������� �����������
    /// </summary>
    /// <param name="comment">����� �����������</param>
    private void DisplayComment(string comment)
    {
        isDisplaying = true;
        commentText.text = comment;
        commentText.gameObject.SetActive(true);

        // ��������� �������� ��� ��������������� �������
        StartCoroutine(AutoHideCommentAfterDelay());
    }

    /// <summary>
    /// ������� ����������� � ��������������� ����
    /// </summary>
    private void HideComment()
    {
        pauseController.Resume();
        isDisplaying = false;
        notificationMoveToCentrController.HideNotification();
        commentText.gameObject.SetActive(false);
    }

    /// <summary>
    /// �������� ��� ��������������� ������� ����������� ����� �������� �����
    /// </summary>
    private IEnumerator AutoHideCommentAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (isDisplaying)
        {
            HideComment();
        }
    }
}