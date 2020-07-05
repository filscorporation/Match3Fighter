using System;

namespace NetworkShared.Data
{
    public enum GameMode
    {
        Ranked,
        Practice,
    }

    [Serializable]
    public class PutPlayerIntoQueueRequest
    {
        public GameMode GameMode;

        public GameParameters GameParameters;
    }
}
