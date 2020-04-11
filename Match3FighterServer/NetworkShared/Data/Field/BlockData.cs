using System;

namespace NetworkShared.Data.Field
{
    public enum BlockState
    {
        Default,
        Created,
        DroppedAsNew,
        Moved,
        DestroyedAsCombo,
        Damaged,
        DestroyedByDamage,
        CreatedAsComboResult,
    }

    [Serializable]
    public class BlockData
    {
        public BlockState State;

        public int ID;

        public BlockData ReplacedBlock;
    }
}
