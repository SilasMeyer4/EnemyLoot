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
   [HarmonyPatch(typeof(FlowerSnakeEnemy))]
   internal class SnakeDrop
   {
      [HarmonyPatch("KillEnemy")]
      [HarmonyPostfix]
      static void Patch(FlowerSnakeEnemy __instance)
      {

         //if (!Config.Instance.SpiderDropSpiderEgg.Value)
         //{
         //   return;
         //}

         if (!NetworkManager.Singleton.IsServer)
         {
            return;
         }

         EnemyLoot.Instance.mls.LogMessage("Try spawning SnakeEgg");
         Item egg = EnemyLoot.SnakeEgg;

         GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(egg.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
         gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

         int scrapValue = new System.Random().Next(10, 15);
         gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
         gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
         RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
         {
                gameObject.GetComponent<NetworkObject>()
         }, new int[]
         {
                gameObject.GetComponent<GrabbableObject>().scrapValue
         });

         EnemyLoot.Instance.mls.LogMessage("SnakeEgg was spawned");
      }
   }
   
}
