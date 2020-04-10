using System;
using System.Collections.Generic;
using System.Linq;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Manages effects from block combos and etc
    /// </summary>
    public class BlockEffectsManager
    {
        /// <summary>
        /// Returns effect from combo
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public Effect GetEffectFromCombo(List<Block> combo)
        {
            Effect effect = new Effect();

            BlockTypes comboType = combo.FirstOrDefault(b => b.Type != BlockTypes.Chameleon)?.Type ?? BlockTypes.Chameleon;
            effect.ComboEffectType = comboType;
            // TODO:
            switch (comboType)
            {
                case BlockTypes.Attack:
                    break;
                case BlockTypes.Mana:
                    break;
                case BlockTypes.Health:
                    break;
                case BlockTypes.Arcane:
                    break;
                case BlockTypes.Gold:
                    break;
                case BlockTypes.Chameleon:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return effect;
        }
    }
}
