using System;
using Assets.Source.FieldManagement;
using Assets.Source.InputManagement;
using Assets.Source.NetworkManagement;
using Assets.Source.PlayerManagement;
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

        public InputManagerBase InputManager;

        private const string MainMenuSceneName = "MainMenuScene";
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

            PlayerManager.Instance.Initialize();

            PlayerManager.Instance.SetPlayerState(response.GameState.MainPlayer);
            PlayerManager.Instance.SetEnemyState(response.GameState.EnemyPlayer);

            FieldManager.Instance.GenerateMainField(response.GameState.MainField);
            FieldManager.Instance.GenerateEnemyField(response.GameState.EnemyField);
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
        /// Ends game
        /// </summary>
        public void FinishGame()
        {
            Debug.Log("Finish game");

            SceneManager.LoadScene(MainMenuSceneName);
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

        /// <summary>
        /// Change game state - players, fields etc
        /// </summary>
        /// <param name="data"></param>
        public void ChangeGameState(GameStateData data)
        {
            FieldManager.Instance.DeleteFields();

            PlayerManager.Instance.SetPlayerState(data.MainPlayer);
            PlayerManager.Instance.SetEnemyState(data.EnemyPlayer);

            FieldManager.Instance.GenerateMainField(data.MainField);
            FieldManager.Instance.GenerateEnemyField(data.EnemyField);
        }

        /// <summary>
        /// Request field state after block swap
        /// </summary>
        public void OnPlayerBlockSwap(int x, int y, BlockSwipeDirection direction)
        {
            Debug.Log("Send swap");

            BlockSwapRequest request = new BlockSwapRequest();
            request.X = x;
            request.Y = y;
            request.Direction = (int) direction;

            NetworkManager.Instance.SendBlockSwapData(request);
        }
    }
}
