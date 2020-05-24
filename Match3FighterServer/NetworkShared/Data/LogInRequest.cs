using System;

namespace NetworkShared.Data
{
    /// <summary>
    /// Request to login client
    /// </summary>
    [Serializable]
    public class LogInRequest
    {
        public string PlayerID;
    }
}
