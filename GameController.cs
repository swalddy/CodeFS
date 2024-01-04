using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private NPC[] allNPCs;
    [SerializeField] private GameObject stageClearUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;

    private GameState state;
    private bool isSceneLoading = false;

    public enum GameState
    {
        FreeRoam,
        Dialog
    }


    private void Start()
    {
        stageClearUI.SetActive(false);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            CheckAllNPCsInteracted();
        }
    }

    private void CheckAllNPCsInteracted()
    {
        foreach (var npc in allNPCs)
        {
            if (!npc.IsInteracted)
                return;
        }

        ShowStageClearUI();
    }

    private void ShowStageClearUI()
    {
        playerController.DisableMovement();
        stageClearUI.SetActive(true);
        continueButton.onClick.AddListener(() => ContinueGame());
        menuButton.onClick.AddListener(() => ReturnToMenu());
    }

    private void ContinueGame()
    {
        LoadNextLevel();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync(0); 
    }

    private void LoadNextLevel()
    {
        if (isSceneLoading) 
            return;

        isSceneLoading = true;
        int nextSceneIndex = GetNextSceneIndex(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(LoadSceneAsync(nextSceneIndex));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isSceneLoading = false;
    }

    private int GetNextSceneIndex(int currentSceneIndex)
    {
        switch (currentSceneIndex)
        {
            case 1:
                return 3;
            case 3:
                return 4;
            default:
                return 0;
        }
    }
}
