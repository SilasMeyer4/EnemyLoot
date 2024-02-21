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
        public const string modName = "Enemy Loot";
        public const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static EnemyLoot Instance;

        public static Item spiderEgg;
        public static Item guiltyGearCase;
        internal static AudioClip newAudio;

        internal ManualLogSource mls;



        internal ConfigEntry<bool> SpiderDropSpiderEgg;
        internal ConfigEntry<bool> MaskedDropMask;
        //internal ConfigEntry<bool> MaskedDropBody;
        internal ConfigEntry<bool> HoarderDropGuiltyGear;


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

            //Loads Assets

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "enemyloot");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            //Adds the spiderEgg

            spiderEgg = bundle.LoadAsset<Item>("Assets/Items/SpiderEggItem.asset");
            NetworkPrefabs.RegisterNetworkPrefab(spiderEgg.spawnPrefab);
            Utilities.FixMixerGroups(spiderEgg.spawnPrefab);
            Items.RegisterScrap(spiderEgg, 0, Levels.LevelTypes.All);

            //Adds Guilty Gear Strive Case

            guiltyGearCase = bundle.LoadAsset<Item>("Assets/Items/GuiltyGearCaseItem.asset");
            EnemyLoot.newAudio = bundle.LoadAsset<AudioClip>("Assets/Audio/GuiltyGearTownInsideMe.mp3");
            GuiltyGearCaseBehaviour script = guiltyGearCase.spawnPrefab.AddComponent<GuiltyGearCaseBehaviour>();
            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = guiltyGearCase;
            NetworkPrefabs.RegisterNetworkPrefab(guiltyGearCase.spawnPrefab);
            Utilities.FixMixerGroups(guiltyGearCase.spawnPrefab);
            Items.RegisterScrap(guiltyGearCase, 3, Levels.LevelTypes.All);

            

            //Logger

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogMessage(modGUID + " has loaded succesfully.");

            //Patching

            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), modGUID);
            harmony.PatchAll(typeof(EnemyLoot));
            harmony.PatchAll(typeof(MaskedDrop));
            harmony.PatchAll(typeof(SpiderDrop));
            harmony.PatchAll(typeof(HoarderBugDrop));
        }
    }

}
