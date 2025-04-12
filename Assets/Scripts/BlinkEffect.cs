using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Playables;

public class BlinkEffect : MonoBehaviour
{
    public Image fadeImage; // ������ �� ��� Image
    public float fadeDuration = 0.5f; // ������������ ���������
    public bool CloseEyes;
    

    void Start()
    {
        // ��������� ���������
        fadeImage.color = new Color(0, 0, 0, 0); // ����������
    }

    public void Blink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        if (CloseEyes)
        {
            yield return FadeToColor(new Color(0, 0, 0, 1));

        }
        // ��������� � �������
        else
        {
            yield return FadeToColor(new Color(0, 0, 0, 0));
            
        }
        // ��������� ������� � �����������
       
    }

    private IEnumerator FadeToColor(Color targetColor)
    {
        Color startColor = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = targetColor; // ���������, ��� ���� ���������� � �������� ��������
    }
    void Update()
    {
        
        
            GetComponent<BlinkEffect>().Blink();
        
    }
}
