using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    public string message; // ���������, ������� ����� �������� ��� ���������������
    public CommentController commentController; // ������ �� CommentController

    void Start()
    {
        // ����� ������ CommentController � �����
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ���������, ��� ������� ����������� �������
        {
            commentController.ShowComment(message); // ���������� ���������
            Destroy(gameObject);
        }
    }
}
