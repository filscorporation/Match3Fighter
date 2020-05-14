using System;
using NetworkShared.Data.Field;

namespace NetworkShared.Data
{
    [Serializable]
    public class UpgradeRequest
    {
        public BlockTypes UpgradeBlockType;
    }
}
