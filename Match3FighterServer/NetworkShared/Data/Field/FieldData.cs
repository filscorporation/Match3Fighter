using System;

namespace NetworkShared.Data.Field
{
    [Serializable]
    public class FieldData
    {
        public int InGameID;

        public GlobalEffectData[] GlobalEffects;

        public BlockData[,] Blocks;
    }
}
