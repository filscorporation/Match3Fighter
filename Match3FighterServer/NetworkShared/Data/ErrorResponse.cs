using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkShared.Data
{
    public enum ErrorType
    {
        Default,
        ImpossibleTurn,
        NotEnoughMana,
    }

    [Serializable]
    public class ErrorResponse
    {
        public ErrorType Type;
    }
}
