using System;
using MatchServer.Players;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Generates and updates game fields
    /// </summary>
    public class FieldManager
    {
        private const int FieldWidth = 6;
        private const int FieldHeight = 6;

        /// <summary>
        /// Generates random field for player
        /// </summary>
        /// <param name="player"></param>
        public Field GenerateFieldForPlayer(Player player)
        {
            Random random = new Random();

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
    }
}
