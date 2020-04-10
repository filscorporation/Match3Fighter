using System;
using Assets.Source.GameManagement;
using NetworkShared.Core;
using NetworkShared.Data;
using UnityEngine;

namespace Assets.Source.NetworkManagement
{
    /// <summary>
    /// Connects client to the server and handles messages
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager instance;

        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<NetworkManager>();
                }
                return instance;
            }
        }

        private static Client client;

        #region Core

        public void Awake()
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();
        }

        public void Update()
        {
            ThreadManager.UpdateMain();
        }

        public void OnApplicationQuit()
        {
            DisconnectPlayer();
        }

        public void ConnectPlayer()
        {
            Debug.Log("Connecting player");
            client = new Client(this);
            client.ConnectToServer();
        }

        public void DisconnectPlayer()
        {
            client?.Disconnect();
            client = null;
        }

        #endregion

        #region Receive

        public void ClientReceiveData(int type, object data)
        {
            switch ((DataTypes)type)
            {
                case DataTypes.ConnectResponse:
                    break;
                case DataTypes.LogInResponse:
                    break;
                case DataTypes.StartGameResponse:
                    HandleStartGameResponse((StartGameResponse) data);
                    break;
                case DataTypes.GameStateResponse:
                    HandleGameStateResponse((GameStateResponse) data);
                    break;
                case DataTypes.ErrorResponse:
                    Debug.Log(((ErrorResponse)data).Type);
                    break;
                case DataTypes.LogInRequest:
                case DataTypes.PutPlayerIntoQueueRequest:
                case DataTypes.BlockSwapRequest:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void HandleStartGameResponse(StartGameResponse response)
        {
            GameManager.Instance.LoadGameScene(response);
        }

        public void HandleGameStateResponse(GameStateResponse response)
        {
            GameManager.Instance.ChangeGameState(response.GameState);
        }

        public void HandleGameStateResponse(StartGameResponse response)
        {
            GameManager.Instance.LoadGameScene(response);
        }

        #endregion

        #region Send

        public void SendPutPlayerIntoQueueRequestRequest(PutPlayerIntoQueueRequest request)
        {
            client.SendData((int)DataTypes.PutPlayerIntoQueueRequest, request);
        }

        public void SendBlockSwapData(BlockSwapRequest request)
        {
            client.SendData((int)DataTypes.BlockSwapRequest, request);
        }

        #endregion
    }
}
