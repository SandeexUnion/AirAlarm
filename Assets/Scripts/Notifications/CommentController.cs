using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
    private GameObject nextCommentGameObject;

    [Tooltip("������������ ����������� ����������� (���� �� ����� �������)")]
    [SerializeField]
    private float displayDuration = 2f;

    [Header("Internal References")]
    private NotificationMoveToCentr notificationMoveToCentrController;
    private PauseController pauseController;

    [Header("State Tracking")]
    public bool isDisplaying = false;

    [SerializeField] int CurrentSceneNum = 0;
    [SerializeField] GameObject currentTrigger;

    

    /// <summary>
    /// ����, ����������� ������������ �� � ������ ������ �����������
    /// </summary>


    /// <summary>
    /// ������������� ���������� ��� ������
    /// </summary>
    private void Start()
    {
        InitializeDependencies();
        SetupInitialState();
        CurrentSceneNum = SceneManager.GetActiveScene().buildIndex;
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
    public void ShowComment(string comment, GameObject trigger = null)
    {
        if (!isDisplaying)
        {
            currentTrigger = trigger;
            DisplayComment(comment);
            // ������ ���������� �������� ������
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
        pauseController?.Resume();
        isDisplaying = false;
        notificationMoveToCentrController?.HideNotification();
        commentText.gameObject.SetActive(false);

        // ������ ���������� �������� ������

        if (nextCommentGameObject != null)
        {
            nextCommentGameObject.SetActive(true);
        }

        if (currentTrigger != null)
        {
            if (currentTrigger.CompareTag("Completed"))
            {
                SceneManager.LoadScene(0);
                
            }
            else if (currentTrigger.CompareTag("Failure"))
            {
                SceneManager.LoadScene(CurrentSceneNum);
                
            }
            Destroy(currentTrigger);
        }
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