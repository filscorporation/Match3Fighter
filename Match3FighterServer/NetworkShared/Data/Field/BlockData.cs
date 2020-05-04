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
        CreatedFromFlip,
        FlippedOver,
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

        public OnBlockEffectData[] OnBlockEffects;
    }

    [Serializable]
    public class BlockStateData
    {
        public BlockState State;

        public int X;

        public int Y;
    }
}
