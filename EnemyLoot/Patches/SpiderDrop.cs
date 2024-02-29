using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using SilasMeyer_EnemyLoot;


namespace EnemyLoot.Patches
{
    [HarmonyPatch(typeof(SandSpiderAI))]
    internal class SpiderDrop
    {


        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(SandSpiderAI __instance)
        {

            if (!Config.Instance.SpiderDropSpiderEgg.Value)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Try spawning egg");
            Item egg = EnemyLoot_SilasMeyer.EnemyLoot.spiderEgg;

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(egg.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
            gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

            int scrapValue = new System.Random().Next(300, 350);
            gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
            RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
            {
                gameObject.GetComponent<NetworkObject>()
            }, new int[]
            {
                gameObject.GetComponent<GrabbableObject>().scrapValue
            });

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Egg was spawned");



            

        }




    }
}
