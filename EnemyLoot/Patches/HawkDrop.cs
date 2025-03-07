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
   [HarmonyPatch(typeof(BaboonBirdAI))]
   internal class HawkDrop
    {
      [HarmonyPatch("KillEnemy")]
      [HarmonyPostfix]
      static void Patch(BaboonBirdAI __instance)
      {

         if (!EnemyLoot.Config.ThumperDropOrangeOrb.Value)
         {
            return;
         }

         if (!NetworkManager.Singleton.IsServer)
         {
            return;
         }

         EnemyLoot.Instance.mls.LogMessage("Creating feather");
         Item orangeOrb = EnemyLoot.orangeOrb;

         GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orangeOrb.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
         gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;
         int scrapValue = new System.Random().Next(90, 120);
         gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
         gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
         RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
         {
                gameObject.GetComponent<NetworkObject>()
         }, new int[]
         {
                gameObject.GetComponent<GrabbableObject>().scrapValue
         });

         EnemyLoot.Instance.mls.LogMessage("feather was created");
      }
   }
}
