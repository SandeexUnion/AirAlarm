using UnityEngine;
using UnityEngine.UI; // ��� ������������� UI
using TMPro; // ���� ����������� TextMeshPro
using System.Collections;

public class CommentController : MonoBehaviour
{
    public Text commentText; // ��� �������� ������
    // public TextMeshProUGUI commentText; // ��� TextMeshPro
    public float displayDuration = 2f; // ����� ����������� �����������
    private bool isDisplaying = false; // ���� ��� ������������ ��������� �����������

    public bool IsDisplaying => isDisplaying; // �������� ��� ������� � �����

    void Start()
    {
        commentText.gameObject.SetActive(false); // ������ ����� � ������
    }

    public void ShowComment(string comment)
    {
        if (!isDisplaying) // ���������, ������������ �� ����� � ������ ������
        {
            StartCoroutine(DisplayComment(comment));
        }
    }

    private IEnumerator DisplayComment(string comment)
    {
        isDisplaying = true; // ������������� ���� �����������
        commentText.text = comment; // ���������� ����� �����������
        commentText.gameObject.SetActive(true); // �������� �����
        yield return new WaitForSeconds(displayDuration); // ����� �������� �����
        commentText.gameObject.SetActive(false); // ������ �����
        isDisplaying = false; // ���������� ���� �����������
    }
}


