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


namespace EnemyLoot.Patches
{
    [HarmonyPatch(typeof(CentipedeAI))]
    internal class SnareFleaDrop
    {


        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(CentipedeAI __instance)
        {

            if (!EnemyLoot_SilasMeyer.EnemyLoot.Instance.SnareFleaDropWhiteOrb.Value)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Creating White Orb");
            Item whiteOrb = EnemyLoot_SilasMeyer.EnemyLoot.whiteOrb;

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(whiteOrb.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
            gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;

            gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(20);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
            RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
            {
                gameObject.GetComponent<NetworkObject>()
            }, new int[]
            {
                gameObject.GetComponent<GrabbableObject>().scrapValue
            });

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("White Orb was created");
        }
    }
}