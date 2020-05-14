using System;
using Assets.Source.UIManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Player;
using UnityEngine;

namespace Assets.Source.PlayerManagement
{
    /// <summary>
    /// Controls players UI and interactions
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager instance;

        public static PlayerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerManager>();
                }
                return instance;
            }
        }

        private Player player;

        private Player enemy;

        public void Update()
        {
            EstimatePlayersStats(player);
            UIManager.Instance.SetPlayerUI(player);
            EstimatePlayersStats(enemy);
            UIManager.Instance.SetEnemyUI(enemy);
        }

        public void EstimatePlayersStats(Player p)
        {
            float mana = Time.deltaTime * (p.ManaPerSecond + p.ManaPerSecondFromEffects);
            p.Mana = Math.Min(p.Mana + mana, p.MaxMana);
            float health = Time.deltaTime * p.HealthPerSecondFromEffect;
            p.Health = Math.Min(p.Health + health, p.MaxHealth);
        }

        /// <summary>
        /// Returns player or enemy matching id
        /// </summary>
        /// <param name="ingameID"></param>
        /// <returns></returns>
        public Player GetPlayerByID(int ingameID)
        {
            if (player.InGameID == ingameID)
                return player;
            if (enemy.InGameID == ingameID)
                return enemy;
            return null;
        }
        
        /// <summary>
        /// Creates players info
        /// </summary>
        public void Initialize()
        {
            player = new Player();
            enemy = new Player();
        }

        /// <summary>
        /// Shows message depending on which player won
        /// </summary>
        /// <param name="playerID"></param>
        public void ShowPlayerWonOrLost(int playerID)
        {
            if (player.InGameID == playerID)
            {
                UIManager.Instance.ActivateWonUI();
                // You won
                Debug.Log("You won");
            }
            else
            {
                UIManager.Instance.ActivateLostUI();
                // You lost
                Debug.Log("You lost");
            }
        }

        /// <summary>
        /// Sets players UI and local data
        /// </summary>
        /// <param name="data"></param>
        public void SetPlayerState(PlayerData data)
        {
            player.InGameID = data.InGameID;
            player.Name = data.Name;
            player.MaxHealth = data.MaxHealth;
            player.Health = data.Health;
            player.MaxMana = data.MaxMana;
            player.Mana = data.Mana;
            player.ManaPerSecond = data.ManaPerSecond;

            UIManager.Instance.SetPlayerUI(player);
        }

        /// <summary>
        /// Sets enemy UI and local data
        /// </summary>
        /// <param name="data"></param>
        public void SetEnemyState(PlayerData data)
        {
            enemy.Name = data.Name;
            enemy.MaxHealth = data.MaxHealth;
            enemy.Health = data.Health;
            enemy.MaxMana = data.MaxMana;
            enemy.Mana = data.Mana;
            enemy.ManaPerSecond = data.ManaPerSecond;

            UIManager.Instance.SetEnemyUI(enemy);
        }

        public void ActivateShieldEffect(EffectData data)
        {
            if (player.InGameID == (int)data.Data["Target"])
            {
                UIManager.Instance.ActivatePlayerShieldEffect((bool)data.Data["Created"]);
            }
            else
            {
                UIManager.Instance.ActivateEnemyShieldEffect((bool)data.Data["Created"]);
            }
        }

        /// <summary>
        /// Animates health loss or gain
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AnimateHealthEffect(EffectData data)
        {
            if (player.InGameID == (int)data.Data["Target"])
            {
                UIManager.Instance.AnimatePlayerHealthUI(player, (float)data.Data["Value"]);
            }
            else
            {
                UIManager.Instance.AnimateEnemyHealthUI(enemy, (float)data.Data["Value"]);
            }
        }

        /// <summary>
        /// Animates mana loss or gain
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AnimateManaEffect(EffectData data)
        {
            if (player.InGameID == (int)data.Data["Target"])
            {
                UIManager.Instance.AnimatePlayerManaUI(player, (float)data.Data["Value"]);
            }
            else
            {
                UIManager.Instance.AnimateEnemyManaUI(enemy, (float)data.Data["Value"]);
            }
        }
    }
}
