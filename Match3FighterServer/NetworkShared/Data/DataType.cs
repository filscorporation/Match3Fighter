namespace NetworkShared.Data
{
    /// <summary>
    /// Types for data server and client sends and receives
    /// </summary>
    public enum DataTypes
    {
        ConnectResponse = 1,
        LogInRequest = 2,
        LogInResponse = 3,

        PutPlayerIntoQueueRequest = 10,
        StartGameResponse = 11,
    }
}
