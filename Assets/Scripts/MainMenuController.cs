using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    // Основные UI элементы
    [Header("Основные панели")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject topicSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingsPanel;

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
    [SerializeField] private Image topicBackgroundImage;

    // Визуальные эффекты
    [Header("Визуальные эффекты")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 0.5f;
    public int a;

    // Текущая выбранная тема
    private string currentSelectedTopic;

    // Доступные темы и уровни
    private readonly Dictionary<string, TopicData> gameTopics = new Dictionary<string, TopicData>()
    {
        {
            "Пожарная безопасность",
            new TopicData(
                new List<string>{"FireLevel1", "FireLevel2", "FireLevel3"},
                "Изучите правила поведения при пожаре в различных ситуациях",
                new Color(0.8f, 0.2f, 0.2f) // Красный
            )
        },
        {
            "Ракетная опасность",
            new TopicData(
                new List<string>{"RocketLevel1", "RocketLevel2", "RocketLevel3"},
                "Действия при ракетном обстреле и угрозе взрыва",
                new Color(0.4f, 0.4f, 0.4f) // Серый
            )
        },
        {
            "Туториал",
            new TopicData(
                new List<string>{ "TutorialLevel1", "TutorialLevel2", "TutorialLevel"},
                "Правила поведения при наводнении",
                new Color(0.2f, 0.4f, 0.8f) // Синий
            )
        }
    };

    private void Start()
    {
        CalculateGridLayout();
        ShowMainMenu();

        // Загрузка сохраненных настроек
        LoadSettings();

        // Настройка обработчиков
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void Update()
    {

    }

    #region Основная навигация

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        topicSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    public void OnSettingsButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Выбор темы и уровней

    public void OnTopicSelected(string topicName)
    {
        if (!gameTopics.ContainsKey(topicName)) return;

        currentSelectedTopic = topicName;
        StartCoroutine(TransitionToLevelSelection());
    }

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

    private void InitializeLevelSelection()
    {
        // Очистка старых кнопок
        foreach (Transform child in levelButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        // Настройка сетки
        var grid = levelButtonsContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = levelButtonsContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(300, 80);
            grid.spacing = new Vector2(20, 20);
            grid.constraintCount = 2; // 2 колонки
            if (Screen.width > Screen.height) // Горизонтальная
            {
                grid.cellSize = new Vector2(250, 80);
                grid.constraintCount = 4;
            }
            else // Вертикальная
            {
                grid.cellSize = new Vector2(300, 100);
                grid.constraintCount = 2;
            }
        }

        // Создание кнопок
        var levels = gameTopics[currentSelectedTopic].LevelScenes;
        for (int i = 0; i < levels.Count; i++)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            levelButton.GetComponentInChildren<Text>().text = $"Уровень {i + 1}";

            // Настройка внешнего вида
            var colors = levelButton.colors;
            colors.normalColor = new Color(0.2f, 0.4f, 0.8f);
            levelButton.colors = colors;

            int levelIndex = i;
            levelButton.onClick.AddListener(() => OnLevelSelected(levelIndex));
        }

        // Принудительное обновление layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelButtonsContainer.GetComponent<RectTransform>());
    }

    void CalculateGridLayout()
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

    private void OnLevelSelected(int levelIndex)
    {
        StartCoroutine(LoadLevelAsync(gameTopics[currentSelectedTopic].LevelScenes[levelIndex]));
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
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

    #endregion

    #region Кнопки "Назад"

    public void OnBackFromTopicSelect()
    {
        topicSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnBackFromLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    public void SetMasterVolume(float volume)
    {
        float volumeValue = masterVolumeSlider.value;

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
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenMode", isFullscreen ? 1 : 0);
    }

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

    #endregion

    #region Вспомогательный класс

    private class TopicData
    {
        public List<string> LevelScenes { get; }
        public string Description { get; }
        public Color ThemeColor { get; }

        public TopicData(List<string> levelScenes, string description, Color themeColor)
        {
            LevelScenes = levelScenes;
            Description = description;
            ThemeColor = themeColor;
        }
    }

    #endregion
}