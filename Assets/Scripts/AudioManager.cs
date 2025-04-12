
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips; // Массив аудиотреков 
    public AudioSource audioSource;
    private int currentTrackIndex = 0;
    public CommentController commentController;
    public GameObject bed;
    public BlinkEffect blinkEffect;
    public GameObject triggers1;
    public GameObject triggers2;
    private bool hasFinished = false; // Переменная для отслеживания вызова метода
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextTrack();
    }

    

    void Update()
    {
        Debug.Log(audioClips[currentTrackIndex].name);
        // Проверяем, закончилась ли игра
        if (!audioSource.isPlaying && !hasFinished)
        {
            hasFinished = true; // Устанавливаем флаг, что метод уже вызван
            OnAudioFinished(); // Вызываем метод
        }
        else if (audioSource.isPlaying)
        {
            if (audioSource.clip.name == "Сирена2.")
            {
                triggers1.gameObject.SetActive(false);
                triggers2.gameObject.SetActive(true);
            }
            hasFinished = false; // Сбрасываем флаг, если музыка снова играет
        }
    }

    void OnAudioFinished()
    {
        // Переход к следующему треку 
        currentTrackIndex++;
        
        if (audioSource.clip.name == "смерть")
        {
            blinkEffect.CloseEyes = true;

        }
        else
        {
            commentController.ShowComment("Думаю уже можно выходить.");
            
            bed.tag = "Interact";

        }

    }

    public void PlayNextTrack()
    {
        audioSource.clip = audioClips[currentTrackIndex];
        audioSource.Play();
        Debug.Log($"Воспроизводится трек: {audioSource.clip.name}");
    }
}