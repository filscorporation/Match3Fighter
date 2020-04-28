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
                    field.Blocks[i, j].SetXY(i, j);
                }
            }

            return field;
        }

        /// <summary>
        /// Sets default state to all blocks
        /// </summary>
        /// <param name="field"></param>
        public void SetDefaultState(Field field)
        {
            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    field.Blocks[i, j].ReplacedBlock = null;
                    field.Blocks[i, j].PreviousStates.Clear();
                }
            }
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
            blockB.RememberState(BlockState.Swapped);
            blockB.SetXY(swap.X, swap.Y);
            field.Blocks[nx, ny] = blockA;
            blockA.RememberState(BlockState.Swapped);
            blockA.SetXY(nx, ny);

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
                    if (!field.Blocks[i, j].IsLastDestroyedState())
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
                    if (!field.Blocks[i, j].IsLastDestroyedState())
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
        /// Returns random block on a field that is not in destroyed state and not at field borders
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public Block GetRandomNonDestroyedBlockExceptBorders(Field field)
        {
            int totalNonDestroyed = 0;
            for (int i = 1; i < fieldWidth - 1; i++)
            {
                for (int j = 1; j < fieldHeight - 1; j++)
                {
                    if (!field.Blocks[i, j].IsLastDestroyedState())
                        totalNonDestroyed++;
                }
            }

            if (totalNonDestroyed == 0)
                return null;

            int randomIndex = random.Next(0, totalNonDestroyed);

            for (int i = 1; i < fieldWidth - 1; i++)
            {
                for (int j = 1; j < fieldHeight - 1; j++)
                {
                    if (!field.Blocks[i, j].IsLastDestroyedState())
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
        /// Returns blocks next to target one
        /// </summary>
        /// <param name="field"></param>
        /// <param name="block"></param>
        /// <param name="cross">Include diagonal</param>
        /// <param name="range"></param>
        /// <returns></returns>
        public IEnumerable<Block> GetNeighbours(Field field, Block block, bool cross = false, int range = 1)
        {
            for (int i = -range; i <= range; i++)
            {
                for (int j = -range; j <= range; j++)
                {
                    if (i != 0 && j != 0 && !cross)
                        continue;

                    if (block.X + i >= 0 && block.X + i < field.Blocks.GetLength(0) &&
                        block.Y + j >= 0 && block.Y + j < field.Blocks.GetLength(1))
                        yield return field.Blocks[block.X + i, block.Y + j];
                }
            }
        }

        /// <summary>
        /// Returns list of possile combos, that includes any of passed blocks
        /// </summary>
        /// <param name="field"></param>
        /// <param name="includeAny"></param>
        public List<Combo> CheckForCombos(Field field, List<Block> includeAny)
        {
            List<Combo> combos = new List<Combo>();

            foreach (Block block in includeAny)
            {
                List<Block> ver = AddAllSameVertically(field, block);
                List<Block> hor = AddAllSameHorizontally(field, block);

                if (Math.Max(ver.Count, hor.Count) >= MinComboCount)
                {
                    combos.Add(new Combo(ver.Count > hor.Count ? ver : hor));
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
        public void DestroyBlocks(Field field, IEnumerable<Block> blocks, BlockState destroyedState)
        {
            foreach (Block block in blocks)
            {
                if (!block.IsLastDestroyedState())
                    block.RememberState(destroyedState);
            }
        }

        /// <summary>
        /// Creates new block at position of random block in range
        /// </summary>
        /// <param name="field"></param>
        /// <param name="uniqueBlock"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Block CreateBlockInRange(Field field, UniqueBlock uniqueBlock, List<Block> range)
        {
            Block block = new Block();
            block.Type = uniqueBlock.BaseType;
            block.UniqueBlock = uniqueBlock;

            int n = random.Next(0, range.Count);
            Block blockToReplace = range[n];
            block.SetXY(blockToReplace.X, blockToReplace.Y);
            block.RememberState(BlockState.CreatedAsComboResult);
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
                    if (block.IsLastDestroyedState())
                    {
                        offset++;
                    }
                    else
                    {
                        if (offset > 0)
                        {
                            field.Blocks[i, j] = null;
                            if (block.ReplacedBlock != null)
                            {
                                // If this block already replaced some - put replaced on this old 
                                field.Blocks[i, j] = block.ReplacedBlock;
                            }
                            block.ReplacedBlock = field.Blocks[i, j - offset];
                            field.Blocks[i, j - offset] = block;
                            block.RememberState(BlockState.Moved);
                            block.SetXY(i, j - offset);
                        }
                    }
                }

                for (int j = h - 1; j >= 0; j--)
                {
                    if (offset <= 0)
                        break;
                    Block newBlock = Block.GetRandomBlock(random);
                    newBlock.ReplacedBlock = field.Blocks[i, j];
                    field.Blocks[i, j] = newBlock;
                    field.Blocks[i, j].SetXY(i, j);
                    field.Blocks[i, j].RememberState(BlockState.DroppedAsNew);
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
