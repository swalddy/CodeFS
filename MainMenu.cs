using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadSceneAsync(2); 
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
