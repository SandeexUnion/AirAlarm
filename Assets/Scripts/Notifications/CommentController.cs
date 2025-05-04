// CommentController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CommentController : MonoBehaviour
{
    [SerializeField] private Text commentText;
    [SerializeField] private float displayDuration = 2f;

    private NotificationMoveToCentr notificationMoveToCentrController;
    private PauseController pauseController;
    private bool isDisplaying = false;

    public bool IsDisplaying => isDisplaying;

    void Start()
    {
        notificationMoveToCentrController = Object.FindFirstObjectByType<NotificationMoveToCentr>();
        commentText.gameObject.SetActive(false);
        pauseController = Object.FindFirstObjectByType<PauseController>();
    }
        
    private void Update()
    {
        if(isDisplaying && Input.GetKeyUp(KeyCode.Space))
        {
            pauseController.Resume();
            isDisplaying = false;
            notificationMoveToCentrController.HideNotification();


        }
    }

    public void ShowComment(string comment)
    {
        if (!isDisplaying)
        {
            DisplayComment(comment);
        }
    }

    private void DisplayComment(string comment)
    {
        isDisplaying = true;
        commentText.text = comment;
        commentText.gameObject.SetActive(true);

        

        
    }
    private void DisableComment(string comment)
    {
        commentText.gameObject.SetActive(false);
        isDisplaying = false;
    }

    
}