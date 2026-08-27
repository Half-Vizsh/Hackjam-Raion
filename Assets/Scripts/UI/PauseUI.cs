using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject  MusicPanel;
    [SerializeField] private GameObject TutorialPanel;
    private bool _isPause;
    public void Start()
    {
        Resume();
    }
    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPause)
            {
                Resume();
            } else {
                Pause();
            }
        }
    }
    #region Public Methods
    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        _isPause = true;
    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        _isPause = false;
    }
    public void MoveToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    public void OpenMusicPanel()
    {
        TutorialPanel.SetActive(false);
        MusicPanel.SetActive(true);    
    }
    public void OpenTutorialPanel()
    {
        MusicPanel.SetActive(false);
        TutorialPanel.SetActive(true);
    }
    #endregion
}
