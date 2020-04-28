using System.Collections.Generic;
using System.Linq;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Blocks combo (3 or more) with additional info
    /// </summary>
    public class Combo
    {
        public Combo(List<Block> blocks)
        {
            Type = blocks.FirstOrDefault(b => b.Type != BlockTypes.Chameleon)?.Type ?? BlockTypes.Chameleon;
            Blocks = blocks;
        }

        public BlockTypes Type;

        public List<Block> Blocks;

        public float EffectScale = 1F;
    }
}
