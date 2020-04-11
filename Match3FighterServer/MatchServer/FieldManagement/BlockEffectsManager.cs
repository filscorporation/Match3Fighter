using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatchServer.Players;
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

        private readonly Random random;

        private Dictionary<BlockTypes, Effect> effects;

        public BlockEffectsManager(FieldManager fieldManager)
        {
            this.fieldManager = fieldManager;

            random = new Random();

            InitializeEffects();
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

        /// <summary>
        /// Returns effect from combo
        /// </summary>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        /// <returns></returns>
        public EffectData ApplyEffectsFromCombo(GameMatch match, int playerUserIndex, List<Block> combo)
        {
            BlockTypes comboType = combo.FirstOrDefault(b => b.Type != BlockTypes.Chameleon)?.Type ?? BlockTypes.Chameleon;
            Effect effect = effects[comboType];
            EffectData data = effect.Apply(fieldManager, random, match, playerUserIndex, combo);
            
            return data;
        }
    }
}
