using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour
{
    public void MoveToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
