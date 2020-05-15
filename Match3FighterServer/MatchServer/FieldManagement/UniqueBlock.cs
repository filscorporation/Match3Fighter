using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Unique block that drops from 4-5-6 combos
    /// </summary>
    public abstract class UniqueBlock
    {
        public abstract string Name { get; }

        public abstract int Level { get; }

        public abstract BlockTypes BaseType { get; }

        public GlobalEffect AttachedGlobalEffect;

        /// <summary>
        /// Applies unique block effect on its creation
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="user"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual List<EffectData> OnCreate(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            return new List<EffectData>();
        }

        /// <summary>
        /// Applies unique block effect on its destruction
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="user"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual List<EffectData> OnDelete(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            return new List<EffectData>();
        }

        /// <summary>
        /// Applies effect from a combo to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        /// <param name="block"></param>
        public abstract List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block);

        protected EffectData HealthData(Player player, float value)
        {
            EffectData hData = new EffectData();
            hData.EffectType = EffectType.HealthChanged;
            hData.Data = new Dictionary<string, object>();
            hData.Data["Target"] = player.InGameID;
            hData.Data["Value"] = value;
            return hData;
        }

        protected EffectData ManaData(Player player, float value)
        {
            EffectData mData = new EffectData();
            mData.EffectType = EffectType.ManaChanged;
            mData.Data = new Dictionary<string, object>();
            mData.Data["Target"] = player.InGameID;
            mData.Data["Value"] = value;
            return mData;
        }

        protected EffectData GlobalEffectData(Player player, GlobalEffect effect)
        {
            EffectData geData = new EffectData();
            geData.EffectType = EffectType.GlobalEffect;
            geData.Data = new Dictionary<string, object>();
            geData.Data["Target"] = player.InGameID;
            geData.Data["Type"] = effect.Type;
            geData.Data["Value"] = effect.Value;
            geData.Data["Created"] = true;
            return geData;
        }

        protected EffectData GlobalEffectRemovedData(Player player, GlobalEffect effect)
        {
            EffectData geData = new EffectData();
            geData.EffectType = EffectType.GlobalEffect;
            geData.Data = new Dictionary<string, object>();
            geData.Data["Target"] = player.InGameID;
            geData.Data["Type"] = effect.Type;
            geData.Data["Value"] = effect.Value;
            geData.Data["Created"] = false;
            return geData;
        }

        protected EffectData UniqueShotData(Field fromField, Field toField, Block fromBlock, Block toBlock, string name)
        {
            EffectData sData = new EffectData();
            sData.EffectType = EffectType.UniqueEffect;
            sData.Data = new Dictionary<string, object>();
            sData.Data["InitX"] = fromBlock.X;
            sData.Data["InitY"] = fromBlock.Y;
            sData.Data["InitField"] = fromField.InGameID;
            sData.Data["TargetX"] = toBlock.X;
            sData.Data["TargetY"] = toBlock.Y;
            sData.Data["TargetField"] = toField.InGameID;
            sData.Data["Value"] = name;
            return sData;
        }

        public UniqueBlockData ToData()
        {
            return new UniqueBlockData
            {
                Name = Name,
                Level = Level,
                Type = BaseType,
            };
        }
    }
}
