using System;
using System.Collections.Generic;
using NetworkShared.Data.Field;

namespace NetworkShared.Data.Player
{
    [Serializable]
    public class UniqueBlockCollectionData
    {
        public UniqueBlockData[] Collection;

        public Dictionary<BlockTypes, UniqueBlockData> Level1Blocks;

        public Dictionary<BlockTypes, UniqueBlockData> Level2Blocks;

        public Dictionary<BlockTypes, UniqueBlockData> Level3Blocks;
    }
}
