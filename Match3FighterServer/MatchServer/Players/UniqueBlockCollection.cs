using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchServer.FieldManagement;
using NetworkShared.Data.Field;

namespace MatchServer.Players
{
    /// <summary>
    /// Active unique blocks
    /// </summary>
    public class UniqueBlockCollection
    {
        public Dictionary<BlockTypes, UniqueBlock> Level1Blocks = new Dictionary<BlockTypes, UniqueBlock>();

        public Dictionary<BlockTypes, UniqueBlock> Level2Blocks = new Dictionary<BlockTypes, UniqueBlock>();

        public Dictionary<BlockTypes, UniqueBlock> Level3Blocks = new Dictionary<BlockTypes, UniqueBlock>();
    }
}
