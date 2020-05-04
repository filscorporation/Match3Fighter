using System.Collections;
using System.Collections.Generic;
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
        public GameObject EnemyShieldIcon;
        public GameObject PlayerShieldIcon;

        public GameObject PlayerStatsEffect;
        public List<GameObject> PlayerHPEffects = new List<GameObject>();
        public List<GameObject> PlayerMPEffects = new List<GameObject>();
        public List<GameObject> EnemyHPEffects = new List<GameObject>();
        public List<GameObject> EnemyMPEffects = new List<GameObject>();

        public void StartGameButtonClick()
        {
            GameManager.Instance.QueueGame();
        }

        public void DebugButtonClick()
        {
            GameManager.Instance.QueueDebugGame();
        }

        public void BackGameButtonClick()
        {
            GameManager.Instance.FinishGame();
        }

        public void SetPlayerUI(Player player)
        {
            PlayerNameText.text = player.Name;
            PlayerHPText.text = $"{Mathf.RoundToInt(player.Health)}hp";
            PlayerMPText.text = $"{Mathf.RoundToInt(player.Mana)}mp";
        }

        public void SetEnemyUI(Player player)
        {
            EnemyNameText.text = player.Name;
            EnemyHPText.text = $"{Mathf.RoundToInt(player.Health)}hp";
            EnemyMPText.text = $"{Mathf.RoundToInt(player.Mana)}mp";
        }

        private IEnumerator DestroyAndDelete(List<GameObject> list, GameObject obj)
        {
            yield return new WaitForSeconds(2F);
            list.Remove(obj);
            Destroy(obj);

            foreach (GameObject item in list)
            {
                RectTransform rect = item.GetComponent<RectTransform>();
                rect.offsetMin -= new Vector2(0, 80);
                rect.offsetMax -= new Vector2(0, 80);
            }
        }

        public void AnimatePlayerHealthUI(Player player, float value)
        {
            int i = PlayerHPEffects.Count;

            GameObject go = Instantiate(PlayerStatsEffect, Vector3.zero, Quaternion.identity, PlayerHPText.transform);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, (i + 1) * 80);
            rect.offsetMax = new Vector2(0, (i + 1) * 80);
            Text text = go.GetComponent<Text>();
            text.text = value.ToString("F1");
            text.color = value < 0 ? Color.red : Color.green;
            PlayerHPEffects.Add(go);

            StartCoroutine(DestroyAndDelete(PlayerHPEffects, go));
        }

        public void AnimateEnemyHealthUI(Player player, float value)
        {
            int i = EnemyHPEffects.Count;

            GameObject go = Instantiate(PlayerStatsEffect, Vector3.zero, Quaternion.identity, EnemyHPText.transform);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, (i + 1) * 80);
            rect.offsetMax = new Vector2(0, (i + 1) * 80);
            Text text = go.GetComponent<Text>();
            text.text = value.ToString("F1");
            text.color = value < 0 ? Color.red : Color.green;
            EnemyHPEffects.Add(go);

            StartCoroutine(DestroyAndDelete(EnemyHPEffects, go));
        }

        public void AnimatePlayerManaUI(Player player, float value)
        {
            int i = PlayerMPEffects.Count;

            GameObject go = Instantiate(PlayerStatsEffect, Vector3.zero, Quaternion.identity, PlayerMPText.transform);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, (i + 1) * 80);
            rect.offsetMax = new Vector2(0, (i + 1) * 80);
            Text text = go.GetComponent<Text>();
            text.text = value.ToString("F1");
            text.color = value < 0 ? Color.black : Color.blue;
            PlayerMPEffects.Add(go);

            StartCoroutine(DestroyAndDelete(PlayerMPEffects, go));
        }

        public void AnimateEnemyManaUI(Player player, float value)
        {
            int i = EnemyMPEffects.Count;

            GameObject go = Instantiate(PlayerStatsEffect, Vector3.zero, Quaternion.identity, EnemyMPText.transform);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, (i + 1) * 80);
            rect.offsetMax = new Vector2(0, (i + 1) * 80);
            Text text = go.GetComponent<Text>();
            text.text = value.ToString("F1");
            text.color = value < 0 ? Color.black : Color.blue;
            EnemyMPEffects.Add(go);

            StartCoroutine(DestroyAndDelete(EnemyMPEffects, go));
        }

        public void ActivatePlayerShieldEffect(bool isActive = true)
        {
            PlayerShieldIcon.SetActive(isActive);
        }

        public void ActivateEnemyShieldEffect(bool isActive = true)
        {
            EnemyShieldIcon.SetActive(isActive);
        }
    }
}
