using UnityEngine;

/// <summary>
/// Контроллер паузы игры с управлением временем и звуковыми эффектами
/// </summary>
public class PauseController : MonoBehaviour
{
    private float[] _originalPitches; // Массив оригинальных значений pitch аудио источников
    private AudioSource[] _audioSources; // Кэшированные аудио источники
    private bool _isPaused = false; // Флаг состояния паузы

    /// <summary>
    /// Ставит игру на паузу
    /// </summary>
    public void Pause()
    {
        if (_isPaused) return;

        // Остановка игрового времени
        Time.timeScale = 0f;

        // Получаем все аудио источники в сцене
        _audioSources = FindObjectsOfType<AudioSource>();
        _originalPitches = new float[_audioSources.Length];

        // Применяем эффект паузы ко всем аудио источникам
        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (_audioSources[i] == null) continue;

            // Сохраняем оригинальный pitch
            _originalPitches[i] = _audioSources[i].pitch;
            // Устанавливаем pitch в 0 для эффекта паузы
            _audioSources[i].pitch = 0f;
        }

        _isPaused = true;
    }

    /// <summary>
    /// Возобновляет игру
    /// </summary>
    public void Resume()
    {
        if (!_isPaused) return;

        // Восстанавливаем нормальное игровое время
        Time.timeScale = 1f;

        // Восстанавливаем оригинальные параметры звука
        for (int i = 0; i < _audioSources.Length; i++)
        {
            // Проверка на случай уничтоженных объектов
            if (_audioSources[i] != null)
            {
                _audioSources[i].pitch = _originalPitches[i];
            }
        }

        _isPaused = false;
    }

    /// <summary>
    /// Переключает состояние паузы
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