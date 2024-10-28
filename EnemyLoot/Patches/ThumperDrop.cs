using HarmonyLib;
using UnityEngine;
using Unity.Netcode;


namespace EnemyLoot.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class ThumperDrop
    {


        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(CrawlerAI __instance)
        {

            if (!EnemyLoot.Config.ThumperDropOrangeOrb.Value)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            EnemyLoot.Instance.mls.LogMessage("Creating Orange Orb");
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

            EnemyLoot.Instance.mls.LogMessage("Orange Orb was created");
        }
    }
}