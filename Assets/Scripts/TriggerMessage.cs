// TriggerMessage.cs
using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private CommentController commentController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            commentController.ShowComment(message);
            Destroy(gameObject);
        }
    }
}