using CSync.Lib;
using GameNetcodeStuff;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace EnemyLoot.Behaviours
{
   internal class WeirdHeadBehaviour : PhysicsProp
   {
      private int activationCounter = 0;
      private bool _isTimerRunning = false;
      private AudioSource _audioSource;
      private PlayerControllerB _player;
      public UnityEngine.Vector3 EnemyPosition;
      public UnityEngine.Vector3 PlayerPosition;
      static List<EnemyAI> _enemyList = new List<EnemyAI>();
      public EnemyAI Enemy;
      internal EntranceTeleport[] EntranceArray;
      private bool _wasInsideBeforeTeleport;
      private static Dictionary<int, AudioSource> _audioSourceMap = new Dictionary<int, AudioSource>();
      static System.Random random = new System.Random();
      private int _searchCounter;


      public override void ItemActivate(bool used, bool buttonDown = true)
      {
         _player = playerHeldBy;
         EntranceArray = UnityEngine.Object.FindObjectsOfType<EntranceTeleport>(false);


         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {

            _enemyList = RoundManager.Instance.SpawnedEnemies;
            _searchCounter = 0;
            //Try getting valid enemy
            do
            {
               if (_enemyList == null || _enemyList.Count == 0)
               {
                  EnemyLoot.Instance.mls.LogMessage("No enemy exists");
                  return;
               }
               int _index = random.Next(_enemyList.Count);
               Enemy = _enemyList[_index];
               EnemyLoot.Instance.mls.LogMessage("Found: " + Enemy.GetType().Name + " | SearchCounter: " + _searchCounter);

            } while (!IsEnemyTypeValid(Enemy) && _searchCounter < 5);


            if (_searchCounter >= 5)
            {
               EnemyLoot.Instance.mls.LogMessage("Tried finding valid enemy - failed");
               return;
            }

            if (Enemy == null)
            {
               _audioSource = gameObject.GetComponent<AudioSource>();
               _audioSource.clip = EnemyLoot.WeirdHeadNoEnemySFX;
               return;
            }

            EnemyPosition = Enemy.transform.position;

            PlayerPosition = _player.transform.position;

            if (!_isTimerRunning)
            {

               activationCounter++;
               StartCoroutine(orbTeleport());
            }
            else
            {
               _audioSource = gameObject.GetComponent<AudioSource>();
               System.Random rnd = new System.Random();
               int index = rnd.Next(EnemyLoot.WeirdHeadWhistles.Count);
               _audioSource.clip = EnemyLoot.WeirdHeadWhistles[index];
               _audioSource.Play();

            }
         }
      }

      private bool IsEnemyTypeValid(EnemyAI _enemy)
      {
         _searchCounter++;
         switch (_enemy.GetType().Name)
         {
            
            case "DoublewingAI": 
            case "RedLocustBees":
            case "ButlerBeesEnemyAI":
            case "DocileLocustBeesAI":
            case "SandWormAI":
            case "DressGirlAI":
            case "FlowerSnakeEnemy":
               return false;
              
            default:
               return true;
         }
      }

      private IEnumerator orbTeleport()
      {
      
         _isTimerRunning = true;
         _audioSource = gameObject.GetComponent<AudioSource>();
         _audioSource.clip = EnemyLoot.WeirdHeadTeleportSFX;
         _audioSource.Play();

         _wasInsideBeforeTeleport = _player.isInsideFactory;
         SpawnableEnemyWithRarity _enemyToSpawn = null;
         bool _IsOutsideEnemy = false;

         SetControlTipsForItem();
         yield return new WaitForSeconds(0.5f);
         //Teleport
          

         if (_wasInsideBeforeTeleport && Enemy.isOutside)
         {
            EntranceArray[0].TeleportPlayer();
         }
         else if (!_wasInsideBeforeTeleport && !Enemy.isOutside)
         {
            EntranceArray[EntranceArray.Length / 2].TeleportPlayer();
         }

         //TODO EnemyTeleport funktioniert noch nicht
         _player.TeleportPlayer(EnemyPosition);
         RoundManager.Instance.DespawnEnemyOnServer(Enemy.NetworkObject);

         //Search Enemy in enemies
         if (!Enemy.enemyType.isOutsideEnemy)
         {
            foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in RoundManager.Instance.currentLevel.Enemies)
            {
               EnemyLoot.Instance.mls.LogMessage("Index of: " + spawnableEnemyWithRarity.enemyType.name + " | " + RoundManager.Instance.currentLevel.Enemies.IndexOf(spawnableEnemyWithRarity));
               if (spawnableEnemyWithRarity.enemyType == Enemy.enemyType)
               {
                 
                  _enemyToSpawn = spawnableEnemyWithRarity;
                  break;
               }
            }
         }
         else
         {

            foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in RoundManager.Instance.currentLevel.OutsideEnemies)
            {
               EnemyLoot.Instance.mls.LogMessage("Index of: " + spawnableEnemyWithRarity.enemyType.name + " | " + RoundManager.Instance.currentLevel.Enemies.IndexOf(spawnableEnemyWithRarity));
               if (spawnableEnemyWithRarity.enemyType == Enemy.enemyType)
               {
                  _enemyToSpawn = spawnableEnemyWithRarity;
                  _IsOutsideEnemy = true;
                  break;
               }
            }

         }

        

         if (_enemyToSpawn != null)
         {
            if (!_IsOutsideEnemy) //TODO. hmm EnemySpawns only for me
            {
               RoundManager.Instance.SpawnEnemyOnServer(
               PlayerPosition, PlayerPosition.y, RoundManager.Instance.currentLevel.Enemies.IndexOf(_enemyToSpawn));
            }

            //RoundManager.Instance.SpawnEnemyGameObject(PlayerPosition, PlayerPosition.y, 1, Enemy.enemyType);
            EnemyLoot.Instance.mls.LogMessage(Enemy.enemyType.enemyName + "should have been spawned");
            EnemyLoot.Instance.mls.LogMessage(_enemyToSpawn.enemyType.name + " was spawned");
            SetControlTipsForItem();


            yield return new WaitForSeconds(10);
         }
         else
         {
            EnemyLoot.Instance.mls.LogMessage("Wasn't able to spawn Enemy");
         }


         _isTimerRunning = false;

         SetControlTipsForItem();
      }



      public override void GrabItem()
      {
         base.GrabItem();
         WeirdHeadSoundPatch.WasPickedUp = true;
         EnemyLoot.Instance.mls.LogMessage("Picked Up Head");
         _audioSource = gameObject.GetComponent<AudioSource>();

         int instanceId = gameObject.GetInstanceID();
         _audioSourceMap[instanceId] = _audioSource;

      }

      public override void Update()
      {

         base.Update();
         //LockForEnemies();

      }

      //private bool LockForEnemies()
      //{
      //   SetControlTipsForItem();
      //   if (RoundManager.Instance.SpawnedEnemies.Count == 0)
      //   {
      //      return false;
      //   }

      //   return true;

      //}

      public static AudioSource GetRandomAudioSource()
      {
         if (_audioSourceMap.Count == 0)
         {
            return null;
         }
         System.Random rand = new System.Random();
         return _audioSourceMap.ElementAt(rand.Next(0, _audioSourceMap.Count)).Value;
      }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop Weird Head : [G]",
            "Weird Head is sleeping"
            };

         if (_isTimerRunning)
         {
            toolTips[1] = "Weird Head on cooldown";
         }
         else if (RoundManager.Instance.SpawnedEnemies.Count > 0)
         {
            toolTips[1] = "Activate Weird Head : [LMB]";
         }

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);

      }

      public override void OnDestroy()
      {
         base.OnDestroy();
         WeirdHeadSoundPatch.WasPickedUp = false;
      }

      public override void DiscardItem()
      {
         base.DiscardItem();
         WeirdHeadSoundPatch.WasPickedUp = false;
         EnemyLoot.Instance.mls.LogMessage("Drop Item");
      }

   }

   [HarmonyPatch(typeof(SoundManager))]
   internal class WeirdHeadSoundPatch
   {

      public static bool WasPickedUp = false;
      public static bool OnCooldown = false;
      public static AudioSource AudioSource;
      public static WeirdHeadBehaviour itemInPatch = null;


      [HarmonyPatch("Update")]
      [HarmonyPostfix]
      static void Patch()
      {

         if (WasPickedUp && !OnCooldown)
         {
            EnemyLoot.Instance.mls.LogMessage("Playing Sound");
            SoundManager.Instance.StartCoroutine(PlayHeadSound());
         }


      }

      private static IEnumerator PlayHeadSound()
      {
         OnCooldown = true;
         int minCD = 10;
         int maxCD = 20;

         EnemyLoot.Instance.mls.LogMessage("In Sound function");

         System.Random rnd2 = new System.Random();
         int counter = rnd2.Next(minCD, maxCD);
         int index = rnd2.Next(EnemyLoot.WeirdHeadFlutes.Count);


         var _networkManager = NetworkManager.Singleton;

         yield return new WaitForSeconds(counter / 2);



         if (_networkManager.IsServer)
         {
            RpcPlaySoundOnServer(EnemyLoot.WeirdHeadFlutes[index]);
         }
         else
         {
            RpcPlaySoundOnClients(EnemyLoot.WeirdHeadFlutes[index]);
         }

         yield return new WaitForSeconds(counter / 2);

         OnCooldown = false;

      }


      [ClientRpc]
      private static void RpcPlaySoundOnClients(AudioClip audioClip)
      {
         var audioSource = new GameObject("GlobalAudioSource").AddComponent<AudioSource>();
         audioSource.clip = audioClip;
         audioSource.spatialBlend = 0;
         audioSource.volume = 1.2f;
         audioSource.Play();

         GameObject.Destroy(audioSource.gameObject, audioClip.length);
      }

      [ServerRpc]
      public static void RpcPlaySoundOnServer(AudioClip audioClip)
      {
         RpcPlaySoundOnClients(audioClip);
      }

   }

   [HarmonyPatch(typeof(EnemyAI))]
   [HarmonyPatch("PlayerIsTargetable")]
   internal class PlayerTargetablePatch
   {
      static bool Prefix(ref bool overrideInsideFactoryCheck)
      {
         overrideInsideFactoryCheck = true;

         return true;
      }
   }

   [HarmonyPatch(typeof(EnemyAI))]
   [HarmonyPatch("PlayerIsTargetable")]
   internal class PlayerInvinciblePatch
   {
      static bool Prefix(ref bool __result)
      {
         __result = false;
         EnemyLoot.Instance.mls.LogMessage("Player is invicible");

         return false;
      }
   }






}
