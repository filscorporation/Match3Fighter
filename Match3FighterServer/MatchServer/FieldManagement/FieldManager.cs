using System;
using System.Collections.Generic;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Generates and updates game fields
    /// </summary>
    public class FieldManager
    {
        private const int fieldWidth = 6;
        private const int fieldHeight = 6;

        private const int minComboCount = 3;

        private BlockEffectsManager blockEffectsManager;
        private readonly Random random;

        public FieldManager()
        {
            blockEffectsManager = new BlockEffectsManager();
            random = new Random();
        }

        /// <summary>
        /// Generates random field for player
        /// </summary>
        /// <param name="player"></param>
        public Field GenerateFieldForPlayer(Player player)
        {
            Field field = new Field();

            field.Blocks = new Block[fieldWidth, fieldHeight];
            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    field.Blocks[i, j] = Block.GetRandomBlock(random);
                    field.Blocks[i, j].X = i;
                    field.Blocks[i, j].Y = j;
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
        public bool TryRebuildFieldFromSwap(Field field, Swap swap, out List<Effect> effects)
        {
            effects = new List<Effect>();

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

            if (blockA.Type == blockB.Type)
                return false;

            field.Blocks[swap.X, swap.Y] = blockB;
            blockB.X = swap.X;
            blockB.Y = swap.Y;
            field.Blocks[nx, ny] = blockA;
            blockA.X = nx;
            blockA.Y = ny;

            List<List<Block>> combos = CheckForCombos(field, new List<Block> { blockA, blockB });
            foreach (List<Block> combo in combos)
            {
                effects.Add(blockEffectsManager.GetEffectFromCombo(combo));
                DestroyBlocks(field, combo, BlockState.DestroyedAsCombo);
            }

            FillHoles(field);

            return true;
        }

        /// <summary>
        /// Returns list of possile combos, that includes any of passed blocks
        /// </summary>
        /// <param name="field"></param>
        /// <param name="includeAny"></param>
        private List<List<Block>> CheckForCombos(Field field, List<Block> includeAny)
        {
            List<List<Block>> combos = new List<List<Block>>();

            foreach (Block block in includeAny)
            {
                List<Block> ver = new List<Block>();
                AddAllSameVertically(field, ver, block);
                List<Block> hor = new List<Block>();
                AddAllSameHorizontally(field, hor, block);

                if (Math.Max(ver.Count, hor.Count) >= minComboCount)
                {
                    combos.Add(ver.Count > hor.Count ? ver : hor);
                }
            }

            return combos;
        }

        private void AddAllSameHorizontally(Field field, List<Block> outResult, Block start)
        {
            outResult.Add(start);
            for (int i = start.X + 1; i < field.Blocks.GetLength(0); i++)
            {
                Block block = field.GetBlock(i, start.Y);
                if (block != null && block.CanCombo(start))
                    outResult.Add(block);
                else
                    break;
            }
            for (int i = start.X - 1; i >= 0; i--)
            {
                Block block = field.GetBlock(i, start.Y);
                if (block != null && block.CanCombo(start))
                    outResult.Add(block);
                else
                    break;
            }
        }

        private void AddAllSameVertically(Field field, List<Block> outResult, Block start)
        {
            outResult.Add(start);
            for (int j = start.Y + 1; j < field.Blocks.GetLength(1); j++)
            {
                Block block = field.GetBlock(start.X, j);
                if (block != null && block.CanCombo(start))
                    outResult.Add(block);
                else
                    break;
            }
            for (int j = start.Y - 1; j >= 0; j--)
            {
                Block block = field.GetBlock(start.X, j);
                if (block != null && block.CanCombo(start))
                    outResult.Add(block);
                else
                    break;
            }
        }

        private void DestroyBlocks(Field field, List<Block> blocks, BlockState destroyedState)
        {
            foreach (Block block in blocks)
            {
                block.State = destroyedState;
            }
        }

        private void FillHoles(Field field)
        {
            int w = field.Blocks.GetLength(0);
            int h = field.Blocks.GetLength(1);
            
            for (int i = 0; i < w; i++)
            {
                int offset = 0;
                for (int j = 0; j < h; j++)
                {
                    Block block = field.Blocks[i, j];
                    if (block.IsInDestroyedState())
                    {
                        offset++;
                    }
                    else
                    {
                        if (offset > 0)
                        {
                            block.ReplacedBlock = field.Blocks[i, j - offset];
                            field.Blocks[i, j - offset] = block;
                            block.X = i;
                            block.Y = j - offset;
                            field.Blocks[i, j] = null;
                        }
                    }
                }

                for (int j = h - 1; j >= 0; j--)
                {
                    if (offset <= 0)
                        break;
                    field.Blocks[i, j] = Block.GetRandomBlock(random);
                    field.Blocks[i, j].X = i;
                    field.Blocks[i, j].Y = j;
                    offset--;
                }
            }
        }
    }
}
