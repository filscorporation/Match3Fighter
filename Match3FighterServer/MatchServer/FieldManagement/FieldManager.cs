using System;
using System.Collections.Generic;
using System.Linq;
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

        public const int MinComboCount = 3;

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
        /// Processes block swap
        /// </summary>
        /// <param name="field"></param>
        /// <param name="swap"></param>
        /// <param name="affected">Blocks that was affected by swap</param>
        /// <returns>True if swap is possible</returns>
        public bool TryRebuildFieldFromSwap(Field field, Swap swap, out List<Block> affected)
        {
            affected = new List<Block>();

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

            affected.Add(blockA);
            affected.Add(blockB);

            return true;
        }

        /// <summary>
        /// Returns random block on a field that is not in destroyed state
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public Block GetRandomNonDestroyedBlock(Field field)
        {
            int totalNonDestroyed = 0;
            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    if (!field.Blocks[i, j].IsInDestroyedState())
                        totalNonDestroyed++;
                }
            }

            if (totalNonDestroyed == 0)
                return null;

            int randomIndex = random.Next(0, totalNonDestroyed);

            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    if (!field.Blocks[i, j].IsInDestroyedState())
                    {
                        if (randomIndex == 0)
                            return field.Blocks[i, j];
                        randomIndex--;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns list of possile combos, that includes any of passed blocks
        /// </summary>
        /// <param name="field"></param>
        /// <param name="includeAny"></param>
        public List<List<Block>> CheckForCombos(Field field, List<Block> includeAny)
        {
            List<List<Block>> combos = new List<List<Block>>();

            foreach (Block block in includeAny)
            {
                List<Block> ver = AddAllSameVertically(field, block);
                List<Block> hor = AddAllSameHorizontally(field, block);

                if (Math.Max(ver.Count, hor.Count) >= MinComboCount)
                {
                    combos.Add(ver.Count > hor.Count ? ver : hor);
                }
            }

            return combos;
        }

        /// <summary>
        /// Put all blocks into destroyed state
        /// </summary>
        /// <param name="field"></param>
        /// <param name="blocks"></param>
        /// <param name="destroyedState"></param>
        public void DestroyBlocks(Field field, List<Block> blocks, BlockState destroyedState)
        {
            foreach (Block block in blocks)
            {
                block.State = destroyedState;
            }
        }

        /// <summary>
        /// Creates new block of passed type at position of random block in range
        /// </summary>
        /// <param name="field"></param>
        /// <param name="blockType"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Block CreateBlockInRange(Field field, BlockTypes blockType, List<Block> range)
        {
            Block block = new Block();
            block.Type = blockType;
            block.State = BlockState.CreatedAsComboResult;

            int n = random.Next(0, range.Count);
            Block blockToReplace = range[n];
            block.X = blockToReplace.X;
            block.Y = blockToReplace.Y;
            block.ReplacedBlock = blockToReplace;

            field.Blocks[block.X, block.Y] = block;

            return block;
        }

        /// <summary>
        /// Fills all destroyed blocks with new from above
        /// </summary>
        /// <param name="field"></param>
        public void FillHoles(Field field)
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
                    field.Blocks[i, j].State = BlockState.Created;
                    offset--;
                }
            }
        }

        // TODO: merge next two methods
        private List<Block> AddAllSameHorizontally(Field field, Block start)
        {
            List<Block> outResult = new List<Block>();
            outResult.Add(start);

            BlockTypes comboType = start.Type;
            for (int i = start.X + 1; i < field.Blocks.GetLength(0); i++)
            {
                Block block = field.GetBlock(i, start.Y);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType))
                    outResult.Add(block);
                else
                    break;
            }

            if (start.Type == BlockTypes.Chameleon)
                comboType = BlockTypes.Chameleon;
            List<Block> altResult = new List<Block>();

            for (int i = start.X - 1; i >= 0; i--)
            {
                Block block = field.GetBlock(i, start.Y);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType))
                    altResult.Add(block);
                else
                    break;
            }

            if (start.Type == BlockTypes.Chameleon)
            {
                // Middle block for combo was chameleon, we should look which side combo is longer
                Block altNonChameleon = altResult.FirstOrDefault(b => b.Type != BlockTypes.Chameleon);
                if (altNonChameleon == null)
                {
                    // Alternative side are all chameleons, adding it to combo
                    outResult.AddRange(altResult);
                }
                else
                {
                    if (outResult.All(b => b.CanCombo(altNonChameleon.Type)))
                    {
                        // All at alternative side can combo with first part, adding
                        outResult.AddRange(altResult);
                    }
                    else
                    {
                        // To sides make different combo, taking the longest one
                        if (outResult.Count - 1 < altResult.Count)
                        {
                            outResult = altResult;
                        }
                    }
                }
            }
            else
            {
                outResult.AddRange(altResult);
            }

            return outResult;
        }

        private List<Block> AddAllSameVertically(Field field, Block start)
        {
            List<Block> outResult = new List<Block>();
            outResult.Add(start);

            BlockTypes comboType = start.Type;
            for (int j = start.Y + 1; j < field.Blocks.GetLength(1); j++)
            {
                Block block = field.GetBlock(start.X, j);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType))
                    outResult.Add(block);
                else
                    break;
            }
            
            if (start.Type == BlockTypes.Chameleon)
                comboType = BlockTypes.Chameleon;
            List<Block> altResult = new List<Block>();

            for (int j = start.Y - 1; j >= 0; j--)
            {
                Block block = field.GetBlock(start.X, j);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType))
                    outResult.Add(block);
                else
                    break;
            }

            if (start.Type == BlockTypes.Chameleon)
            {
                // Middle block for combo was chameleon, we should look which side combo is longer
                Block altNonChameleon = altResult.FirstOrDefault(b => b.Type != BlockTypes.Chameleon);
                if (altNonChameleon == null)
                {
                    // Alternative side are all chameleons, adding it to combo
                    outResult.AddRange(altResult);
                }
                else
                {
                    if (outResult.All(b => b.CanCombo(altNonChameleon.Type)))
                    {
                        // All at alternative side can combo with first part, adding
                        outResult.AddRange(altResult);
                    }
                    else
                    {
                        // To sides make different combo, taking the longest one
                        if (outResult.Count - 1 < altResult.Count)
                        {
                            outResult = altResult;
                        }
                    }
                }
            }
            else
            {
                outResult.AddRange(altResult);
            }

            return outResult;
        }
    }
}
