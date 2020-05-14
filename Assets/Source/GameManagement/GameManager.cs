using System;
using Assets.Source.FieldManagement;
using Assets.Source.InputManagement;
using Assets.Source.NetworkManagement;
using Assets.Source.PlayerManagement;
using Assets.Source.UIManagement;
using Assets.Source.UpgradeManagement;
using NetworkShared.Data;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;
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
                {
                    Debug.LogError("No game data to start game");
                    FinishGame();
                    return;
                }

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

            PutPlayerIntoQueueRequest request = new PutPlayerIntoQueueRequest
            {
                DebugMode = false,
            };
            NetworkManager.Instance.SendPutPlayerIntoQueueRequestRequest(request);
        }

        /// <summary>
        /// Puts player into the game queue with single player option as debug mode
        /// </summary>
        public void QueueDebugGame()
        {
            Debug.Log("Queue game");

            PutPlayerIntoQueueRequest request = new PutPlayerIntoQueueRequest
            {
                DebugMode = true,
            };
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

            UpgradeManager.Instance.ApplyUpgradeInfo(data.GameState.MainUpgradesInfo);

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
                        ProcessGlobalEffect(effect);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ProcessGlobalEffect(EffectData effect)
        {
            GlobalEffectType type = (GlobalEffectType)effect.Data["Type"];
            bool created = (bool)effect.Data["Created"];
            int target;
            float value;

            switch (type)
            {
                case GlobalEffectType.Shield:
                    PlayerManager.Instance.ActivateShieldEffect(effect);
                    break;
                case GlobalEffectType.HealOverTime:
                    target = (int)effect.Data["Target"];
                    value = (float)effect.Data["Value"];
                    if (created)
                        PlayerManager.Instance.GetPlayerByID(target).HealthPerSecondFromEffect += value;
                    else
                        PlayerManager.Instance.GetPlayerByID(target).HealthPerSecondFromEffect -= value;
                    break;
                case GlobalEffectType.ManaOverTime:
                    target = (int)effect.Data["Target"];
                    value = (float)effect.Data["Value"];
                    if (created)
                        PlayerManager.Instance.GetPlayerByID(target).ManaPerSecondFromEffects += value;
                    else
                        PlayerManager.Instance.GetPlayerByID(target).ManaPerSecondFromEffects -= value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        /// <summary>
        /// Request to upgrade block type
        /// </summary>
        /// <param name="blockType"></param>
        public void OnPlayerUpgrade(BlockTypes blockType)
        {
            UpgradeRequest request = new UpgradeRequest();
            request.UpgradeBlockType = blockType;

            NetworkManager.Instance.SendUpgradeData(request);
        }
    }
}
