using System.Collections.Generic;
using System.Linq;
using MatchServer.FieldManagement;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;

namespace MatchServer.Players
{
    /// <summary>
    /// Unique blocks collection of the player
    /// </summary>
    public class UniqueBlockCollection
    {
        /// <summary>
        /// All available block for the player
        /// </summary>
        public List<UniqueBlock> Collection = new List<UniqueBlock>();

        /// <summary>
        /// Active level 1 blocks
        /// </summary>
        public Dictionary<BlockTypes, UniqueBlock> Level1Blocks = new Dictionary<BlockTypes, UniqueBlock>();

        /// <summary>
        /// Active level 2 blocks
        /// </summary>
        public Dictionary<BlockTypes, UniqueBlock> Level2Blocks = new Dictionary<BlockTypes, UniqueBlock>();

        /// <summary>
        /// Active level 3 blocks
        /// </summary>
        public Dictionary<BlockTypes, UniqueBlock> Level3Blocks = new Dictionary<BlockTypes, UniqueBlock>();

        public UniqueBlockCollectionData ToData()
        {
            return new UniqueBlockCollectionData
            {
                Collection = Collection.Select(v => v.ToData()).ToArray(),
                Level1Blocks = Level1Blocks.ToDictionary(
                    v => v.Key,
                    v => v.Value.ToData()),
                Level2Blocks = Level2Blocks.ToDictionary(
                    v => v.Key,
                    v => v.Value.ToData()),
                Level3Blocks = Level3Blocks.ToDictionary(
                    v => v.Key,
                    v => v.Value.ToData()),
            };
        }
    }
}
