    using UnityEngine;
    using UnityEngine.UI;

    public class LevelStartUI : MonoBehaviour
    {
        [SerializeField] private GameObject levelStartPanel;
        [SerializeField] private PlayerController playerController;

        private void Start()
        {
            playerController.DisableMovement();

            Button mainButton = levelStartPanel.GetComponentInChildren<Button>();
            mainButton.onClick.AddListener(StartLevel);
        }

        private void StartLevel()
        {
            playerController.EnableMovement();

            levelStartPanel.SetActive(false);
        }
    }
