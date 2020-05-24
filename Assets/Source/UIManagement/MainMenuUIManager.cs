using System.Collections;
using Assets.Source.GameManagement;
using Assets.Source.NetworkManagement;
using Assets.Source.PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

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

        public GameObject GameLayout;
        public GameObject MainGameLayout;
        public GameObject PlayerMenu;

        public InputField PlayerNameText;

        public Text CurrencyText;
        private const string currencyTextString = "Gold: ";
        public Text RatingText;
        private const string ratingTextString = "Rating: ";

        public Text ConnectingStateText;

        private bool isPlayerMenuActive = false;

        private void Start()
        {
            StartCoroutine(ActivateMenuLayout());
        }

        private IEnumerator ActivateMenuLayout()
        {
            GameLayout.SetActive(false);

            ConnectingStateText.text = "Connecting";
            yield return new WaitUntil(() => NetworkManager.Instance.IsConnected);
            ConnectingStateText.text = "Connected. Signing in";
            yield return new WaitUntil(() => NetworkManager.Instance.IsLoggedIn);
            ConnectingStateText.text = "Connected. Signed in";

            

            GameLayout.SetActive(true);
        }

        /// <summary>
        /// Sets player name text in UI
        /// </summary>
        /// <param name="playerName"></param>
        public void SetPlayerName(string playerName)
        {
            PlayerNameText.text = playerName;
        }

        /// <summary>
        /// Sets player currency value text in UI
        /// </summary>
        /// <param name="currency"></param>
        public void SetCurrency(int currency)
        {
            CurrencyText.text = currencyTextString + currency;
        }

        /// <summary>
        /// Sets player rating value text in UI
        /// </summary>
        /// <param name="rating"></param>
        public void SetRating(int rating)
        {
            RatingText.text = ratingTextString + rating;
        }

        public void OnPlayersNameChanged()
        {
            PlayerStatsManager.Instance.SetPlayerName(PlayerNameText.text);
        }

        public void PlayerButtonClick()
        {
            isPlayerMenuActive = !isPlayerMenuActive;

            if (isPlayerMenuActive)
            {
                PlayerStatsManager.Instance.DrawPlayerBlocksCollection();
            }

            PlayerMenu.SetActive(isPlayerMenuActive);

            MainGameLayout.SetActive(!isPlayerMenuActive);
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
