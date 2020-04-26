using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkShared.Data.Field
{
    [Serializable]
    public class FieldData
    {
        public int InGameID;

        public BlockData[,] Blocks;
    }
}
