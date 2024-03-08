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
using EnemyLoot;


namespace EnemyLoot.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedDrop
    {




        private static Item _mask;



        [HarmonyPatch("KillEnemy")]
        [HarmonyPostfix]
        static void Patch(MaskedPlayerEnemy __instance)
        {

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (Config.Instance.MaskedDropMask.Value)
            {
                return;
            }

            EnemyLoot.Instance.mls.LogMessage("Try getting Mask");
                Item mask = MaskedDrop.GetMask();

                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(mask.spawnPrefab, __instance.transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
                gameObject.GetComponentInChildren<GrabbableObject>().fallTime = 0f;
                int scrapValue = new System.Random().Next(40, 60);
                gameObject.GetComponentInChildren<GrabbableObject>().SetScrapValue(scrapValue);
                gameObject.GetComponentInChildren<NetworkObject>().Spawn(false);
                RoundManager.Instance.SyncScrapValuesClientRpc(new NetworkObjectReference[]
                {
                gameObject.GetComponent<NetworkObject>()
                }, new int[]
                {
                gameObject.GetComponent<GrabbableObject>().scrapValue
                });

                EnemyLoot.Instance.mls.LogMessage("Mask was spawned");
            

            //funktioniert irgendwie noch nicht
            //if (EnemyLoot_SilasMeyer.EnemyLoot.Instance.MaskedDropBody.Value && __instance.mimickingPlayer.deadBody == null &&
            //    __instance.mimickingPlayer != null && __instance.mimickingPlayer.isPlayerDead)
            //{
            //    Body();
            //    EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Dropped Body!");
            //}
            //else
            //{
            //    EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage("Did not drop Body");
            //}

            //void Body()
            //{
            //    DeadBodyInfo deadPlayerBody = __instance.mimickingPlayer.deadBody;
            //    deadPlayerBody.gameObject.SetActive(true);
            //    __instance.gameObject.SetActive(false);
            //    deadPlayerBody.SetBodyPartsKinematic(false);
            //    deadPlayerBody.SetRagdollPositionSafely(__instance.transform.position, false);
            //    deadPlayerBody.deactivated = false;
            //    deadPlayerBody.transform.Find("spine.001/spine.002/spine.003/spine.004/HeadMask").gameObject.SetActive(false);
            //}



        }

        //fetches a mask from the ItemList
        private static Item GetMask()
        {

            if (MaskedDrop._mask == null)
            {
              //  int randomValue = new System.Random().Next(0, 2);

                //if (randomValue == 1)
                //{
                //MaskedDrop._mask = Enumerable.First<Item>(StartOfRound.Instance.allItemsList.itemsList, (Item m) => m.name == "TragedyMask");
                //} else
                //{
                MaskedDrop._mask = Enumerable.First<Item>(StartOfRound.Instance.allItemsList.itemsList, (Item m) => m.name == "ComedyMask");
              //  }
                
            }
            EnemyLoot.Instance.mls.LogMessage(MaskedDrop._mask + ": Found Mask");
            return MaskedDrop._mask;
        }



    }
}
