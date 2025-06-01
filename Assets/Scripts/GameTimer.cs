using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ������ ����, ������������� ���������� ����� � ��������������� ���
/// � ������� �������� �������������� ���������� � ���������� �����.
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private float totalTime = 30f; // ����� ����� ��� �������
    [SerializeField] private Image pieImage;        // ����������� ��� ��������� ������� (FillMethod = Radial360)

    [Header("�����")]
    [SerializeField] private Color startColor = Color.green; // ��������� ���� ����������
    [SerializeField] private Color endColor = Color.red;    // �������� ���� ����������

    private float currentTime; // ������� ���������� �����

    /// <summary>
    /// ������������� ���������� ��� ������
    /// </summary>
    void Start()
    {
        currentTime = totalTime;

        // ����������� ����������� ��� ��������� �����������
        if (pieImage != null)
        {
            pieImage.type = Image.Type.Filled;
            pieImage.fillMethod = Image.FillMethod.Radial360;
            pieImage.fillOrigin = (int)Image.Origin360.Top;
            pieImage.fillClockwise = false;
        }
    }

    /// <summary>
    /// ���������� ��������� ������� ������ ����
    /// </summary>
    void Update()
    {
        // ��������� ����� � ������ ��������� �������
        currentTime -= Time.deltaTime;

        // ��������� ���������� �������������
        UpdateTimer();

        // �������� ��������� �������
        if (currentTime <= 0)
        {
            currentTime = 0;
            // ������������� ������� ����� ��� ��������� �������
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// ���������� ����������� ������������� �������
    /// </summary>
    void UpdateTimer()
    {
        // ������� ���� ��� ������ �� �����������
        if (pieImage == null) return;

        // ��������� �������� (�� 1 �� 0)
        float progress = currentTime / totalTime;

        // ������������� ���������� ����������
        pieImage.fillAmount = progress;

        // ������������� ���� �� ���������� � ���������
        pieImage.color = Color.Lerp(endColor, startColor, progress);
    }
}