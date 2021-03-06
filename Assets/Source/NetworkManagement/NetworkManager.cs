﻿using System;
using Assets.Source.GameManagement;
using Assets.Source.PlayerManagement;
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

        public bool IsConnected { get; private set; } = false;
        public bool IsLoggedIn { get; private set; } = false;

        #region Core

        public void Awake()
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();
            else
            {
                IsConnected = true;
                // TODO: maybe player it not loged in...
                IsLoggedIn = true;
            }
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
                    IsConnected = true;
                    LogIn();
                    break;
                case DataTypes.LogInResponse:
                    HandleLogInResponse((LogInResponse)data);
                    break;
                case DataTypes.StartGameResponse:
                    HandleStartGameResponse((StartGameResponse) data);
                    break;
                case DataTypes.GameStateResponse:
                    HandleGameStateResponse((GameStateResponse)data);
                    break;
                case DataTypes.GameEndResponse:
                    HandleGameEndResponse((GameEndResponse)data);
                    break;
                case DataTypes.ErrorResponse:
                    Debug.Log(((ErrorResponse)data).Type);
                    break;
                case DataTypes.PlayerStatsResponse:
                    HandlePlayerStatsResponse((PlayerStatsResponse)data);
                    break;
                case DataTypes.UpgradeRequest:
                case DataTypes.GetPlayerStatsRequest:
                case DataTypes.SetPlayerStatsRequest:
                case DataTypes.LogInRequest:
                case DataTypes.PutPlayerIntoQueueRequest:
                case DataTypes.BlockSwapRequest:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void LogIn()
        {
            LogInRequest request = new LogInRequest();
            request.PlayerID = SystemInfo.deviceUniqueIdentifier;
            client.SendData((int)DataTypes.LogInRequest, request);
        }

        private void HandleLogInResponse(LogInResponse response)
        {
            IsLoggedIn = true;
            PlayerStatsManager.Instance.ShowPlayerLoggedIn(response.Type);
            PlayerStatsManager.Instance.SetPlayerStats(response.PlayerStats);
            PlayerStatsManager.Instance.DrawPlayerStats();
        }

        private void HandleStartGameResponse(StartGameResponse response)
        {
            GameManager.Instance.LoadGameScene(response);
        }

        private void HandleGameStateResponse(GameStateResponse response)
        {
            GameManager.Instance.ChangeGameState(response);
        }

        private void HandlePlayerStatsResponse(PlayerStatsResponse response)
        {
            PlayerStatsManager.Instance.SetPlayerStats(response.PlayerStats);
            PlayerStatsManager.Instance.DrawPlayerStats();
        }

        private void HandleGameEndResponse(GameEndResponse response)
        {
            GameManager.Instance.EndGame(response);
        }

        #endregion

        #region Send

        public void SendPutPlayerIntoQueueRequest(PutPlayerIntoQueueRequest request)
        {
            if (!IsConnected)
                return;

            client.SendData((int)DataTypes.PutPlayerIntoQueueRequest, request);
        }

        public void SendBlockSwapData(BlockSwapRequest request)
        {
            client.SendData((int)DataTypes.BlockSwapRequest, request);
        }

        public void SendBlockTapData(BlockTapRequest request)
        {
            client.SendData((int)DataTypes.BlockTapRequest, request);
        }

        public void SendUpgradeData(UpgradeRequest request)
        {
            client.SendData((int)DataTypes.UpgradeRequest, request);
        }

        public void SendUseSkillData(UseSkillRequest request)
        {
            client.SendData((int)DataTypes.UseSkillRequest, request);
        }

        public void SendGetPlayerStatsRequest()
        {
            if (!IsConnected)
                return;

            client.SendData((int)DataTypes.GetPlayerStatsRequest, new GetPlayerStatsRequest());
        }

        public void SendSetPlayerStatsRequest(SetPlayerStatsRequest request)
        {
            if (!IsConnected)
                return;

            client.SendData((int)DataTypes.SetPlayerStatsRequest, request);
        }

        #endregion
    }
}
