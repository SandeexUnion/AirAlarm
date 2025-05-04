using UnityEngine;

public class PauseController : MonoBehaviour
{
    private float[] originalPitches; // Сохраняем оригинальные pitch
    private AudioSource[] audioSources;
    private bool isPaused = false;

    public void Pause()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        audioSources = FindObjectsOfType<AudioSource>();
        originalPitches = new float[audioSources.Length];

        for (int i = 0; i < audioSources.Length; i++)
        {
            originalPitches[i] = audioSources[i].pitch;
            audioSources[i].pitch = 0f; // Эффект паузы
        }

        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
            {
                audioSources[i].pitch = originalPitches[i]; // Восстанавливаем pitch
            }
        }

        isPaused = false;
    }
}