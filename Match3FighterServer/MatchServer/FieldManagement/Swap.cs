namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Simple players turn
    /// </summary>
    public struct Swap
    {
        public int X;

        public int Y;

        /// <summary>
        /// Direction of the swap, where: 0 - up, 1 - right, 2 - down, 3 - left
        /// </summary>
        public int Direction;

        public Swap(int x, int y, int direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }
    }
}
