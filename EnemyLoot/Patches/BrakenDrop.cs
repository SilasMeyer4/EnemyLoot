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
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class BrakenDrop
    {


        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(FlowermanAI __instance)
        {

            if (!Config.Instance.BrackenDropBlackOrb.Value)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Creating Black Orb");
            Item blackOrb = EnemyLoot_SilasMeyer.EnemyLoot.blackOrb;

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(blackOrb.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
            gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

            int scrapValue = new System.Random().Next(350, 400);
            gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
            RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
            {
                gameObject.GetComponent<NetworkObject>()
            }, new int[]
            {
                gameObject.GetComponent<GrabbableObject>().scrapValue
            });

            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.blackOrbSpawnSFX;
            audioSource.Play();
            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Black Orb was created");
        }
    }
}
