using System;
using System.Collections.Generic;
using NetworkShared.Data.Field;
using NetworkShared.Data.Upgrades;

namespace MatchServer.UpgradesManagement
{
    /// <summary>
    /// Info about one players upgrades in current game match
    /// </summary>
    public class UpgradesInfo
    {
        public UpgradesInfo()
        {
            UpgradesCount = new Dictionary<BlockTypes, int>();
            foreach (object value in Enum.GetValues(typeof(BlockTypes)))
            {
                UpgradesCount[(BlockTypes)value] = 0;
            }
        }

        public Dictionary<BlockTypes, int> UpgradesCount;

        public UpgradesInfoData ToData()
        {
            return new UpgradesInfoData
            {
                UpgradesCount = UpgradesCount,
            };
        }
    }
}
