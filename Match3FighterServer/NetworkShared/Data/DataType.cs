﻿namespace NetworkShared.Data
{
    /// <summary>
    /// Types for data server and client sends and receives
    /// </summary>
    public enum DataTypes
    {
        // Connections
        ConnectResponse = 1,
        LogInRequest = 2,
        LogInResponse = 3,

        // Game creation
        PutPlayerIntoQueueRequest = 10,
        StartGameResponse = 11,

        // Game proccess
        BlockSwapRequest = 20,
        GameStateResponse = 21,
        GameEndResponse = 22,
        UpgradeRequest = 23,
        BlockTapRequest = 24,
        UseSkillRequest = 25,

        // Player setup
        GetPlayerStatsRequest = 30,
        SetPlayerStatsRequest = 31,
        PlayerStatsResponse = 32,

        // Error
        ErrorResponse = 1000,
    }
}
