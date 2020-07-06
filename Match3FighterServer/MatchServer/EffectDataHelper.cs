using System.Collections.Generic;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer
{
    /// <summary>
    /// Creates EffectData objects from parameters
    /// </summary>
    public static class EffectDataHelper
    {
        public static EffectData HealthData(Player player, float value)
        {
            EffectData hData = new EffectData();
            hData.EffectType = EffectType.HealthChanged;
            hData.Data = new Dictionary<string, object>();
            hData.Data["Target"] = player.InGameID;
            hData.Data["Value"] = value;
            return hData;
        }

        public static EffectData ManaData(Player player, float value)
        {
            EffectData mData = new EffectData();
            mData.EffectType = EffectType.ManaChanged;
            mData.Data = new Dictionary<string, object>();
            mData.Data["Target"] = player.InGameID;
            mData.Data["Value"] = value;
            return mData;
        }

        public static EffectData GlobalEffectData(Player player, GlobalEffect effect)
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

        public static EffectData GlobalEffectRemovedData(Player player, GlobalEffect effect)
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

        public static EffectData GlobalEffectRemovedData(Player player, GlobalEffectType type)
        {
            EffectData geData = new EffectData();
            geData.EffectType = EffectType.GlobalEffect;
            geData.Data = new Dictionary<string, object>();
            geData.Data["Target"] = player.InGameID;
            geData.Data["Type"] = type;
            geData.Data["Created"] = false;
            return geData;
        }

        public static EffectData UniqueShotData(Field fromField, Field toField, Block fromBlock, Block toBlock, string name)
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

        public static EffectData ShotData(Field fromField, Field toField, Block fromBlock, Block toBlock, float dmg)
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
