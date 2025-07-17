using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EnemyLoot.Behaviours;
using EnemyLoot.Patches;
using HarmonyLib;
using LethalLib.Modules;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace EnemyLoot
{
   [BepInPlugin(modGUID, modName, modVersion)]
   //[BepInDependency("com.sigurd.csync", "5.0.1")]
   public class EnemyLoot : BaseUnityPlugin
   {
      public const string modGUID = "SilasMeyer.EnemyLoot";
      public const string modName = "EnemyLoot";
      public const string modVersion = "0.3.3";

      private readonly Harmony harmony = new Harmony(modGUID);

      public static EnemyLoot Instance;

      public static new MyConfig Config;

      public static Item spiderEgg;
      public static Item guiltyGearCase;
      public static Item blackOrb;
      public static Item whiteOrb;
      public static Item orangeOrb;
      public static Item Spoon;
      public static Item SnakeEgg;
      public static Item WeirdHead;
      public static Item Eye;
      public static AssetBundle bundle;
      internal static SpoonBehaviour ScriptSpoon;

      internal static AudioClip guiltyGearSFX;
      internal static AudioClip blackOrbSpawnSFX;
      internal static AudioClip blackOrbTeleportSFX;
      internal static AudioClip blackOrbCDSFX;
      internal static AudioClip whiteOrbActivationSFX;
      internal static AudioClip whiteOrbDestroySFX;
      internal static AudioClip whiteOrbCDSFX;
      internal static AudioClip orangeOrbCDSFX;
      internal static AudioClip orangeOrbActivationSFX;
      internal static AudioClip SpoonCDSFX;
      internal static AudioClip SpoonActivationSFX;
      internal static AudioClip WeirdHeadNoEnemySFX;
      internal static AudioClip WeirdHeadTeleportSFX;
      internal static AudioClip WeirdHeadCDSFX;
      internal static List<AudioClip> WeirdHeadWhistles = new List<AudioClip>();
      internal static List<AudioClip> WeirdHeadFlutes = new List<AudioClip>();
      internal static List<AudioClip> WeirdHeadVoices = new List<AudioClip>();

      internal static Sprite IconBase;
      internal static Sprite IconCD;
      internal static Sprite IconActive;

      public bool isOrangeOrbActive = false;

      internal ManualLogSource mls;

      void Awake()

      {
         if (Instance == null)
         {
            Instance = this;
         }

         Config = new MyConfig(base.Config);

         mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
         mls.LogMessage("Loading " + modName);



         //Loads Assets

         string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemyloot");
         bundle = AssetBundle.LoadFromFile(assetDir);

         //Loading Icons

         IconBase = bundle.LoadAsset<Sprite>("Assets/Items/Icons/EnemyLootIconBase.png");
         IconCD = bundle.LoadAsset<Sprite>("Assets/Items/Icons/EnemyLootIconCD.png");
         IconActive = bundle.LoadAsset<Sprite>("Assets/Items/Icons/EnemyLootIconActive.png");


         //Adds the spiderEgg

         spiderEgg = bundle.LoadAsset<Item>("Assets/Items/SpiderEggItem.asset");
         spiderEgg.positionOffset = new Vector3(-0.2f, 0, 0.3f);
         spiderEgg.rotationOffset = new Vector3(-90, 0, -10);
         PhysicsProp spiderEggPysics = spiderEgg.spawnPrefab.GetComponent<PhysicsProp>();
         spiderEggPysics.grabbableToEnemies = true;
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

         //Adds Spoon

         Spoon = bundle.LoadAsset<Item>("Assets/Items/SpoonItem.asset");
         EnemyLoot.SpoonActivationSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/SpoonActivateSFX.mp3");
         EnemyLoot.SpoonCDSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/SpoonCDSFX.mp3");
         ScriptSpoon = Spoon.spawnPrefab.AddComponent<SpoonBehaviour>();
         ScriptSpoon.grabbable = true;
         ScriptSpoon.grabbableToEnemies = true;
         ScriptSpoon.itemProperties = Spoon;
         Spoon.rotationOffset = new Vector3(0, -70, 0);
         Spoon.positionOffset = new Vector3(0.03f, 0.03f, -0.03f);
         NetworkPrefabs.RegisterNetworkPrefab(Spoon.spawnPrefab);
         Utilities.FixMixerGroups(Spoon.spawnPrefab);
         Items.RegisterScrap(Spoon, 0, Levels.LevelTypes.All);

         //Adds Weird Head

         WeirdHead = bundle.LoadAsset<Item>("Assets/Items/WeirdHeadItem.asset");

         EnemyLoot.WeirdHeadTeleportSFX = bundle.LoadAsset<AudioClip>("Assets/Audio/HeadSwapSFX.mp3");
         EnemyLoot.WeirdHeadWhistles.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadWhistle1.mp3"));
         EnemyLoot.WeirdHeadWhistles.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadWhistle2.mp3"));
         EnemyLoot.WeirdHeadWhistles.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadWhistle3.mp3"));
         EnemyLoot.WeirdHeadWhistles.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/Headwhistle4.mp3"));
         EnemyLoot.WeirdHeadFlutes.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadFlute1.mp3"));
         EnemyLoot.WeirdHeadFlutes.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadFlute2.mp3"));
         EnemyLoot.WeirdHeadFlutes.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadFlute3.mp3"));
         EnemyLoot.WeirdHeadFlutes.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadFlute4.mp3"));
         EnemyLoot.WeirdHeadFlutes.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadFlute5.mp3"));
         EnemyLoot.WeirdHeadVoices.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadVoiceNiceTry.mp3"));
         EnemyLoot.WeirdHeadVoices.Add(bundle.LoadAsset<AudioClip>("Assets/Audio/HeadVoiceEscape.mp3"));


         WeirdHeadBehaviour _scriptWeirdHead = WeirdHead.spawnPrefab.AddComponent<WeirdHeadBehaviour>();
         _scriptWeirdHead.grabbable = true;
         _scriptWeirdHead.grabbableToEnemies = true;
         _scriptWeirdHead.itemProperties = WeirdHead;
         WeirdHead.rotationOffset = new Vector3(20, 150, 0);
         WeirdHead.positionOffset = new Vector3(-0.04f, 0.02f, -0.02f);
         NetworkPrefabs.RegisterNetworkPrefab(WeirdHead.spawnPrefab);
         Utilities.FixMixerGroups(WeirdHead.spawnPrefab);
         Items.RegisterScrap(WeirdHead, 0, Levels.LevelTypes.All);



         //Logger


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

         harmony.PatchAll(typeof(SpoonDamagePatch));


         harmony.PatchAll(typeof(ButlerDrop));
         harmony.PatchAll(typeof(OrangeOrbSprintMeterPatch));
         harmony.PatchAll(typeof(WeirdHeadSoundPatch));
         harmony.PatchAll(typeof(ManeaterDrop));
         harmony.PatchAll(typeof(PlayerTargetablePatch));
         //harmony.PatchAll(typeof(SnakeDrop));
         //harmony.PatchAll(typeof(PlayerInvinciblePatch));
      }
   }



   //public class MyConfig : CSync.Lib.SyncedConfig2<MyConfig>
   //{

   //   public ConfigEntry<float> DebugLevel { get; private set; }

   //   [CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> areItemsInShop;

   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> SpiderDropSpiderEgg { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> MaskedDropMask { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> HoarderDropGuiltyGear { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> BrackenDropBlackOrb { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> SnareFleaDropWhiteOrb { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> ThumperDropOrangeOrb { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<bool> ButlerDropSpoon { get; private set; }
   //   [field: CSync.Lib.SyncedEntryField] public CSync.Lib.SyncedEntry<int> GuiltyGearSpawnRate { get; private set; }

   //   public MyConfig(ConfigFile cfg) : base(EnemyLoot.modGUID)
   //   {


   //      areItemsInShop = cfg.BindSyncedEntry("1. General", "Activate Items in shop", true, "Adds the new items to the shop");
   //      SpiderDropSpiderEgg = cfg.BindSyncedEntry("2. Drops", "Drop Spider Egg", true, "Spider drops egg on death");
   //      MaskedDropMask = cfg.BindSyncedEntry("2. Drops", "Drop Mask", true, "Masked drops mask on death");
   //      HoarderDropGuiltyGear = cfg.BindSyncedEntry("2. Drops", "Drop Guilty Gear Case", true, "Hoarder Bug can drop Guilty Gear Strive Case");
   //      BrackenDropBlackOrb = cfg.BindSyncedEntry("2. Drops", "Drop Black Orb", true, "Braken can drop Black Orb");
   //      SnareFleaDropWhiteOrb = cfg.BindSyncedEntry("2. Drops", "Drop White Orb", true, "Snare Flea can drop White Orb");
   //      ThumperDropOrangeOrb = cfg.BindSyncedEntry("2. Drops", "Drop Orange Orb", true, "Thumper can drop Orange Orb");
   //      ButlerDropSpoon = cfg.BindSyncedEntry("2. Drops", "Drop Spoon", true, "Butler can drop a spoon");
   //      GuiltyGearSpawnRate = cfg.BindSyncedEntry("3. Spawnrates", "Guilty Gear Spawnrate", 60, "Spawnrate in percent of the Guilty Gear drop when killing a Hoarder Bug. Enter a number from 0-100.");

   //      ConfigManager.Register(this);
   //   }
   //}

   public class MyConfig
   {

      public readonly ConfigEntry<float> DebugLevel;

      public readonly ConfigEntry<bool> areItemsInShop;

      public readonly ConfigEntry<bool> SpiderDropSpiderEgg;
      public readonly ConfigEntry<bool> MaskedDropMask;
      public readonly ConfigEntry<bool> HoarderDropGuiltyGear;
      public readonly ConfigEntry<bool> BrackenDropBlackOrb;
      public readonly ConfigEntry<bool> SnareFleaDropWhiteOrb;
      public readonly ConfigEntry<bool> ThumperDropOrangeOrb;
      public readonly ConfigEntry<bool> ButlerDropSpoon;
      public readonly ConfigEntry<int> GuiltyGearSpawnRate;

      public MyConfig(ConfigFile cfg)
      {

         cfg.SaveOnConfigSet = false;

         areItemsInShop = cfg.Bind("1. General", "Activate Items in shop", false, "Adds the new items to the shop");
         SpiderDropSpiderEgg = cfg.Bind("2. Drops", "Drop Spider Egg", true, "Spider drops egg on death");
         MaskedDropMask = cfg.Bind("2. Drops", "Drop Mask", true, "Masked drops mask on death");
         HoarderDropGuiltyGear = cfg.Bind("2. Drops", "Drop Guilty Gear Case", true, "Hoarder Bug can drop Guilty Gear Strive Case");
         BrackenDropBlackOrb = cfg.Bind("2. Drops", "Drop Black Orb", true, "Braken can drop Black Orb");
         SnareFleaDropWhiteOrb = cfg.Bind("2. Drops", "Drop White Orb", true, "Snare Flea can drop White Orb");
         ThumperDropOrangeOrb = cfg.Bind("2. Drops", "Drop Orange Orb", true, "Thumper can drop Orange Orb");
         ButlerDropSpoon = cfg.Bind("2. Drops", "Drop Spoon", true, "Butler can drop a spoon");
         GuiltyGearSpawnRate = cfg.Bind("3. Spawnrates", "Guilty Gear Spawnrate", 60, "Spawnrate in percent of the Guilty Gear drop when killing a Hoarder Bug. Enter a number from 0-100.");

         ClearOrphanedEntries(cfg);
         cfg.Save();

         cfg.SaveOnConfigSet = true;

      }


      static void ClearOrphanedEntries(ConfigFile cfg)
      {
         PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
         var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
         orphanedEntries.Clear();
      }
   }



}
