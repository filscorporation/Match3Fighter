using System;
using Assets.Source.FieldManagement;
using Assets.Source.InputManagement;
using Assets.Source.NetworkManagement;
using Assets.Source.PlayerManagement;
using Assets.Source.UIManagement;
using NetworkShared.Data;
using NetworkShared.Data.Effects;
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
        public void ChangeGameState(GameStateResponse data)
        {
            FieldManager.Instance.DeleteFields();

            PlayerManager.Instance.SetPlayerState(data.GameState.MainPlayer);
            PlayerManager.Instance.SetEnemyState(data.GameState.EnemyPlayer);

            FieldManager.Instance.GenerateMainField(data.GameState.MainField);
            FieldManager.Instance.GenerateEnemyField(data.GameState.EnemyField);

            foreach (EffectData effect in data.Effects)
            {
                switch (effect.EffectType)
                {
                    case EffectType.HealthChanged:
                        PlayerManager.Instance.AnimateHealthEffect(effect);
                        break;
                    case EffectType.ManaChanged:
                        PlayerManager.Instance.AnimateManaEffect(effect);
                        break;
                    case EffectType.BlockShot:
                        FieldManager.Instance.DrawShootEffect(effect);
                        break;
                    case EffectType.UniqueEffect:
                        // TODO: temporary
                        FieldManager.Instance.DrawShootEffect(effect);
                        break;
                    case EffectType.GlobalEffect:
                        PlayerManager.Instance.ActivateShieldEffect(effect);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// End game when one of players won
        /// </summary>
        /// <param name="data"></param>
        public void EndGame(GameEndResponse data)
        {
            FieldManager.Instance.CanControl = false;

            PlayerManager.Instance.ShowPlayerWonOrLost(data.PlayerWon);
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
