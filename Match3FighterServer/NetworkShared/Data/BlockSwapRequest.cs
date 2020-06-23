using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class BlockSwapRequest
    {
        public int X;

        public int Y;

        /// <summary>
        /// Direction of the swap, where: 0 - up, 1 - right, 2 - down, 3 - left
        /// </summary>
        public int Direction;
    }
}
