using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using NetworkShared.Data.Effects;
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
                    field.Blocks[i, j].ProcessedHorizontally = false;
                    field.Blocks[i, j].ProcessedVertically = false;
                }
            }
        }

        /// <summary>
        /// Clears destroyed blocks list and calls their on destroy methods
        /// </summary>
        /// <param name="field"></param>
        /// <param name="match"></param>
        /// <param name="user"></param>
        public List<EffectData> ClearDestroyedBlocks(Field field, GameMatch match, Player user)
        {
            List<EffectData> data = new List<EffectData>();

            foreach (Block block in field.DestroyedBlocks)
            {
                if (block.UniqueBlock != null)
                    data.AddRange(block.UniqueBlock.OnDelete(this, random, match, user, block));
            }

            field.DestroyedBlocks.Clear();

            return data;
        }

        /// <summary>
        /// Removes expired effects from field and blocks
        /// </summary>
        /// <param name="field"></param>
        public void RefreshDurationEffects(Field field)
        {
            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    field.Blocks[i, j].RemoveExpiredEffects();
                }
            }
        }

        /// <summary>
        /// Updates global effects with over time value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="player"></param>
        public void RefreshGlobalEffects(Field field, Player player)
        {
            foreach (GlobalEffect globalEffect in field.GlobalEffects)
            {
                switch (globalEffect.Type)
                {
                    case GlobalEffectType.Shield:
                        break;
                    case GlobalEffectType.HealOverTime:
                        player.GainHealth(globalEffect.UpdateValue());
                        break;
                    case GlobalEffectType.ManaOverTime:
                        player.GainMana(globalEffect.UpdateValue());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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

            if (blockA.OnBlockEffects.Any(e => e.Type == OnBlockEffectType.Frozen)
                || blockB.OnBlockEffects.Any(e => e.Type == OnBlockEffectType.Frozen))
                return false;

            // Remove lock from swapped
            if (blockA.IsLocked || blockB.IsLocked)
            {
                ClearLockedBlocks(field);
            }

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
        /// Processes block lock
        /// </summary>
        /// <param name="field"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool TryLockBlock(Field field, int x, int y)
        {
            Block block = field.GetBlock(x, y);
            bool preferHor = false;
            bool preferVer = false;

            if (field.LockedBlocks.Any())
            {
                if (block.IsLocked && field.LockedBlocks.Count > 1)
                {
                    // Rotate locked selection
                    if (field.LockedBlocks[0].X == field.LockedBlocks[1].X)
                    {
                        preferHor = true;
                    }
                    else
                    {
                        preferVer = true;
                    }
                }

                ClearLockedBlocks(field);
            }

            List<Block> hor = AddAllSameHorizontally(field, block);
            List<Block> ver = AddAllSameVertically(field, block);
            List<Block> toLock;
            if (preferHor && hor.Count > 1)
                toLock = hor;
            else if (preferVer && ver.Count > 1)
                toLock = ver;
            else
                toLock = hor.Count >= ver.Count ? hor : ver;
            foreach (Block blockToLock in toLock)
            {
                blockToLock.IsLocked = true;
            }
            field.LockedBlocks.AddRange(toLock);

            return true;
        }

        private void ClearLockedBlocks(Field field)
        {
            foreach (Block lockedBlock in field.LockedBlocks)
            {
                lockedBlock.IsLocked = false;
            }
            field.LockedBlocks.Clear();
        }

        /// <summary>
        /// Returns random blocks on a field that are not in destroyed state
        /// </summary>
        /// <param name="field"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Block> GetRandomNonDestroyedBlocks(Field field, int count = 1)
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
                yield break;

            if (totalNonDestroyed < count)
                count = totalNonDestroyed;

            List<int> randoms = UniqueRandom(totalNonDestroyed, count).OrderBy(i => i).ToList();
            int index = 0;

            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    if (!field.Blocks[i, j].IsLastDestroyedState())
                    {
                        if (index == randoms.First())
                        {
                            randoms.RemoveAt(0);
                            yield return field.Blocks[i, j];
                            if (!randoms.Any())
                                yield break;
                        }
                        index++;
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Returns count size list of random unique integers less than max
        /// </summary>
        /// <param name="max"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<int> UniqueRandom(int max, int count)
        {
            int[] list = new int[max];

            for (int i = 0; i < max; i++)
            {
                list[i] = i;
            }

            return list.Shuffle().Take(count);
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
        /// Returns list of possible combos, that includes any of passed blocks
        /// </summary>
        /// <param name="field"></param>
        /// <param name="includeAny"></param>
        public List<Combo> CheckForCombos(Field field, List<Block> includeAny)
        {
            List<Combo> combos = new List<Combo>();

            foreach (Block block in includeAny)
            {
                List<Block> ver = AddAllSameVertically(field, block, true);
                List<Block> hor = AddAllSameHorizontally(field, block, true);

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
                {
                    block.RememberState(destroyedState);
                    field.DestroyedBlocks.Add(block);
                    if (block.IsLocked)
                        ClearLockedBlocks(field);
                }
            }
        }

        /// <summary>
        /// Put all blocks into flipped over state
        /// </summary>
        /// <param name="field"></param>
        /// <param name="blocks"></param>
        public void FlipBlocks(Field field, IEnumerable<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                block.RememberState(BlockState.FlippedOver);
                if (block.IsLocked)
                    ClearLockedBlocks(field);
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

                    if (block.IsLastFlippedOverState())
                    {
                        Block newBlock = Block.GetRandomBlock(random);
                        newBlock.ReplacedBlock = field.Blocks[i, j];
                        field.Blocks[i, j] = newBlock;
                        field.Blocks[i, j].SetXY(i, j);
                        field.Blocks[i, j].RememberState(BlockState.CreatedFromFlip);
                    }

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

                            if (block.IsLocked)
                                ClearLockedBlocks(field);
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

        /// <summary>
        /// Returns list of all possible swaps to make
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerable<Swap> GetAllPossibleSwaps(Field field)
        {
            int w = field.Blocks.GetLength(0);
            int h = field.Blocks.GetLength(1);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Block block = field.GetBlock(i, j);
                    if (!block.ProcessedHorizontally)
                    {
                        List<Block> hor = AddAllSameHorizontally(field, block, true);
                        foreach (Block horBlock in hor)
                        {
                            horBlock.ProcessedHorizontally = true;
                        }
                        if (hor.Count >= MinComboCount - 1)
                        {
                            BlockTypes type = hor.FirstOrDefault(b => b.Type != BlockTypes.Chameleon)?.Type ?? BlockTypes.Chameleon;
                            int lx = hor.Min(b => b.X);
                            int rx = hor.Max(b => b.X);
                            int y = hor[0].Y;
                            if (field.GetBlock(lx - 1, y - 1)?.CanCombo(type) ?? false)
                                yield return new Swap(lx - 1, y - 1, 0); // Up
                            if (field.GetBlock(lx - 1, y + 1)?.CanCombo(type) ?? false)
                                yield return new Swap(lx - 1, y + 1, 2); // Down
                            if (field.GetBlock(rx + 1, y - 1)?.CanCombo(type) ?? false)
                                yield return new Swap(rx + 1, y - 1, 0); // Up
                            if (field.GetBlock(rx + 1, y + 1)?.CanCombo(type) ?? false)
                                yield return new Swap(rx + 1, y + 1, 2); // Down
                            if (field.GetBlock(lx - 2, y)?.CanCombo(type) ?? false)
                                yield return new Swap(lx - 2, y, 1); // Right
                            if (field.GetBlock(rx + 2, y)?.CanCombo(type) ?? false)
                                yield return new Swap(rx + 2, y, 3); // Left
                        }
                    }
                    if (!block.ProcessedVertically)
                    {
                        List<Block> ver = AddAllSameVertically(field, block, true);
                        foreach (Block verBlock in ver)
                        {
                            verBlock.ProcessedVertically = true;
                        }
                        if (ver.Count >= MinComboCount - 1)
                        {
                            BlockTypes type = ver.FirstOrDefault(b => b.Type != BlockTypes.Chameleon)?.Type ?? BlockTypes.Chameleon;
                            int dy = ver.Min(b => b.Y);
                            int uy = ver.Max(b => b.Y);
                            int x = ver[0].X;
                            if (field.GetBlock(x - 1, dy - 1)?.CanCombo(type) ?? false)
                                yield return new Swap(x - 1, dy - 1, 1); // Right
                            if (field.GetBlock(x + 1, dy - 1)?.CanCombo(type) ?? false)
                                yield return new Swap(x + 1, dy - 1, 3); // Left
                            if (field.GetBlock(x - 1, uy + 1)?.CanCombo(type) ?? false)
                                yield return new Swap(x - 1, uy + 1, 1); // Right
                            if (field.GetBlock(x + 1, uy + 1)?.CanCombo(type) ?? false)
                                yield return new Swap(x + 1, uy + 1, 3); // Left
                            if (field.GetBlock(x, dy - 2)?.CanCombo(type) ?? false)
                                yield return new Swap(x, dy - 2, 0); // Up
                            if (field.GetBlock(x, uy + 2)?.CanCombo(type) ?? false)
                                yield return new Swap(x, uy + 2, 2); // Down
                        }
                    }
                }
            }
        }

        // TODO: merge next two methods
        private List<Block> AddAllSameHorizontally(Field field, Block start, bool excludeLocked = false)
        {
            List<Block> outResult = new List<Block>();
            outResult.Add(start);

            BlockTypes comboType = start.Type;
            for (int i = start.X + 1; i < field.Blocks.GetLength(0); i++)
            {
                Block block = field.GetBlock(i, start.Y);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType) && !(excludeLocked && block.IsLocked))
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
                if (block != null && block.CanCombo(comboType) && !(excludeLocked && block.IsLocked))
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

        private List<Block> AddAllSameVertically(Field field, Block start, bool excludeLocked = false)
        {
            List<Block> outResult = new List<Block>();
            outResult.Add(start);

            BlockTypes comboType = start.Type;
            for (int j = start.Y + 1; j < field.Blocks.GetLength(1); j++)
            {
                Block block = field.GetBlock(start.X, j);
                if (comboType == BlockTypes.Chameleon)
                    comboType = block.Type;
                if (block != null && block.CanCombo(comboType) && !(excludeLocked && block.IsLocked))
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
                if (block != null && block.CanCombo(comboType) && !(excludeLocked && block.IsLocked))
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
    }
}
