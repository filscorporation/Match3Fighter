using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Source.UIManagement;
using NetworkShared.Data.Player;
using UnityEngine;

namespace Assets.Source.PlayerManagement
{
    /// <summary>
    /// Controlls players UI and interations
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

        /// <summary>
        /// Sets players UI and local data
        /// </summary>
        /// <param name="data"></param>
        public void SetPlayerState(PlayerData data)
        {
            player = new Player();
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
            enemy = new Player();
            enemy.Name = data.Name;
            enemy.MaxHealth = data.MaxHealth;
            enemy.Health = data.Health;
            enemy.MaxMana = data.MaxMana;
            enemy.Mana = data.Mana;
            enemy.ManaPerSecond = data.ManaPerSecond;

            UIManager.Instance.SetEnemyUI(enemy);
        }
    }
}
