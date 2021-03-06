﻿using System;
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
        /// In game unique identifier
        /// </summary>
        public int InGameID;

        public List<GlobalEffect> GlobalEffects = new List<GlobalEffect>();

        public Block[,] Blocks;

        public List<Block> DestroyedBlocks = new List<Block>();

        public List<Block> LockedBlocks = new List<Block>();

        /// <summary>
        /// Checks if index outside borders and returns block if not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Block GetBlock(int x, int y)
        {
            if (x < 0 || x >= Blocks.GetLength(0) || y < 0 || y >= Blocks.GetLength(1))
                return null;
            return Blocks[x, y];
        }

        /// <summary>
        /// Removes shield effect if has one
        /// </summary>
        /// <returns></returns>
        public bool TryBlock(out GlobalEffect effect)
        {
            if ((effect = GlobalEffects.FirstOrDefault(
                ge => ge.Type == GlobalEffectType.Shield)) != null)
            {
                return GlobalEffects.Remove(effect);
            }

            return false;
        }

        public FieldData ToData()
        {
            int w = Blocks.GetLength(0);
            int h = Blocks.GetLength(1);

            FieldData data = new FieldData();
            data.InGameID = InGameID;
            data.GlobalEffects = GlobalEffects.Select(e => e.ToData()).ToArray();
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
