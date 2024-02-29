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
    [HarmonyPatch(typeof(HoarderBugAI))]
    internal class HoarderBugDrop
    {


        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(HoarderBugAI __instance)
        {

            if (!Config.Instance.HoarderDropGuiltyGear.Value)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }


            int spawnValue = new System.Random().Next(1, 101);
            
            if (spawnValue <= Config.Instance.GuiltyGearSpawnRate.Value)
            {
                EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Try spawning Guilty Gear Case");
                Item itemCase = EnemyLoot_SilasMeyer.EnemyLoot.guiltyGearCase;

                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemCase.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
                gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

                int scrapValue = new System.Random().Next(50, 90);
                gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
                gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
                RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
                {
                gameObject.GetComponent<NetworkObject>()
                }, new int[]
                {
                gameObject.GetComponent<GrabbableObject>().scrapValue
                });

                EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Guilty Gear Case was spawned");
            }

          





        }




    }
}

