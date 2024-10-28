using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace EnemyLoot.Patches
{
   [HarmonyPatch(typeof(CaveDwellerAI))]
   internal class ManeaterDrop
   {
      [HarmonyPatch("KillEnemy")]
      [HarmonyPostfix]
      static void Patch(CaveDwellerAI __instance)
      {

         //if (!Config.Instance.ThumperDropWeirdHead.Value)
         //{
         //   return;
         //}

         if (!NetworkManager.Singleton.IsServer)
         {
            return;
         }

         EnemyLoot.Instance.mls.LogMessage("Creating Weird Head");
         Item WeirdHead = EnemyLoot.WeirdHead;

         GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(WeirdHead.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
         gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;
         int scrapValue = new System.Random().Next(400, 700);
         gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
         gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
         RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
         {
                gameObject.GetComponent<NetworkObject>()
         }, new int[]
         {
                gameObject.GetComponent<GrabbableObject>().scrapValue
         });

         EnemyLoot.Instance.mls.LogMessage("Weird Head was created");
      }
   }
}
