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

        public BlockState State = BlockState.Default;

        public int X;

        public int Y;

        public Block ReplacedBlock;

        /// <summary>
        /// Returns randomly generated block
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Block GetRandomBlock(Random random)
        {
            Block block = new Block();

            int n = random.Next(Enum.GetNames(typeof(BlockTypes)).Length - 1);
            block.Type = (BlockTypes)n;

            return block;
        }

        /// <summary>
        /// Return true if blocks can make combo
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanCombo(BlockTypes type)
        {
            if (type == BlockTypes.Chameleon || Type == BlockTypes.Chameleon)
                return true;

            return type == Type;
        }

        /// <summary>
        /// Returns if block state is any of destroyed
        /// </summary>
        /// <returns></returns>
        public bool IsInDestroyedState()
        {
            return State == BlockState.DestroyedAsCombo || State == BlockState.DestroyedByDamage;
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
