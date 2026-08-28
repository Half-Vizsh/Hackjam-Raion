using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject  MusicPanel;
    [SerializeField] private GameObject TutorialPanel;
    private void Start()
    {
        Resume();
    }
    void Update()
    {
        if (PauseController.instance.IsPause)
        {
            Pause();
        } else
        {
            Resume();
        }
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
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
}
