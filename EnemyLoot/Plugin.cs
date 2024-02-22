using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using UnityEngine;
using LethalLib.Modules;
using EnemyLoot.Patches;
using EnemyLoot.Behaviours;
using System.Runtime.CompilerServices;

namespace EnemyLoot_SilasMeyer
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class EnemyLoot : BaseUnityPlugin
    {
        public const string modGUID = "SilasMeyer.EnemyLoot";
        public const string modName = "EnemyLoot";
        public const string modVersion = "0.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static EnemyLoot Instance;

        public static Item spiderEgg;
        public static Item guiltyGearCase;
        public static Item blackOrb;
        public static Item whiteOrb;
        internal static AudioClip guiltyGearSFX;
        internal static AudioClip blackOrbSpawnSFX;
        internal static AudioClip blackOrbTeleportSFX;
        internal static AudioClip blackOrbCDSFX;
        internal static AudioClip whiteOrbHealingSFX;

        internal ManualLogSource mls;



        internal ConfigEntry<bool> SpiderDropSpiderEgg;
        internal ConfigEntry<bool> MaskedDropMask;
        //internal ConfigEntry<bool> MaskedDropBody;
        internal ConfigEntry<bool> HoarderDropGuiltyGear;
        internal ConfigEntry<bool> BrackenDropBlackOrb;
        internal ConfigEntry<bool> SnareFleaDropWhiteOrb;


        void Awake()

        {
            if (Instance == null)
            {
                Instance = this;
            }

            //Config File

            this.SpiderDropSpiderEgg = base.Config.Bind<bool>("General", "Drop Spider Egg", true, "Spider drops egg on death");
            this.MaskedDropMask = base.Config.Bind<bool>("General", "Drop Mask", true, "Masked drops mask on death");
            //  this.MaskedDropBody = base.Config.Bind<bool>("General", "Drop Body(not added yet)", true, "Masked drops body on death");
            this.HoarderDropGuiltyGear = base.Config.Bind<bool>("General", "Drop Guilty Gear Case", true, "Hoarder Bug can drop Guilty Gear Strive Case");
            this.BrackenDropBlackOrb = base.Config.Bind<bool>("General", "Drop Black Orb", true, "Braken can drop Black Orb");
            this.SnareFleaDropWhiteOrb = base.Config.Bind<bool>("General", "Drop White Orb", true, "Snare Flea can drop White Orb");

            //Loads Assets

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemyloot");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            //Adds the spiderEgg

            spiderEgg = bundle.LoadAsset<Item>("Assets/Items/SpiderEggItem.asset");
            spiderEgg.positionOffset = new Vector3(-0.2f, 0, 0.3f);
            spiderEgg.rotationOffset = new Vector3(-90, 0, -10);
            NetworkPrefabs.RegisterNetworkPrefab(spiderEgg.spawnPrefab);
            Utilities.FixMixerGroups(spiderEgg.spawnPrefab);
            Items.RegisterScrap(spiderEgg, 0, Levels.LevelTypes.All);

            //Adds Guilty Gear Strive Case

            guiltyGearCase = bundle.LoadAsset<Item>("Assets/Items/GuiltyGearCaseItem.asset");
            EnemyLoot.guiltyGearSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/GuiltyGearTownInsideMe.mp3");
            GuiltyGearCaseBehaviour scriptGG = guiltyGearCase.spawnPrefab.AddComponent<GuiltyGearCaseBehaviour>();
            scriptGG.grabbable = true;
            scriptGG.grabbableToEnemies = true;
            scriptGG.itemProperties = guiltyGearCase;
            guiltyGearCase.rotationOffset = new Vector3(0, 90, 0);
            guiltyGearCase.positionOffset = new Vector3(0.1f, 0, -0.08f);
            NetworkPrefabs.RegisterNetworkPrefab(guiltyGearCase.spawnPrefab);
            Utilities.FixMixerGroups(guiltyGearCase.spawnPrefab);
            Items.RegisterScrap(guiltyGearCase, 3, Levels.LevelTypes.All);

            TerminalNode node1 = ScriptableObject.CreateInstance<TerminalNode>();
            node1.clearPreviousText = true;
            node1.displayText = "Info test zu Case";
            Items.RegisterShopItem(guiltyGearCase, null, null, node1, 0);

            //Adds Black Orb

            blackOrb = bundle.LoadAsset<Item>("Assets/Items/BlackOrbItem.asset");
            EnemyLoot.blackOrbSpawnSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbSpawnSFX.mp3");
            EnemyLoot.blackOrbTeleportSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbTeleportSFX.mp3");
            EnemyLoot.blackOrbCDSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbCDSFX.mp3");
            BlackOrbBehaviour scriptBO = blackOrb.spawnPrefab.AddComponent<BlackOrbBehaviour>();
            scriptBO.grabbable = true;
            scriptBO.grabbableToEnemies = true;
            scriptBO.itemProperties = blackOrb;
            blackOrb.rotationOffset = new Vector3(0, 0, 0);
            blackOrb.positionOffset = new Vector3(-0.04f, 0.02f, -0.02f);
            NetworkPrefabs.RegisterNetworkPrefab(blackOrb.spawnPrefab);
            Utilities.FixMixerGroups(blackOrb.spawnPrefab);
            Items.RegisterScrap(blackOrb, 0, Levels.LevelTypes.All);

            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Info test zu orb";
            Items.RegisterShopItem(blackOrb, null, null, node, 0);

            //Adds White Orb

            whiteOrb= bundle.LoadAsset<Item>("Assets/Items/WhiteOrbItem.asset");
            EnemyLoot.whiteOrbSpawnSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbSpawnSFX.mp3");
            EnemyLoot.whiteOrbTeleportSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbTeleportSFX.mp3");
            EnemyLoot.whiteOrbCDSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/BlackOrbCDSFX.mp3");
            WhiteOrbBehaviour scriptWO = whiteOrb.spawnPrefab.AddComponent<WhiteOrbBehaviour>();
            scriptWO.grabbable = true;
            scriptWO.grabbableToEnemies = true;
            scriptWO.itemProperties = whiteOrb;
            whiteOrb.rotationOffset = new Vector3(0, 0, 0);
            whiteOrb.positionOffset = new Vector3(-0.04f, 0.02f, -0.02f);
            NetworkPrefabs.RegisterNetworkPrefab(whiteOrb.spawnPrefab);
            Utilities.FixMixerGroups(whiteOrb.spawnPrefab);
            Items.RegisterScrap(whiteOrb, 2, Levels.LevelTypes.All);

            TerminalNode node2 = ScriptableObject.CreateInstance<TerminalNode>();
            node2.clearPreviousText = true;
            node2.displayText = "Info test zu orb";
            Items.RegisterShopItem(whiteOrb, null, null, node2, 0);

            //Logger

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogMessage(modGUID + " has loaded succesfully.");

            //Patching

            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), modGUID);
            harmony.PatchAll(typeof(EnemyLoot));
            harmony.PatchAll(typeof(MaskedDrop));
            harmony.PatchAll(typeof(SpiderDrop));
            harmony.PatchAll(typeof(HoarderBugDrop));
            harmony.PatchAll(typeof(SnareFleaDrop));
            harmony.PatchAll(typeof(BrakenDrop));
        }
    }

}
