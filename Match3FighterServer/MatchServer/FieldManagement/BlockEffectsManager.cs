using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Manages effects from block combos and etc
    /// </summary>
    public class BlockEffectsManager
    {
        private readonly FieldManager fieldManager;
        private readonly UpgradeManager upgradeManager;

        private readonly Random random;

        private Dictionary<BlockTypes, Effect> effects;

        public static readonly Dictionary<string, UniqueBlock> UniqueBlocks = new Dictionary<string, UniqueBlock>();

        public BlockEffectsManager(FieldManager fieldManager, UpgradeManager upgradeManager)
        {
            this.fieldManager = fieldManager;
            this.upgradeManager = upgradeManager;

            random = new Random();

            InitializeEffects();
            InitializeUniqueBlocks();
        }

        private void InitializeEffects()
        {
            effects = new Dictionary<BlockTypes, Effect>();
            foreach (Type type in Assembly.GetAssembly(typeof(Effect)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Effect))))
            {
                Effect effect = (Effect)Activator.CreateInstance(type);
                effects[effect.ComboEffectType] = effect;
            }
        }

        private void InitializeUniqueBlocks()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(UniqueBlock)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(UniqueBlock))))
            {
                UniqueBlock block = (UniqueBlock)Activator.CreateInstance(type);
                UniqueBlocks[block.Name] = block;
            }
        }

        /// <summary>
        /// Returns effect from combo
        /// </summary>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        /// <returns></returns>
        public List<EffectData> ApplyEffectsFromCombo(GameMatch match, int playerUserIndex, Combo combo)
        {
            List<EffectData> data = new List<EffectData>();
            foreach (Block uniqueBlock in combo.Blocks.Where(b => b.IsUnique))
            {
                data.AddRange(uniqueBlock.UniqueBlock.Apply(
                    fieldManager, random, match, playerUserIndex, combo, uniqueBlock));
            }

            Effect effect = effects[combo.Type];
            data.AddRange(effect.Apply(fieldManager, upgradeManager, random, match, playerUserIndex, combo));
            
            return data;
        }
    }
}
