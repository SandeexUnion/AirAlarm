using UnityEngine;

/// <summary>
/// Контроллер меню паузы, управляющий отображением/скрытием панели паузы
/// и взаимодействующий с системами управления паузой и движением.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Основные компоненты")]
    [SerializeField] private GameObject menuPanel;          // Панель меню паузы
    [SerializeField] private PauseController pauseController; // Контроллер паузы игры
    [SerializeField] private Moving moving;                // Контроллер движения игрока

    [Header("Состояние меню")]
    [Tooltip("Флаг, указывающий отображается ли в данный момент меню паузы")]
    public bool isPauseMenuShowing;                        // Текущее состояние видимости меню

    /// <summary>
    /// Инициализация при старте
    /// </summary>
    private void Start()
    {
        // Скрываем меню паузы при старте
        menuPanel.SetActive(false);
        isPauseMenuShowing = false;
    }

    /// <summary>
    /// Обновление каждый кадр (оставлено для возможного расширения функционала)
    /// </summary>
    private void Update()
    {
        // Можно добавить логику проверки ввода для открытия/закрытия паузы
    }

    #region Управление видимостью меню паузы

    /// <summary>
    /// Показывает меню паузы и ставит игру на паузу
    /// </summary>
    public void ShowPauseMenu()
    {
        isPauseMenuShowing = true;
        menuPanel.SetActive(true);

        // Разблокируем курсор при паузе
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseController.Pause();
    }

    /// <summary>
    /// Скрывает меню паузы и возобновляет игру
    /// </summary>
    public void HidePauseMenu()
    {
        isPauseMenuShowing = false;
        menuPanel.SetActive(false);

        // Блокируем курсор при возврате в игру
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseController.Resume();
    }

    #endregion

    #region Обработчики кнопок меню

    /// <summary>
    /// Обработчик кнопки "Продолжить"
    /// </summary>
    public void OnContinueButtonClicked()
    {
        HidePauseMenu();
    }

    /// <summary>
    /// Обработчик кнопки "В главное меню"
    /// </summary>
    public void OnMainMenuButtonClicked()
    {
        // Здесь должна быть реализация загрузки главного меню
        Debug.Log("Loading main menu...");
    }

    /// <summary>
    /// Обработчик кнопки "Выход"
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}