using System;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Generates and updates game fields
    /// </summary>
    public class FieldManager
    {
        private const int FieldWidth = 6;
        private const int FieldHeight = 6;

        private readonly Random random;

        public FieldManager()
        {
            random = new Random();
        }

        /// <summary>
        /// Generates random field for player
        /// </summary>
        /// <param name="player"></param>
        public Field GenerateFieldForPlayer(Player player)
        {
            Field field = new Field();

            field.Blocks = new Block[FieldWidth, FieldHeight];
            for (int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    field.Blocks[i, j] = Block.GetRandomBlock(random);
                }
            }

            return field;
        }

        /// <summary>
        /// Processes block swap and rebuilds field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="swap"></param>
        /// <returns>True if swap is possible</returns>
        public bool TryRebuildFieldFromSwap(Field field, Swap swap)
        {
            int w = field.Blocks.GetLength(0);
            int h = field.Blocks.GetLength(1);

            if (swap.X < 0 || swap.X > w - 1 || swap.Y < 0 || swap.Y > h - 1)
                return false;

            int xo = swap.Direction == 1 ? 1 : swap.Direction == 3 ? -1 : 0;
            int yo = swap.Direction == 0 ? 1 : swap.Direction == 2 ? -1 : 0;

            int nx = swap.X + xo;
            int ny = swap.Y + yo;

            if (nx < 0 || nx > w - 1 || ny < 0 || ny > h - 1)
                return false;

            Block blockA = field.Blocks[swap.X, swap.Y];
            Block blockB = field.Blocks[nx, ny];

            field.Blocks[swap.X, swap.Y] = blockB;
            field.Blocks[nx, ny] = blockA;

            // TODO: check for 3 in a row and etc..

            return true;
        }
    }
}
