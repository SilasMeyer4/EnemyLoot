﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace EnemyLoot.Patches
{
   [HarmonyPatch(typeof(MouthDogAI))]
   internal class EyelessDogDrop
   {

      [HarmonyPatch("KillEnemy")]
      [HarmonyPostfix]
      static void Patch(MouthDogAI __instance)
      {

         if (!EnemyLoot.Config.SpiderDropSpiderEgg.Value)
         {
            return;
         }

         if (!NetworkManager.Singleton.IsServer)
         {
            return;
         }

         EnemyLoot.Instance.mls.LogMessage("Try spawning eye");
         Item egg = EnemyLoot.spiderEgg;

         GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(egg.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
         gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

         int scrapValue = 666;
         gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
         gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
         RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
         {
                gameObject.GetComponent<NetworkObject>()
         }, new int[]
         {
                gameObject.GetComponent<GrabbableObject>().scrapValue
         });

         EnemyLoot.Instance.mls.LogMessage("Eye was spawned");
      }
   }
}