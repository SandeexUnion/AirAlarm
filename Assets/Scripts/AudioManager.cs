
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips; // ������ ����������� 
    public AudioSource audioSource;
    private int currentTrackIndex = 0;
    public CommentController commentController;
    public GameObject bed;
    public BlinkEffect blinkEffect;
    public GameObject triggers1;
    public GameObject triggers2;
    private bool hasFinished = false; // ���������� ��� ������������ ������ ������
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextTrack();
    }

    

    void Update()
    {
        Debug.Log(audioClips[currentTrackIndex].name);
        // ���������, ����������� �� ����
        if (!audioSource.isPlaying && !hasFinished)
        {
            hasFinished = true; // ������������� ����, ��� ����� ��� ������
            OnAudioFinished(); // �������� �����
        }
        else if (audioSource.isPlaying)
        {
            if (audioSource.clip.name == "������2.")
            {
                triggers1.gameObject.SetActive(false);
                triggers2.gameObject.SetActive(true);
            }
            hasFinished = false; // ���������� ����, ���� ������ ����� ������
        }
    }

    void OnAudioFinished()
    {
        // ������� � ���������� ����� 
        currentTrackIndex++;
        
        if (audioSource.clip.name == "������")
        {
            blinkEffect.CloseEyes = true;

        }
        else
        {
            commentController.ShowComment("����� ��� ����� ��������.");
            
            bed.tag = "Interact";

        }

    }

    public void PlayNextTrack()
    {
        audioSource.clip = audioClips[currentTrackIndex];
        audioSource.Play();
        Debug.Log($"��������������� ����: {audioSource.clip.name}");
    }
}