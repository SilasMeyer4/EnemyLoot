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
        public const string modVersion = "0.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static EnemyLoot Instance;

        public static Item spiderEgg;
        public static Item guiltyGearCase;
        public static Item blackOrb;
        public static Item whiteOrb;
        public static Item orangeOrb;

        internal static AudioClip guiltyGearSFX;
        internal static AudioClip blackOrbSpawnSFX;
        internal static AudioClip blackOrbTeleportSFX;
        internal static AudioClip blackOrbCDSFX;
        internal static AudioClip whiteOrbActivationSFX;
        internal static AudioClip whiteOrbDestroySFX;
        internal static AudioClip whiteOrbCDSFX;
        internal static AudioClip orangeOrbCDSFX;
        internal static AudioClip orangeOrbActivationSFX;

        public bool isOrangeOrbActive = false;

        internal ManualLogSource mls;



        internal ConfigEntry<bool> SpiderDropSpiderEgg;
        internal ConfigEntry<bool> MaskedDropMask;
        //internal ConfigEntry<bool> MaskedDropBody;
        internal ConfigEntry<bool> HoarderDropGuiltyGear;
        internal ConfigEntry<bool> BrackenDropBlackOrb;
        internal ConfigEntry<bool> SnareFleaDropWhiteOrb;
        internal ConfigEntry<bool> ThumperDropOrangeOrb;
        internal ConfigEntry<int> GuiltyGearSpawnRate;



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
            this.ThumperDropOrangeOrb = base.Config.Bind<bool>("General", "Drop Orange Orb", true, "Thumper can drop Orange Orb");
            this.GuiltyGearSpawnRate = base.Config.Bind<int>("General", "Guilty Gear Spawnrate", 60, "Spawnrate of Guilty Gear drop when killing a Hoarder Bug");
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



            //Adds White Orb

            whiteOrb = bundle.LoadAsset<Item>("Assets/Items/WhiteOrbItem.asset");
            EnemyLoot.whiteOrbCDSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/WhiteOrbCDSFX.mp3");
            EnemyLoot.whiteOrbActivationSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/WhiteOrbHealSFX.mp3");
            EnemyLoot.whiteOrbDestroySFX = bundle.LoadAsset<AudioClip>("Assets/Audio/WhiteOrbDestroySFX.mp3");
            WhiteOrbBehaviour scriptWO = whiteOrb.spawnPrefab.AddComponent<WhiteOrbBehaviour>();
            scriptWO.grabbable = true;
            scriptWO.grabbableToEnemies = true;
            scriptWO.itemProperties = whiteOrb;
            whiteOrb.rotationOffset = new Vector3(0, 0, 0);
            whiteOrb.positionOffset = new Vector3(-0.04f, 0.02f, -0.02f);
            NetworkPrefabs.RegisterNetworkPrefab(whiteOrb.spawnPrefab);
            Utilities.FixMixerGroups(whiteOrb.spawnPrefab);
            Items.RegisterScrap(whiteOrb, 0, Levels.LevelTypes.All);


            //Adds Orange Orb

            orangeOrb = bundle.LoadAsset<Item>("Assets/Items/OrangeOrbItem.asset");
            EnemyLoot.orangeOrbActivationSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/OrangeOrbActivationSFX.mp3");
            EnemyLoot.orangeOrbCDSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/OrangeOrbCDSFX.mp3");
            OrangeOrbBehaviour scriptOO = orangeOrb.spawnPrefab.AddComponent<OrangeOrbBehaviour>();
            scriptOO.grabbable = true;
            scriptOO.grabbableToEnemies = true;
            scriptOO.itemProperties = orangeOrb;
            orangeOrb.rotationOffset = new Vector3(0, 0, 0);
            orangeOrb.positionOffset = new Vector3(-0.04f, 0.02f, -0.02f);
            NetworkPrefabs.RegisterNetworkPrefab(orangeOrb.spawnPrefab);
            Utilities.FixMixerGroups(orangeOrb.spawnPrefab);
            Items.RegisterScrap(orangeOrb, 0, Levels.LevelTypes.All);



            //Adds Items to the shop

            //TerminalNode node1 = ScriptableObject.CreateInstance<TerminalNode>();
            //node1.clearPreviousText = true;
            //node1.displayText = "Info test zu Case";
            //Items.RegisterShopItem(guiltyGearCase, null, null, node1, 0);

            //TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            //node.clearPreviousText = true;
            //node.displayText = "Info test zu orb";
            //Items.RegisterShopItem(blackOrb, null, null, node, 0);

            //TerminalNode node2 = ScriptableObject.CreateInstance<TerminalNode>();
            //node2.clearPreviousText = true;
            //node2.displayText = "Info test zu orb";
            //Items.RegisterShopItem(whiteOrb, null, null, node2, 0);

            //TerminalNode node3 = ScriptableObject.CreateInstance<TerminalNode>();
            //node3.clearPreviousText = true;
            //node3.displayText = "Info test zu orb";
            //Items.RegisterShopItem(orangeOrb, null, null, node3, 0);

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
            harmony.PatchAll(typeof(ThumperDrop));
            // harmony.PatchAll(typeof(OrangeOrbSprintMeterPatch));
        }
    }

}
