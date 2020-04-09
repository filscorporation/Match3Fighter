using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;

namespace NetworkShared.Data
{
    [Serializable]
    public class StartGameResponse
    {
        public PlayerData MainPlayer;

        public PlayerData EnemyPlayer;

        public FieldData MainField;

        public FieldData EnemyField;
    }
}
