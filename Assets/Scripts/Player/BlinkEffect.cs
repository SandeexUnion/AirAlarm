using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;
    public bool CloseEyes { get; set; }
    private bool isBlinking = false;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void Blink()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkCoroutine());
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        isBlinking = true;

        yield return FadeToColor(new Color(0, 0, 0, CloseEyes ? 1 : 0));

        if (CloseEyes)
        {
            yield return new WaitForSeconds(0.1f);
            yield return FadeToColor(new Color(0, 0, 0, 0));
        }

        isBlinking = false;
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

        fadeImage.color = targetColor;
    }
}