// CommentController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CommentController : MonoBehaviour
{
    [SerializeField] private Text commentText;
    [SerializeField] private float displayDuration = 2f;

    private bool isDisplaying = false;
    public bool IsDisplaying => isDisplaying;

    void Start() => commentText.gameObject.SetActive(false);

    public void ShowComment(string comment)
    {
        if (!isDisplaying)
        {
            StartCoroutine(DisplayComment(comment));
        }
    }

    private IEnumerator DisplayComment(string comment)
    {
        isDisplaying = true;
        commentText.text = comment;
        commentText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        commentText.gameObject.SetActive(false);
        isDisplaying = false;
    }
}