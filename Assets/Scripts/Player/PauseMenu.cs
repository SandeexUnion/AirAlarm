using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private Moving moving;

    public bool isPauseMenuShowing;

    void Start()
    {
        menuPanel.SetActive(false);
    }

    void Update()
    {

    }

    public void ShowPouseMenu()
    {
        isPauseMenuShowing = true;
        menuPanel.SetActive(true);
        moving.IsCursorLock(false);
        pauseController.Pause();
    }

    public void HidePouseMenu()
    {
        isPauseMenuShowing = false;
        menuPanel.SetActive(false);
        moving.IsCursorLock(true);
        pauseController.Resume();
    }
}