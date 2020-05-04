using System;
using System.Collections.Generic;
using System.Linq;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// One element of the field
    /// </summary>
    public class Block
    {
        public BlockTypes Type;

        public UniqueBlock UniqueBlock;

        public int X;

        public int Y;

        public Stack<BlockPreviousState> PreviousStates = new Stack<BlockPreviousState>();

        public Block ReplacedBlock;

        public List<OnBlockEffect> OnBlockEffects = new List<OnBlockEffect>();

        private const float attackBlockChance = 0.29F;
        private const float healBlockChance = 0.26F;
        private const float manaBlockChance = 0.26F;
        private const float arcaneBlockChance = 1F - attackBlockChance - healBlockChance - manaBlockChance;

        /// <summary>
        /// Returns true if block is unique
        /// </summary>
        public bool IsUnique => UniqueBlock != null;

        /// <summary>
        /// Sets block coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetXY(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Saves previous state
        /// </summary>
        /// <param name="newState"></param>
        public void RememberState(BlockState newState)
        {
            PreviousStates.Push(new BlockPreviousState(newState, X, Y));
        }

        /// <summary>
        /// Returns randomly generated block
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Block GetRandomBlock(Random random)
        {
            Block block = new Block();

            double n = random.NextDouble();
            if (n < attackBlockChance)
                block.Type = BlockTypes.Attack;
            else if (n < attackBlockChance + healBlockChance)
                block.Type = BlockTypes.Health;
            else if (n < attackBlockChance + healBlockChance + manaBlockChance)
                block.Type = BlockTypes.Mana;
            else
                block.Type = BlockTypes.Arcane;

            return block;
        }

        /// <summary>
        /// Return true if blocks can make combo
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanCombo(BlockTypes type)
        {
            if (type == BlockTypes.Chameleon || Type == BlockTypes.Chameleon)
                return true;

            return type == Type;
        }

        /// <summary>
        /// Returns if block state is any of destroyed
        /// </summary>
        /// <returns></returns>
        public bool IsLastDestroyedState()
        {
            if (PreviousStates.Any())
            {
                BlockState state = PreviousStates.Peek().State;
                return state == BlockState.DestroyedAsCombo || state == BlockState.DestroyedByDamage;
            }

            return false;
        }

        /// <summary>
        /// Returns if block state is flipped over
        /// </summary>
        /// <returns></returns>
        public bool IsLastFlippedOverState()
        {
            if (PreviousStates.Any())
            {
                BlockState state = PreviousStates.Peek().State;
                return state == BlockState.FlippedOver;
            }

            return false;
        }

        public void RemoveExpiredEffects()
        {
            OnBlockEffects.RemoveAll(e => 
                (DateTime.UtcNow - e.StartTime).TotalMilliseconds/1000F > e.Duration);
        }

        public BlockData ToData()
        {
            return new BlockData
            {
                X = X,
                Y = Y,
                ID = (int) Type,
                UniqueBlock = UniqueBlock?.Name,
                PreviousStates = PreviousStates.Select(s => s.ToData()).ToArray(),
                ReplacedBlock = ReplacedBlock?.ToData() ?? null,
                OnBlockEffects = OnBlockEffects.Select(e => e.ToData()).ToArray(),
            };
        }
    }

    public class BlockPreviousState
    {
        public BlockState State;

        public int X;

        public int Y;

        public BlockPreviousState(BlockState state, int x, int y)
        {
            State = state;
            X = x;
            Y = y;
        }

        public BlockStateData ToData()
        {
            return new BlockStateData
            {
                State = State,
                X = X,
                Y = Y,
            };
        }
    }
}
