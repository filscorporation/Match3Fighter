using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Info about field
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Ingame unique identifier
        /// </summary>
        public int InGameID;

        public Block[,] Blocks;

        /// <summary>
        /// Checks if index outside borders and returns block if not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Block GetBlock(int x, int y)
        {
            if (x < 0 || x >= Blocks.GetLength(0) || y < 0 || x >= Blocks.GetLength(1))
                return null;
            return Blocks[x, y];
        }

        public FieldData ToData()
        {
            int w = Blocks.GetLength(0);
            int h = Blocks.GetLength(1);

            FieldData data = new FieldData();
            data.InGameID = InGameID;
            data.Blocks = new BlockData[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    data.Blocks[i, j] = Blocks[i, j].ToData();
                }
            }

            return data;
        }
    }
}
