using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace EnemyLoot.Patches
{
   [HarmonyPatch(typeof(ButlerEnemyAI))]
   internal class ButlerDrop
   {
      [HarmonyPatch("KillEnemy")]
      [HarmonyPostfix]
      static void Patch(ButlerEnemyAI __instance)
      {

         if (!EnemyLoot.Config.ButlerDropSpoon.Value)
         {
            return;
         }

         if (!NetworkManager.Singleton.IsServer)
         {
            return;
         }

         EnemyLoot.Instance.mls.LogMessage("Creating Spoon");
         Item Spoon = EnemyLoot.Spoon;

         GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Spoon.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
         gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;
         int scrapValue = new System.Random().Next(333, 444);
         gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
         gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
         RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
         {
                gameObject.GetComponent<NetworkObject>()
         }, new int[]
         {
                gameObject.GetComponent<GrabbableObject>().scrapValue
         });

         EnemyLoot.Instance.mls.LogMessage("Spoon was created");
      }
   }
}
