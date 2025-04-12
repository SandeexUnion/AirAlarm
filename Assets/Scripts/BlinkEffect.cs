using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Playables;

public class BlinkEffect : MonoBehaviour
{
    public Image fadeImage; // Ссылка на ваш Image
    public float fadeDuration = 0.5f; // Длительность затухания
    public bool CloseEyes;
    

    void Start()
    {
        // Начальное состояние
        fadeImage.color = new Color(0, 0, 0, 0); // Прозрачный
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
        // Затухание к черному
        else
        {
            yield return FadeToColor(new Color(0, 0, 0, 0));
            
        }
        // Затухание обратно к прозрачному
       
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

        fadeImage.color = targetColor; // Убедитесь, что цвет установлен в конечное значение
    }
    void Update()
    {
        
        
            GetComponent<BlinkEffect>().Blink();
        
    }
}
