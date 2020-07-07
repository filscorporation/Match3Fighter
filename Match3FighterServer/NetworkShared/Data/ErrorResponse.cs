using System;

namespace NetworkShared.Data
{
    public enum ErrorType
    {
        Default,
        ImpossibleTurn,
        NotEnoughMana,
        NotEnoughEnergy,
        LogInFailed,
    }

    [Serializable]
    public class ErrorResponse
    {
        public ErrorType Type;
    }
}
