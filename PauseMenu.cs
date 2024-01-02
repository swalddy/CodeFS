using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public PlayerController playerController;

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
        playerController.canMove = false;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        playerController.canMove = true;
    }
}
