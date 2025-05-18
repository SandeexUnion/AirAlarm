using UnityEngine;

/// <summary>
/// ���������� ����� ���� � ����������� �������� � ��������� ���������
/// </summary>
public class PauseController : MonoBehaviour
{
    private float[] _originalPitches; // ������ ������������ �������� pitch ����� ����������
    private AudioSource[] _audioSources; // ������������ ����� ���������
    private bool _isPaused = false; // ���� ��������� �����

    /// <summary>
    /// ������ ���� �� �����
    /// </summary>
    public void Pause()
    {
        if (_isPaused) return;

        // ��������� �������� �������
        Time.timeScale = 0f;

        // �������� ��� ����� ��������� � �����
        _audioSources = FindObjectsOfType<AudioSource>();
        _originalPitches = new float[_audioSources.Length];

        // ��������� ������ ����� �� ���� ����� ����������
        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (_audioSources[i] == null) continue;

            // ��������� ������������ pitch
            _originalPitches[i] = _audioSources[i].pitch;
            // ������������� pitch � 0 ��� ������� �����
            _audioSources[i].pitch = 0f;
        }

        _isPaused = true;
    }

    /// <summary>
    /// ������������ ����
    /// </summary>
    public void Resume()
    {
        if (!_isPaused) return;

        // ��������������� ���������� ������� �����
        Time.timeScale = 1f;

        // ��������������� ������������ ��������� �����
        for (int i = 0; i < _audioSources.Length; i++)
        {
            // �������� �� ������ ������������ ��������
            if (_audioSources[i] != null)
            {
                _audioSources[i].pitch = _originalPitches[i];
            }
        }

        _isPaused = false;
    }

    /// <summary>
    /// ����������� ��������� �����
    /// </summary>
    public void TogglePause()
    {
        if (_isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}