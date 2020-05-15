using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Source.PlayerManagement;
using NetworkShared.Data.Field;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// UI of player stats: active hero, collection, unique blocks
    /// </summary>
    public class PlayerStatsUI : MonoBehaviour
    {
        private static PlayerStatsUI instance;

        public static PlayerStatsUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerStatsUI>();
                }
                return instance;
            }
        }

        public Transform StatsMenuParent;
        public GameObject BlockPrefab;
        private const float blockIconSize = 140F;
        private const float level1YOffset = -320;
        private const float xOffset = 10;
        private const float level2YOffset = level1YOffset - blockIconSize * 1.5F;
        private const float level3YOffset = level1YOffset - blockIconSize * 3F;
        private const float collectionYOffset = level1YOffset - blockIconSize * 4.5F;
        private const string uniqueBlockSpritesPath = "UniqueBlockSprites";

        private readonly List<GameObject> level1Blocks = new List<GameObject>();
        private readonly List<GameObject> level2Blocks = new List<GameObject>();
        private readonly List<GameObject> level3Blocks = new List<GameObject>();
        private readonly List<GameObject> collection = new List<GameObject>();

        public void RedrawLevel1Blocks(Dictionary<BlockTypes, UniqueBlockData> blocks)
        {
            foreach (GameObject block in level1Blocks)
            {
                Destroy(block);
            }
            level1Blocks.Clear();
            level1Blocks.AddRange(DrawBlockLine(blocks, level1YOffset));
        }

        public void RedrawLevel2Blocks(Dictionary<BlockTypes, UniqueBlockData> blocks)
        {
            foreach (GameObject block in level2Blocks)
            {
                Destroy(block);
            }
            level2Blocks.Clear();
            level2Blocks.AddRange(DrawBlockLine(blocks, level2YOffset));
        }

        public void RedrawLevel3Blocks(Dictionary<BlockTypes, UniqueBlockData> blocks)
        {
            foreach (GameObject block in level3Blocks)
            {
                Destroy(block);
            }
            level3Blocks.Clear();
            level3Blocks.AddRange(DrawBlockLine(blocks, level3YOffset));
        }

        public void RedrawCollection(GameObject initiator, IEnumerable<UniqueBlockData> blocksNames)
        {
            DisableAllExcept(initiator);

            foreach (GameObject block in collection)
            {
                Destroy(block);
            }
            collection.Clear();

            int i = 0;
            foreach (List<UniqueBlockData> subList in SplitList(blocksNames.ToList(), 4))
            {
                collection.AddRange(DrawBlockLine(subList, collectionYOffset - i * blockIconSize * 1.5F));
                i++;
            }
        }

        public void HideCollection()
        {
            EnableAll();

            foreach (GameObject block in collection)
            {
                Destroy(block);
            }
            collection.Clear();
        }

        private void DisableAllExcept(GameObject go)
        {
            foreach (GameObject block in level1Blocks)
            {
                if (block == go)
                    continue;
                block.GetComponent<Button>().interactable = false;
            }
            foreach (GameObject block in level2Blocks)
            {
                if (block == go)
                    continue;
                block.GetComponent<Button>().interactable = false;
            }
            foreach (GameObject block in level3Blocks)
            {
                if (block == go)
                    continue;
                block.GetComponent<Button>().interactable = false;
            }
        }

        private void EnableAll()
        {
            foreach (GameObject block in level1Blocks)
            {
                block.GetComponent<Button>().interactable = true;
            }
            foreach (GameObject block in level2Blocks)
            {
                block.GetComponent<Button>().interactable = true;
            }
            foreach (GameObject block in level3Blocks)
            {
                block.GetComponent<Button>().interactable = true;
            }
        }

        private IEnumerable<GameObject> DrawBlockLine(Dictionary<BlockTypes, UniqueBlockData> blocks, float yOffset)
        {
            int i = 0;
            foreach (KeyValuePair<BlockTypes, UniqueBlockData> block in blocks)
            {
                GameObject go = Instantiate(BlockPrefab);
                go.transform.SetParent(StatsMenuParent);
                go.GetComponent<Image>().sprite = GetUniqueBlockSprite(block.Value.Name);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(xOffset + blockIconSize / 2 + i * blockIconSize * 1.5F, yOffset);
                rt.sizeDelta = new Vector2(blockIconSize, blockIconSize);
                go.transform.localScale = new Vector3(1, 1, 1);

                Button button = go.GetComponent<Button>();
                button.onClick.AddListener(() =>
                    PlayerStatsManager.Instance.DrawFilteredCollection(go, block.Key, block.Value.Level));

                    i++;

                yield return go;
            }
        }

        private IEnumerable<GameObject> DrawBlockLine(IEnumerable<UniqueBlockData> blocks, float yOffset)
        {
            int i = 0;
            foreach (UniqueBlockData block in blocks)
            {
                GameObject go = Instantiate(BlockPrefab);
                go.transform.SetParent(StatsMenuParent);
                go.GetComponent<Image>().sprite = GetUniqueBlockSprite(block.Name);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(xOffset + blockIconSize / 2 + i * blockIconSize * 1.5F, yOffset);
                rt.sizeDelta = new Vector2(blockIconSize, blockIconSize);
                go.transform.localScale = new Vector3(1, 1, 1);

                Button button = go.GetComponent<Button>();
                button.onClick.AddListener(() =>
                    PlayerStatsManager.Instance.SetUniqueBlock(block.Type, block.Level, block.Name));

                i++;

                yield return go;
            }
        }

        private Sprite GetUniqueBlockSprite(string blockName)
        {
            return Resources.Load<Sprite>(Path.Combine(uniqueBlockSpritesPath, blockName));
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}
