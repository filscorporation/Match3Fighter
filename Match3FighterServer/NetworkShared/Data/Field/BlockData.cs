using System;

namespace NetworkShared.Data.Field
{
    public enum BlockState
    {
        DroppedAsNew,
        Moved,
        Swapped,
        DestroyedAsCombo,
        Damaged,
        DestroyedByDamage,
        CreatedAsComboResult,
    }

    [Serializable]
    public class BlockData
    {
        public int ID;

        public string UniqueBlock;

        public int X;

        public int Y;

        public BlockStateData[] PreviousStates;

        public BlockData ReplacedBlock;
    }

    [Serializable]
    public class BlockStateData
    {
        public BlockState State;

        public int X;

        public int Y;
    }
}
