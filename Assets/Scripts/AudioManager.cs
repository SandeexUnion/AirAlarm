// AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private CommentController commentController;
    [SerializeField] private BlinkEffect blinkEffect;
    [SerializeField] private GameObject triggers1;
    [SerializeField] private GameObject triggers2;
    

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private bool hasFinished = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying && !hasFinished)
        {
            hasFinished = true;
            OnAudioFinished();
        }
        else if (audioSource.isPlaying)
        {
            if (audioSource.clip.name == "������2.")
            {
                triggers1.SetActive(false);
                triggers2.SetActive(true);
            }
            hasFinished = false;
        }
    }

    void OnAudioFinished()
    {
        currentTrackIndex++;

        if (audioSource.clip.name == "������")
        {
            blinkEffect.CloseEyes = true;
        }
        else
        {
            commentController.ShowComment("����� ��� ����� ��������.");
            
        }
    }

    public void PlayNextTrack()
    {
        audioSource.clip = audioClips[currentTrackIndex];
        audioSource.Play();
        Debug.Log($"��������������� ����: {audioSource.clip.name}");
    }
}