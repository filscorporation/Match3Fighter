using System;
using Assets.Source.FieldManagement;
using Assets.Source.NetworkManagement;
using NetworkShared.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }

        private const string MainMenuSceneName = "MainMenu";
        private const string GameSceneName = "GameScene";

        public static StartGameResponse GameDataToStart;

        public void Awake()
        {
            if (SceneManager.GetActiveScene().name == GameSceneName)
            {
                if (GameDataToStart == null)
                    throw new Exception("No game data to start game");

                StartGame(GameDataToStart);
            }
        }

        private void StartGame(StartGameResponse response)
        {
            Debug.Log("Starting game");

            FieldManager.Instance.GenerateMainField(response.MainField);
            FieldManager.Instance.GenerateEnemyField(response.EnemyField);
        }

        /// <summary>
        /// Puts player into the game queue
        /// </summary>
        public void QueueGame()
        {
            Debug.Log("Queue game");

            PutPlayerIntoQueueRequest request = new PutPlayerIntoQueueRequest();
            NetworkManager.Instance.SendPutPlayerIntoQueueRequestRequest(request);
        }

        /// <summary>
        /// Starts game from game data
        /// </summary>
        /// <param name="response"></param>
        public void LoadGameScene(StartGameResponse response)
        {
            Debug.Log("Loading game");

            GameDataToStart = response;
            SceneManager.LoadScene(GameSceneName);
        }
    }
}
