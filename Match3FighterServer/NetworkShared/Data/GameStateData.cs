using System;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;
using NetworkShared.Data.Upgrades;

namespace NetworkShared.Data
{
    [Serializable]
    public class GameStateData
    {
        public PlayerData MainPlayer;

        public PlayerData EnemyPlayer;

        public FieldData MainField;

        public FieldData EnemyField;

        public UpgradesInfoData MainUpgradesInfo;

        public UpgradesInfoData EnemyUpgradesInfo;
    }
}
