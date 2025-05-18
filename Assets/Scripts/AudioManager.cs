using UnityEngine;

/// <summary>
/// �������� �����, ����������� ���������������� �������� ������� � ���������� ���������
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("����� ���������")]
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    [Header("��������� ����������")]
    [SerializeField] private CommentController commentController;
    [SerializeField] private BlinkEffect blinkEffect;

    [Header("�������� ������")]
    [SerializeField] private GameObject triggersPhase1;
    [SerializeField] private GameObject triggersPhase2;

    private int currentTrackIndex = 0;
    private bool hasFinished = false;
    private const string DeathClipName = "������";
    private const string SirenClipName = "������2.";

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
    /// ������������ ��������� ��������������� �����
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
    /// ������������ ����������� ����� � ������ ����������
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
    /// ���������� ��� ���������� ��������������� �����
    /// </summary>
    private void OnAudioFinished()
    {
        currentTrackIndex++;

        
        
        commentController?.ShowComment("����� ��� ����� ��������.");
        
    }

    /// <summary>
    /// ������������� ��������� ���� � ���������
    /// </summary>
    public void PlayNextTrack()
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("��� ����� ������ ��� ���������������");
            return;
        }

        if (currentTrackIndex >= audioClips.Length)
        {
            Debug.Log("��� ����� ��������������");
            return;
        }

        audioSource.clip = audioClips[currentTrackIndex];
        audioSource.Play();
        Debug.Log($"��������������� ����: {audioSource.clip.name}");
    }

    /// <summary>
    /// ������������� ������� ����
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