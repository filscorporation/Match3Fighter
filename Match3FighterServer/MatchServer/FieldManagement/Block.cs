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

        public int X;

        public int Y;

        public Stack<BlockPreviousState> PreviousStates = new Stack<BlockPreviousState>();

        public Block ReplacedBlock;

        private const float AttackBlockChance = 0.36F;
        private const float HealBlockChance = 0.23F;
        private const float ManaBlockChance = 0.23F;
        private const float ArcaneBlockChance = 1F - AttackBlockChance - HealBlockChance - ManaBlockChance;

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
            if (n < AttackBlockChance)
                block.Type = BlockTypes.Attack;
            else if (n < AttackBlockChance + HealBlockChance)
                block.Type = BlockTypes.Health;
            else if (n < AttackBlockChance + HealBlockChance + ManaBlockChance)
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

        public BlockData ToData()
        {
            return new BlockData
            {
                X = X,
                Y = Y,
                ID = (int) Type,
                PreviousStates = PreviousStates.Select(s => s.ToData()).ToArray(),
                ReplacedBlock = ReplacedBlock?.ToData() ?? null,
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
