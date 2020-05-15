using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.NetworkManagement;
using Assets.Source.UIManagement;
using NetworkShared.Data;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;
using UnityEngine;

namespace Assets.Source.PlayerManagement
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private static PlayerStatsManager instance;

        public static PlayerStatsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerStatsManager>();
                }
                return instance;
            }
        }

        public PlayerStats PlayerStats = null;

        public void Start()
        {
            StartCoroutine(LoadStatsDelayed());
        }

        private IEnumerator LoadStatsDelayed()
        {
            yield return new WaitUntil(() => NetworkManager.Instance.IsConnected);
            LoadPlayerStats();
        }

        /// <summary>
        /// Loads players stats from server
        /// </summary>
        /// <returns></returns>
        public void LoadPlayerStats()
        {
            NetworkManager.Instance.SendGetPlayerStatsRequest();
        }

        /// <summary>
        /// Updates current stats with new
        /// </summary>
        /// <param name="data"></param>
        public void SetPlayerStats(PlayerStatsData data)
        {
            PlayerStats = new PlayerStats
            {
                Collection = data.UniqueBlockCollection.Collection.ToList(),
                Level1Blocks = data.UniqueBlockCollection.Level1Blocks,
                Level2Blocks = data.UniqueBlockCollection.Level2Blocks,
                Level3Blocks = data.UniqueBlockCollection.Level3Blocks,
            };
        }

        /// <summary>
        /// Draw player stats on UI
        /// </summary>
        public void DrawPlayerStats()
        {
            if (PlayerStats == null)
                return;

            PlayerStatsUI.Instance.RedrawLevel1Blocks(PlayerStats.Level1Blocks);
            PlayerStatsUI.Instance.RedrawLevel2Blocks(PlayerStats.Level2Blocks);
            PlayerStatsUI.Instance.RedrawLevel3Blocks(PlayerStats.Level3Blocks);
            //PlayerStatsUI.Instance.RedrawCollection(PlayerStats.Collection.Select(v => v.Name));
        }

        /// <summary>
        /// Opens filtered menu to choose block
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="type"></param>
        /// <param name="level"></param>
        public void DrawFilteredCollection(GameObject initiator, BlockTypes type, int level)
        {
            IEnumerable<UniqueBlockData> filtered = PlayerStats.Collection
                .Where(v => v.Level == level
                    // TODO: make local match method for types like on server
                    && (v.Type == type || v.Type == BlockTypes.Chameleon));
            PlayerStatsUI.Instance.RedrawCollection(initiator, filtered);
        }

        /// <summary>
        /// Set new unique block to the slot
        /// </summary>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <param name="newBlockName"></param>
        public void SetUniqueBlock(BlockTypes type, int level, string newBlockName)
        {
            switch (level)
            {
                case 1:
                    PlayerStats.Level1Blocks[type] = PlayerStats.Collection
                        .First(b => b.Name == newBlockName);
                    PlayerStatsUI.Instance.RedrawLevel1Blocks(PlayerStats.Level1Blocks);
                    break;
                case 2:
                    PlayerStats.Level2Blocks[type] = PlayerStats.Collection
                        .First(b => b.Name == newBlockName);
                    PlayerStatsUI.Instance.RedrawLevel2Blocks(PlayerStats.Level2Blocks);
                    break;
                case 3:
                    PlayerStats.Level3Blocks[type] = PlayerStats.Collection
                        .First(b => b.Name == newBlockName);
                    PlayerStatsUI.Instance.RedrawLevel3Blocks(PlayerStats.Level3Blocks);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }

            PlayerStatsUI.Instance.HideCollection();

            SavePlayerStats();
        }

        /// <summary>
        /// Hides collection of blocks UI
        /// </summary>
        public void HideCollection()
        {
            PlayerStatsUI.Instance.HideCollection();
        }

        /// <summary>
        /// Sends players stats to server
        /// </summary>
        /// <returns></returns>
        public void SavePlayerStats()
        {
            SetPlayerStatsRequest request = new SetPlayerStatsRequest
            {
                PlayerStats = PlayerStats.ToData(),
            };
            NetworkManager.Instance.SendSetPlayerStatsRequest(request);
        }
    }
}
