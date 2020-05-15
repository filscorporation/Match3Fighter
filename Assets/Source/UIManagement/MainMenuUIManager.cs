using Assets.Source.GameManagement;
using Assets.Source.PlayerManagement;
using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Controls UI in main menu
    /// </summary>
    public class MainMenuUIManager : MonoBehaviour
    {
        private static MainMenuUIManager instance;

        public static MainMenuUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MainMenuUIManager>();
                }
                return instance;
            }
        }

        public GameObject PlayButton;
        public GameObject DebugButton;
        public GameObject PlayerMenu;

        private bool isPlayerMenuActive = false;

        public void PlayerButtonClick()
        {
            isPlayerMenuActive = !isPlayerMenuActive;

            if (isPlayerMenuActive)
            {
                PlayerStatsManager.Instance.DrawPlayerStats();
            }

            PlayerMenu.SetActive(isPlayerMenuActive);

            PlayButton.SetActive(!isPlayerMenuActive);
            DebugButton.SetActive(!isPlayerMenuActive);
        }

        public void StartGameButtonClick()
        {
            GameManager.Instance.QueueGame();
        }

        public void DebugButtonClick()
        {
            GameManager.Instance.QueueDebugGame();
        }
    }
}
