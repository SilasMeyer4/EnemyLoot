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
using CSync.Lib;
using CSync.Util;
using System.Runtime.Serialization;

namespace EnemyLoot
{
   [BepInPlugin(modGUID, modName, modVersion)]
   [BepInDependency("io.github.CSync")]
   public class EnemyLoot : BaseUnityPlugin
   {
      public const string modGUID = "SilasMeyer.EnemyLoot";
      public const string modName = "EnemyLoot";
      public const string modVersion = "0.2.0";

      private readonly Harmony harmony = new Harmony(modGUID);

      public static EnemyLoot Instance;

      public static Config MyConfig;

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

      void Awake()

      {
         if (Instance == null)
         {
            Instance = this;
         }

         MyConfig = new Config(base.Config);

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




         //Adds Items to the shop for Host



         //Logger

         mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
         mls.LogMessage(modGUID + " has loaded succesfully.");

         //Patching

         //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), modGUID);
         harmony.PatchAll(typeof(Shop));
         harmony.PatchAll(typeof(EnemyLoot));
         harmony.PatchAll(typeof(MaskedDrop));
         harmony.PatchAll(typeof(SpiderDrop));
         harmony.PatchAll(typeof(HoarderBugDrop));
         harmony.PatchAll(typeof(SnareFleaDrop));
         harmony.PatchAll(typeof(BrakenDrop));
         harmony.PatchAll(typeof(ThumperDrop));
         harmony.PatchAll(typeof(Config));
         // harmony.PatchAll(typeof(OrangeOrbSprintMeterPatch));
      }
   }

   [DataContract]
   public class Config : SyncedConfig<Config>
   {

      [DataMember] public SyncedEntry<bool> areItemsInShop;

      [DataMember] public SyncedEntry<bool> SpiderDropSpiderEgg { get; private set; }
      [DataMember] public SyncedEntry<bool> MaskedDropMask { get; private set; }
      [DataMember] public SyncedEntry<bool> HoarderDropGuiltyGear { get; private set; }
      [DataMember] public SyncedEntry<bool> BrackenDropBlackOrb { get; private set; }
      [DataMember] public SyncedEntry<bool> SnareFleaDropWhiteOrb { get; private set; }
      [DataMember] public SyncedEntry<bool> ThumperDropOrangeOrb { get; private set; }
      [DataMember] public SyncedEntry<int> GuiltyGearSpawnRate { get; private set; }

      public Config(ConfigFile cfg) : base(EnemyLoot.modGUID)
      {
         ConfigManager.Register(this);

         areItemsInShop = cfg.BindSyncedEntry("1. General", "Activate Items in shop", true, "Adds the new items to the shop");

         SpiderDropSpiderEgg = cfg.BindSyncedEntry("2. Drops", "Drop Spider Egg", true, "Spider drops egg on death");
         MaskedDropMask = cfg.BindSyncedEntry("2. Drops", "Drop Mask", true, "Masked drops mask on death");
         HoarderDropGuiltyGear = cfg.BindSyncedEntry("2. Drops", "Drop Guilty Gear Case", true, "Hoarder Bug can drop Guilty Gear Strive Case");
         BrackenDropBlackOrb = cfg.BindSyncedEntry("2. Drops", "Drop Black Orb", true, "Braken can drop Black Orb");
         SnareFleaDropWhiteOrb = cfg.BindSyncedEntry("2. Drops", "Drop White Orb", true, "Snare Flea can drop White Orb");
         ThumperDropOrangeOrb = cfg.BindSyncedEntry("2. Drops", "Drop Orange Orb", true, "Thumper can drop Orange Orb");
         GuiltyGearSpawnRate = cfg.BindSyncedEntry("3. Spawnrates", "Guilty Gear Spawnrate", 60, "Spawnrate in percent of the Guilty Gear drop when killing a Hoarder Bug. Enter a number from 0-100.");
      }
   }

}
