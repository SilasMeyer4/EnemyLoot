using BepInEx.Configuration;
using EnemyLoot.Netcode;
using GameNetcodeStuff;
using HarmonyLib;
using LethalLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

namespace SilasMeyer_EnemyLoot
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {

        internal ConfigEntry<bool> areItemsInShop;

        internal ConfigEntry<bool> SpiderDropSpiderEgg;
        internal ConfigEntry<bool> MaskedDropMask;
        //internal ConfigEntry<bool> MaskedDropBody;
        internal ConfigEntry<bool> HoarderDropGuiltyGear;
        internal ConfigEntry<bool> BrackenDropBlackOrb;
        internal ConfigEntry<bool> SnareFleaDropWhiteOrb;
        internal ConfigEntry<bool> ThumperDropOrangeOrb;
        internal ConfigEntry<int> GuiltyGearSpawnRate;

        public Config(ConfigFile cfg)
        {
            InitInstance(this);

            areItemsInShop = cfg.Bind("General", "Activate Items in shop", true, "Adds the new items to the shop");

            SpiderDropSpiderEgg = cfg.Bind("General", "Drop Spider Egg", true, "Spider drops egg on death");
            MaskedDropMask = cfg.Bind("General", "Drop Mask", true, "Masked drops mask on death");
            //  this.MaskedDropBody = base.Config.Bind<bool>("General", "Drop Body(not added yet)", true, "Masked drops body on death");
            HoarderDropGuiltyGear = cfg.Bind("General", "Drop Guilty Gear Case", true, "Hoarder Bug can drop Guilty Gear Strive Case");
            BrackenDropBlackOrb = cfg.Bind("General", "Drop Black Orb", true, "Braken can drop Black Orb");
            SnareFleaDropWhiteOrb = cfg.Bind("General", "Drop White Orb", true, "Snare Flea can drop White Orb");
            ThumperDropOrangeOrb = cfg.Bind("General", "Drop Orange Orb", true, "Thumper can drop Orange Orb");
            GuiltyGearSpawnRate = cfg.Bind("General", "Guilty Gear Spawnrate", 60, "Spawnrate in percent of the Guilty Gear drop when killing a Hoarder Bug. Enter a number from 0-100.");
        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            FastBufferWriter stream = new FastBufferWriter(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("ModName_OnRequestConfigSync", 0uL, stream);
            stream.Dispose();
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            Plugin.logger.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            FastBufferWriter stream = new FastBufferWriter(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("ModName_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                Plugin.logger.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }

            stream.Dispose();
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                Plugin.logger.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                Plugin.logger.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            Plugin.logger.LogInfo("Successfully synced config with host.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            if (IsHost)
            {
                MessageManager.RegisterNamedMessageHandler("EnemyLoot_OnRequestConfigSync", OnRequestSync);
                Synced = true;

                return;
            }

            Synced = false;
            MessageManager.RegisterNamedMessageHandler("EnemyLoot_OnReceiveConfigSync", OnReceiveSync);
            RequestSync();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            Config.RevertSync();
        }

    }
}

