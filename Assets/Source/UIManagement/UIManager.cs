using Assets.Source.GameManagement;
using Assets.Source.PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Controlls UI elements
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>();
                }
                return instance;
            }
        }

        public Text PlayerNameText;
        public Text PlayerHPText;
        public Text PlayerMPText;
        public Text EnemyNameText;
        public Text EnemyHPText;
        public Text EnemyMPText;

        public void StartGameButtonClick()
        {
            GameManager.Instance.QueueGame();
        }

        public void BackGameButtonClick()
        {
            GameManager.Instance.FinishGame();
        }

        public void SetPlayerUI(Player player)
        {
            PlayerNameText.text = player.Name;
            PlayerHPText.text = $"{player.Health}hp";
            PlayerMPText.text = $"{player.Mana}mp";
        }

        public void SetEnemyUI(Player player)
        {
            EnemyNameText.text = player.Name;
            EnemyHPText.text = $"{player.Health}hp";
            EnemyMPText.text = $"{player.Mana}mp";
        }
    }
}
