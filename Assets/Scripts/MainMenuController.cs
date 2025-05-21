using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// ���������� �������� ����, ����������� ���������� ����� ��������,
/// ����������� � ��������� ������� � ���������� ������������ ��������.
/// </summary>
public class MenuController : MonoBehaviour
{
    // �������� UI ��������
    [Header("�������� ������")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject topicSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    // �������� ��������
    [Header("���������")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer audioMixer;

    // �������� ������ ������
    [Header("����� ������")]
    [SerializeField] private Text topicTitleText;
    [SerializeField] private Transform levelButtonsContainer;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Text levelDescriptionText;

    // ������� ������������ ��������
    [Header("������������ �������")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image loadingScreenImage;
    [SerializeField] private Sprite[] fireThemeSprites;
    [SerializeField] private Sprite[] rocketThemeSprites;
    [SerializeField] private Sprite[] tutorialThemeSprites;
    [SerializeField] private Sprite defaultLoadingSprite;

    // ���������� ������� � ��������
    [Header("���������� �������")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 0.5f;

    // ������� ��������� ���� � �������
    private string currentSelectedTopic;
    private int currentSelectedLevelIndex = -1;

    // ��������� ���� � ������
    private readonly Dictionary<string, TopicData> gameTopics = new Dictionary<string, TopicData>()
    {
        {
            "�������� ������������",
            new TopicData(
                new List<string>{"FireLevel1", "FireLevel2", "FireLevel3"},
                "������� ������� ��������� ��� ������ � ��������� ���������",
                new Color(0.8f, 0.2f, 0.2f), // �������
                0 // ������ ���� ��� ��������
            )
        },
        {
            "�������� ���������",
            new TopicData(
                new List<string>{"RocketLevel1", "RocketLevel2", "RocketLevel3"},
                "�������� ��� �������� �������� � ������ ������",
                new Color(0.4f, 0.4f, 0.4f), // �����
                1 // ������ ���� ��� ��������
            )
        },
        {
            "��������",
            new TopicData(
                new List<string>{ "TutorialLevel1", "TutorialLevel2", "TutorialLevel"},
                "��������",
                new Color(0.2f, 0.4f, 0.8f), // �����
                2 // ������ ���� ��� ��������
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
    /// ���������� ������ ���� (�������� �� ������������)
    /// </summary>
    private void Update()
    {
        // ����� �������� ������ ���������� ��� �������������
    }

    #region �������� ��������� �� ����

    /// <summary>
    /// ���������� ������� ���� � �������� ������ ������
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
    /// ���������� ������� ������ "������"
    /// </summary>
    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    /// <summary>
    /// ���������� ������� ������ "���������"
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// ���������� ������� ������ "�����"
    /// </summary>
    public void OnExitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region ����� ���� � �������

    /// <summary>
    /// ���������� ������ ����
    /// </summary>
    /// <param name="topicName">�������� ��������� ����</param>
    public void OnTopicSelected(string topicName)
    {
        if (!gameTopics.ContainsKey(topicName)) return;

        currentSelectedTopic = topicName;
        TopicData selectedTopic = gameTopics[topicName];
        topicTitleText.text = topicName;
        levelDescriptionText.text = selectedTopic.Description;

        // ��������� ������� ����������� � ������������ � �����
        UpdateBackgroundForTopic(selectedTopic.ThemeSpriteIndex);

        StartCoroutine(TransitionToLevelSelection());
    }

    private void UpdateBackgroundForTopic(int themeIndex)
    {
        if (backgroundImage == null) return;

        Sprite[] themeSprites = GetSpritesForTheme(themeIndex);
        if (themeSprites != null && themeSprites.Length > 0)
        {
            backgroundImage.sprite = themeSprites[0]; // ������ ������ - ����� ��� ����
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
    /// �������� ��� �������� �������� � ������ ������
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
    /// ������������� ������ ������ �������
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
    /// ����������� ��������� GridLayout � ����������� �� ���������� ������
    /// </summary>
    private void ConfigureGridLayout()
    {
        var grid = levelButtonsContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = levelButtonsContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(300, 80);
            grid.spacing = new Vector2(20, 20);
            grid.constraintCount = 2; // 2 ������� �� ���������
        }

        // ��������� ��� ���������� ������
        if (Screen.width > Screen.height) // �������������� ����������
        {
            grid.cellSize = new Vector2(250, 80);
            grid.constraintCount = 4;
        }
        else // ������������ ����������
        {
            grid.cellSize = new Vector2(300, 100);
            grid.constraintCount = 2;
        }
    }

    /// <summary>
    /// ������� ������ ��� ������� ������ � ��������� ����
    /// </summary>
    private void CreateLevelButtons()
    {
        TopicData currentTopic = gameTopics[currentSelectedTopic];
        var levels = currentTopic.LevelScenes;
        Sprite[] themeSprites = GetSpritesForTheme(currentTopic.ThemeSpriteIndex);

        for (int i = 0; i < levels.Count; i++)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            levelButton.GetComponentInChildren<Text>().text = $"{currentSelectedTopic} - ������� {i + 1}";

            // ��������� ������ ������ ���� ���� �������
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
    /// ��������� ����������� ������������ ������ � �����
    /// </summary>
    private void CalculateGridLayout()
    {
        int levelsCount = gameTopics[currentSelectedTopic].LevelScenes.Count;
        var grid = levelButtonsContainer.GetComponent<GridLayoutGroup>();

        if (levelsCount <= 3)
        {
            grid.constraintCount = levelsCount; // 1-3 ������ � ���
        }
        else
        {
            grid.constraintCount = 3; // �������� 3 � ���
        }
    }

    /// <summary>
    /// ���������� ������ ����������� ������
    /// </summary>
    /// <param name="levelIndex">������ ���������� ������</param>
    private void OnLevelSelected(int levelIndex)
    {
        currentSelectedLevelIndex = levelIndex;
        StartCoroutine(LoadLevelAsync(gameTopics[currentSelectedTopic].LevelScenes[levelIndex]));
    }

    /// <summary>
    /// �������� ��� ����������� �������� ������
    /// </summary>
    /// <param name="sceneName">��� ����� ��� ��������</param>
    private IEnumerator LoadLevelAsync(string sceneName)
    {
        // ��������� ������� �������� � ������������ � ����� � �������
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
            // ���������� ������ ������ (������ + 1, ��� ��� 0 - ����� ��� ����)
            loadingScreenImage.sprite = themeSprites[currentSelectedLevelIndex + 1];
        }
        else
        {
            loadingScreenImage.sprite = defaultLoadingSprite;
        }
    }

    #endregion

    #region ��������� "�����"

    /// <summary>
    /// ���������� ������ "�����" �� ������ ���� ��� ��������
    /// </summary>
    public void OnBackFromTopicSelect()
    {
        topicSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// ���������� ������ "�����" �� ������ ������
    /// </summary>
    public void OnBackFromLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        topicSelectPanel.SetActive(true);
    }

    #endregion

    #region ���������� �����������

    /// <summary>
    /// ������������� ��������� ��������� �������
    /// </summary>
    /// <param name="volume">������� ��������� (0-1)</param>
    public void SetMasterVolume(float volume)
{
    // ���� ����� ������ �� ��������, ���������� ��� ��������
    // ����� ���������� ���������� volume
    float volumeValue = (masterVolumeSlider != null && masterVolumeSlider.gameObject.activeInHierarchy) 
        ? masterVolumeSlider.value 
        : volume;

    // �������� �� 0 ����� ����������� ���������
    if (volumeValue > 0)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volumeValue) * 20);
    }
    else
    {
        audioMixer.SetFloat("MasterVolume", -80); // ��������� ������������ �������� ���������
    }

    PlayerPrefs.SetFloat("MasterVolume", volumeValue);
    
    // ��������� �������, ���� �� �������
    if (masterVolumeSlider != null && masterVolumeSlider.gameObject.activeInHierarchy)
    {
        masterVolumeSlider.value = volumeValue;
    }
}

    /// <summary>
    /// ������������� ������������� �����
    /// </summary>
    /// <param name="isFullscreen">�������� ������������� �����?</param>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenMode", isFullscreen ? 1 : 0);
    }

    /// <summary>
    /// ��������� ����������� ���������
    /// </summary>
    private void LoadSettings()
    {
        // ��������� (�������� �� ��������� 0.8)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        masterVolumeSlider.value = savedVolume;
        SetMasterVolume(savedVolume);

        // ������������� ����� (�� ��������� true)
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

    #region ��������������� ������

    /// <summary>
    /// ��������������� ����� ��� �������� ������ � ����
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