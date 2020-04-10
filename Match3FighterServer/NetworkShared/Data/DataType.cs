namespace NetworkShared.Data
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

        // Game process
        BlockSwapRequest = 20,
        GameStateResponse = 21,

        // Error
        ErrorResponse = 1000,
    }
}
