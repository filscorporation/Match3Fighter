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
        public Block[,] Blocks;

        public FieldData ToData()
        {
            int w = Blocks.GetLength(0);
            int h = Blocks.GetLength(1);

            FieldData data = new FieldData();
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
