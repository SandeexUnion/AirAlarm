using UnityEngine;

/// <summary>
/// Менеджер аудио, управляющий воспроизведением звуковых дорожек и связанными событиями
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Аудио настройки")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    [Header("Связанные компоненты")]
    [SerializeField] private CommentController commentController;
    [SerializeField] private BlinkEffect blinkEffect;

    [Header("Триггеры уровня")]
    [SerializeField] private GameObject triggersPhase1;
    [SerializeField] private GameObject triggersPhase2;

    private int currentTrackIndex = 0;
    private bool hasFinished = false;
    private const string DeathClipName = "смерть";
    private const string SirenClipName = "Сирена2.";

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PlayNextTrack();
    }

    private void Update()
    {
        HandleAudioPlayback();
    }

    /// <summary>
    /// Обрабатывает состояние воспроизведения аудио
    /// </summary>
    private void HandleAudioPlayback()
    {
        if (!audioSource.isPlaying && !hasFinished)
        {
            hasFinished = true;
            OnAudioFinished();
        }
        else if (audioSource.isPlaying)
        {
            HandleSpecialTracks();
            hasFinished = false;
        }
    }

    /// <summary>
    /// Обрабатывает специальные треки с особым поведением
    /// </summary>
    private void HandleSpecialTracks()
    {
        if (audioSource.clip.name == SirenClipName)
        {
            triggersPhase1.SetActive(false);
            triggersPhase2.SetActive(true);
        }
    }

    /// <summary>
    /// Вызывается при завершении воспроизведения трека
    /// </summary>
    private void OnAudioFinished()
    {
        currentTrackIndex++;

        
        
        commentController?.ShowComment("Думаю уже можно выходить.");
        
    }

    /// <summary>
    /// Воспроизводит следующий трек в плейлисте
    /// </summary>
    public void PlayNextTrack()
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("Нет аудио клипов для воспроизведения");
            return;
        }

        if (currentTrackIndex >= audioClips.Length)
        {
            Debug.Log("Все треки воспроизведены");
            return;
        }

        audioSource.clip = audioClips[currentTrackIndex];
        audioSource.Play();
        Debug.Log($"Воспроизводится трек: {audioSource.clip.name}");
    }

    /// <summary>
    /// Перезапускает текущий трек
    /// </summary>
    public void RestartCurrentTrack()
    {
        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }
}