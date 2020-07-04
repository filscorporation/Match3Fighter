using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.FieldManagement;
using NetworkShared.Data;

namespace MatchServer.Players
{
    /// <summary>
    /// AI logic for practicing
    /// </summary>
    public class AIManager
    {
        private FieldManager fieldManager;

        private readonly List<Bot> bots = new List<Bot>();

        public AIManager(FieldManager fieldManager)
        {
            this.fieldManager = fieldManager;
        }

        /// <summary>
        /// Returns new bot player instance
        /// </summary>
        /// <returns></returns>
        public Bot GetActualBot()
        {
            Bot bot = new Bot();
            bot.SetDefaultUniqueBlocks();
            bot.PlayerID = Guid.NewGuid().ToString();
            bot.Name = "Bot";

            bots.Add(bot);

            return bot;
        }

        /// <summary>
        /// Calculates tick of AI logic
        /// </summary>
        public void UpdateBots()
        {
            List<Bot> toRemove = new List<Bot>();
            foreach (Bot bot in bots)
            {
                if (bot.CurrentMatch == null)
                {
                    toRemove.Add(bot);
                    continue;
                }

                DoBotAction(bot);
            }

            foreach (Bot toRemoveBot in toRemove)
            {
                bots.Remove(toRemoveBot);
            }
        }

        private void DoBotAction(Bot bot)
        {
            if (!bot.CanAct)
                return;

            bot.UpdateAct();
            Field field = bot.InGameID == 1 ? bot.CurrentMatch.Field1 : bot.CurrentMatch.Field2;

            Swap swap = fieldManager.GetAllPossibleSwaps(field).FirstOrDefault();
            if (swap.Equals(default(Swap)))
            {
                // No possible turns
                return;
            }

            BlockSwapRequest request = new BlockSwapRequest
            {
                X = swap.X,
                Y = swap.Y,
                Direction = swap.Direction,
            };

            GameCore.Instance.ProcessBlockSwap(bot, request);
        }
    }
}
