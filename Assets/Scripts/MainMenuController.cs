using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// Контроллер главного меню, управляющий навигацией между панелями,
/// настройками и загрузкой уровней с поддержкой тематических спрайтов.
/// </summary>
public class MenuController : MonoBehaviour
{
    // Основные UI элементы
    [Header("Основные панели")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject topicSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    // Элементы настроек
    [Header("Настройки")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer audioMixer;

    // Элементы выбора уровня
    [Header("Выбор уровня")]
    [SerializeField] private Text topicTitleText;
    [SerializeField] private Transform levelButtonsContainer;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Text levelDescriptionText;

    // Система тематических спрайтов
    [Header("Тематические спрайты")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image loadingScreenImage;
    [SerializeField] private Sprite[] fireThemeSprites;
    [SerializeField] private Sprite[] rocketThemeSprites;
    [SerializeField] private Sprite[] tutorialThemeSprites;
    [SerializeField] private Sprite defaultLoadingSprite;

    // Визуальные эффекты и анимации
    [Header("Визуальные эффекты")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 0.5f;

    // Текущая выбранная тема и уровень
    private string currentSelectedTopic;
    private int currentSelectedLevelIndex = -1;

    // Доступные темы и уровни
    private readonly Dictionary<string, TopicData> gameTopics = new Dictionary<string, TopicData>()
    {
        {
            "Пожарная безопасность",
            new TopicData(
                new List<string>{"FireLevel1", "FireLevel2", "FireLevel3"},
                "Изучите правила поведения при пожаре в различных ситуациях",
                new Color(0.8f, 0.2f, 0.2f), // Красный
                0 // Индекс темы для спрайтов
            )
        },
        {
            "Ракетная опасность",
            new TopicData(
                new List<string>{"RocketLevel1", "RocketLevel2", "RocketLevel3"},
                "Действия при ракетном обстреле и угрозе взрыва",
                new Color(0.4f, 0.4f, 0.4f), // Серый
                1 // Индекс темы для спрайтов
            )
        },
        {
            "Туториал",
            new TopicData(
                new List<string>{ "TutorialLevel1", "TutorialLevel2", "TutorialLevel"},
                "Обучение",
                new Color(0.2f, 0.4f, 0.8f), // Синий
                2 // Индекс темы для спрайтов
            )
        }
    };

    private void Start()
    {
        CalculateGridLayout();
        ShowMainMenu();
        LoadSettings();

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    /// <summary>
    /// Обновление каждый кадр (временно не используется)
    /// </summary>
    private void Update()
    {
        // Можно добавить логику обновления при необходимости
    }

    #region Основная навигация по меню

    /// <summary>
    /// Показывает главное меню и скрывает другие панели
    /// </summary>
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        topicSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        loadingPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Играть"
    /// </summary>
    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Настройки"
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Выход"
    /// </summary>
    public void OnExitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Выбор темы и уровней

    /// <summary>
    /// Обработчик выбора темы
    /// </summary>
    /// <param name="topicName">Название выбранной темы</param>
    public void OnTopicSelected(string topicName)
    {
        if (!gameTopics.ContainsKey(topicName)) return;

        currentSelectedTopic = topicName;
        TopicData selectedTopic = gameTopics[topicName];
        topicTitleText.text = topicName;
        levelDescriptionText.text = selectedTopic.Description;

        // Обновляем фоновое изображение в соответствии с темой
        UpdateBackgroundForTopic(selectedTopic.ThemeSpriteIndex);

        StartCoroutine(TransitionToLevelSelection());
    }

    private void UpdateBackgroundForTopic(int themeIndex)
    {
        if (backgroundImage == null) return;

        Sprite[] themeSprites = GetSpritesForTheme(themeIndex);
        if (themeSprites != null && themeSprites.Length > 0)
        {
            backgroundImage.sprite = themeSprites[0]; // Первый спрайт - общий для темы
        }
    }

    private Sprite[] GetSpritesForTheme(int themeIndex)
    {
        switch (themeIndex)
        {
            case 0: return fireThemeSprites;
            case 1: return rocketThemeSprites;
            case 2: return tutorialThemeSprites;
            default: return null;
        }
    }

    /// <summary>
    /// Корутина для плавного перехода к выбору уровня
    /// </summary>
    private IEnumerator TransitionToLevelSelection()
    {
        topicSelectPanel.SetActive(false);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(transitionDuration);
        }

        InitializeLevelSelection();
        levelSelectPanel.SetActive(true);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("FadeIn");
        }
    }

    /// <summary>
    /// Инициализация панели выбора уровней
    /// </summary>
    private void InitializeLevelSelection()
    {
        foreach (Transform child in levelButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        ConfigureGridLayout();
        CreateLevelButtons();
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelButtonsContainer.GetComponent<RectTransform>());
    }


    /// <summary>
    /// Настраивает параметры GridLayout в зависимости от ориентации экрана
    /// </summary>
    private void ConfigureGridLayout()
    {
        var grid = levelButtonsContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = levelButtonsContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(300, 80);
            grid.spacing = new Vector2(20, 20);
            grid.constraintCount = 2; // 2 колонки по умолчанию
        }

        // Адаптация под ориентацию экрана
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            grid.cellSize = new Vector2(250, 80);
            grid.constraintCount = 4;
        }
        else // Вертикальная ориентация
        {
            grid.cellSize = new Vector2(300, 100);
            grid.constraintCount = 2;
        }
    }

    /// <summary>
    /// Создает кнопки для каждого уровня в выбранной теме
    /// </summary>
    private void CreateLevelButtons()
    {
        TopicData currentTopic = gameTopics[currentSelectedTopic];
        var levels = currentTopic.LevelScenes;
        Sprite[] themeSprites = GetSpritesForTheme(currentTopic.ThemeSpriteIndex);

        for (int i = 0; i < levels.Count; i++)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            levelButton.GetComponentInChildren<Text>().text = $"{currentSelectedTopic} - Уровень {i + 1}";

            // Установка иконки уровня если есть спрайты
            if (themeSprites != null && i + 1 < themeSprites.Length)
            {
                levelButton.GetComponent<Image>().sprite = themeSprites[i + 1];
            }

            var colors = levelButton.colors;
            colors.normalColor = currentTopic.ThemeColor;
            levelButton.colors = colors;

            int levelIndex = i;
            levelButton.onClick.AddListener(() => OnLevelSelected(levelIndex));
        }
    }

