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

        public bool Equals(Swap other)
        {
            return X == other.X && Y == other.Y && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            return obj is Swap other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Direction;
                return hashCode;
            }
        }
    }
}
