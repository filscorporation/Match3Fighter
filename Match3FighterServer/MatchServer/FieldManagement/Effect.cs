using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Some in game effect after event - deal damage, add health, add mana..
    /// </summary>
    public abstract class Effect
    {
        public abstract BlockTypes ComboEffectType { get; }

        /// <summary>
        /// Applies effect from a combo to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="upgradeManager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        public abstract List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo);

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

        protected EffectData GlobalEffectRemovedData(Player player, GlobalEffect effect)
        {
            EffectData geData = new EffectData();
            geData.EffectType = EffectType.GlobalEffect;
            geData.Data = new Dictionary<string, object>();
            geData.Data["Target"] = player.InGameID;
            geData.Data["Type"] = effect.Type;
            geData.Data["Created"] = false;
            return geData;
        }

        protected EffectData ShotData(Field fromField, Field toField, Block fromBlock, Block toBlock, float dmg)
        {
            EffectData sData = new EffectData();
            sData.EffectType = EffectType.BlockShot;
            sData.Data = new Dictionary<string, object>();
            sData.Data["InitX"] = fromBlock.X;
            sData.Data["InitY"] = fromBlock.Y;
            sData.Data["InitField"] = fromField.InGameID;
            sData.Data["TargetX"] = toBlock.X;
            sData.Data["TargetY"] = toBlock.Y;
            sData.Data["TargetField"] = toField.InGameID;
            sData.Data["Value"] = -dmg;
            return sData;
        }
    }
}