    /// <summary>
    /// Вычисляет оптимальное расположение кнопок в сетке
    /// </summary>
    private void CalculateGridLayout()
    {
        int levelsCount = gameTopics[currentSelectedTopic].LevelScenes.Count;
        var grid = levelButtonsContainer.GetComponent<GridLayoutGroup>();

        if (levelsCount <= 3)
        {
            grid.constraintCount = levelsCount; // 1-3 кнопки в ряд
        }
        else
        {
            grid.constraintCount = 3; // Максимум 3 в ряд
        }
    }

    /// <summary>
    /// Обработчик выбора конкретного уровня
    /// </summary>
    /// <param name="levelIndex">Индекс выбранного уровня</param>
    private void OnLevelSelected(int levelIndex)
    {
        currentSelectedLevelIndex = levelIndex;
        StartCoroutine(LoadLevelAsync(gameTopics[currentSelectedTopic].LevelScenes[levelIndex]));
    }

    /// <summary>
    /// Корутина для асинхронной загрузки уровня
    /// </summary>
    /// <param name="sceneName">Имя сцены для загрузки</param>
    private IEnumerator LoadLevelAsync(string sceneName)
    {
        // Установка спрайта загрузки в соответствии с темой и уровнем
        UpdateLoadingScreenImage();

        loadingPanel.SetActive(true);
        levelSelectPanel.SetActive(false);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(transitionDuration);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void UpdateLoadingScreenImage()
    {
        if (loadingScreenImage == null) return;

        TopicData currentTopic = gameTopics[currentSelectedTopic];
        Sprite[] themeSprites = GetSpritesForTheme(currentTopic.ThemeSpriteIndex);

        if (themeSprites != null && currentSelectedLevelIndex >= 0 &&
            currentSelectedLevelIndex < themeSprites.Length - 1)
        {
            // Используем спрайт уровня (индекс + 1, так как 0 - общий для темы)
            loadingScreenImage.sprite = themeSprites[currentSelectedLevelIndex + 1];
        }
        else
        {
            loadingScreenImage.sprite = defaultLoadingSprite;
        }
    }

    #endregion

    #region Навигация "Назад"

    /// <summary>
    /// Обработчик кнопки "Назад" из выбора темы или настроек
    /// </summary>
    public void OnBackFromTopicSelect()
    {
        topicSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Обработчик кнопки "Назад" из выбора уровня
    /// </summary>
    public void OnBackFromLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    #endregion

    #region Управление настройками

    /// <summary>
    /// Устанавливает громкость основного микшера
    /// </summary>
    /// <param name="volume">Уровень громкости (0-1)</param>
    public void SetMasterVolume(float volume)
{
    // Если метод вызван из слайдера, используем его значение
    // Иначе используем переданный volume
    float volumeValue = (masterVolumeSlider != null && masterVolumeSlider.gameObject.activeInHierarchy) 
        ? masterVolumeSlider.value 
        : volume;

    // Проверка на 0 перед вычислением логарифма
    if (volumeValue > 0)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volumeValue) * 20);
    }
    else
    {
        audioMixer.SetFloat("MasterVolume", -80); // Установка минимального значения громкости
    }

    PlayerPrefs.SetFloat("MasterVolume", volumeValue);
    
    // Обновляем слайдер, если он активен
    if (masterVolumeSlider != null && masterVolumeSlider.gameObject.activeInHierarchy)
    {
        masterVolumeSlider.value = volumeValue;
    }
}

    /// <summary>
    /// Устанавливает полноэкранный режим
    /// </summary>
    /// <param name="isFullscreen">Включить полноэкранный режим?</param>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenMode", isFullscreen ? 1 : 0);
    }

    /// <summary>
    /// Загружает сохраненные настройки
    /// </summary>
    private void LoadSettings()
    {
        // Громкость (значение по умолчанию 0.8)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        masterVolumeSlider.value = savedVolume;
        SetMasterVolume(savedVolume);

        // Полноэкранный режим (по умолчанию true)
        bool fullscreen = PlayerPrefs.GetInt("FullscreenMode", 1) == 1;
        fullscreenToggle.isOn = fullscreen;
        SetFullscreen(fullscreen);
    }

    public void OnCreditsButtonClicked()
    {

       topicSelectPanel.SetActive(false);
       creditsPanel.SetActive(true);
    
    }

    #endregion

    #region Вспомогательные классы

    /// <summary>
    /// Вспомогательный класс для хранения данных о теме
    /// </summary>
    private class TopicData
    {
        public List<string> LevelScenes { get; }
        public string Description { get; }
        public Color ThemeColor { get; }
        public int ThemeSpriteIndex { get; }

        public TopicData(List<string> levelScenes, string description,
                       Color themeColor, int themeSpriteIndex)
        {
            LevelScenes = levelScenes;
            Description = description;
            ThemeColor = themeColor;
            ThemeSpriteIndex = themeSpriteIndex;
        }
    }

    #endregion
}