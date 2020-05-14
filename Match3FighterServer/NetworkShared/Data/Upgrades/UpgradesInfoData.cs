using System;
using System.Collections.Generic;
using NetworkShared.Data.Field;

namespace NetworkShared.Data.Upgrades
{
    [Serializable]
    public class UpgradesInfoData
    {
        public Dictionary<BlockTypes, int> UpgradesCount;
    }
}
