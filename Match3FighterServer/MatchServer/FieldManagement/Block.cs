using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// One element of the field
    /// </summary>
    public class Block
    {
        public BlockTypes Type;

        public static Block GetRandomBlock(Random random)
        {
            Block block = new Block();

            int n = random.Next(Enum.GetNames(typeof(BlockTypes)).Length);
            block.Type = (BlockTypes)n;

            return block;
        }

        public BlockData ToData()
        {
            return new BlockData
            {
                ID = (int) Type,
            };
        }
    }
}
