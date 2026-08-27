using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private GameObject CreditPanel;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject SettingPanel;
    private void Start()
    {
        TitleMenu();
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void CreditMenu()
    {
        CreditPanel.SetActive(true);
        MainPanel.SetActive(false);
        SettingPanel.SetActive(false);
    }
    public void SettingMenu()
    {
        CreditPanel.SetActive(false);
        MainPanel.SetActive(false);
        SettingPanel.SetActive(true);        
    }
    public void TitleMenu()
    {
        CreditPanel.SetActive(false);
        MainPanel.SetActive(true);
        SettingPanel.SetActive(false);        
    }
}
