using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using LethalLib.Modules;


namespace EnemyLoot
{
   [HarmonyPatch(typeof(PlayerControllerB))]
   public class Shop
   {



      [HarmonyPatch("Awake")]
      [HarmonyPostfix]
      static void loadShopHost()
      {
         EnemyLoot.Instance.mls.LogMessage("areItemsInShop for Host? " + EnemyLoot.Config.areItemsInShop.Value);

         if (EnemyLoot.Config.areItemsInShop.Value)
         {

            //TerminalNode node1 = ScriptableObject.CreateInstance<TerminalNode>();
            //node1.clearPreviousText = true;
            //node1.displayText = "Info test zu Case";
            //Items.RegisterShopItem(guiltyGearCase, null, null, node1, 0);

            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Can be used for teleportation";
            Items.RegisterShopItem(EnemyLoot.blackOrb, null, null, node, 800);
            //800



            TerminalNode node2 = ScriptableObject.CreateInstance<TerminalNode>();
            node2.clearPreviousText = true;
            node2.displayText = "Can be used to heal yourself";
            Items.RegisterShopItem(EnemyLoot.whiteOrb, null, null, node2, 200);
            //100

            TerminalNode node3 = ScriptableObject.CreateInstance<TerminalNode>();
            node3.clearPreviousText = true;
            node3.displayText = "Gives unlimited stamina and more walk speed";
            Items.RegisterShopItem(EnemyLoot.orangeOrb, null, null, node3, 300);
            //200

            TerminalNode node4 = ScriptableObject.CreateInstance<TerminalNode>();
            node4.clearPreviousText = true;
            node4.displayText = "Gives you semi-inviciblity";
            Items.RegisterShopItem(EnemyLoot.Spoon, null, null, node4, 900);
            //200

            TerminalNode node5 = ScriptableObject.CreateInstance<TerminalNode>();
            node5.clearPreviousText = true;
            node5.displayText = "@!&*@^*";
            Items.RegisterShopItem(EnemyLoot.WeirdHead, null, null, node5, 800);
            //200

            TerminalNode node6 = ScriptableObject.CreateInstance<TerminalNode>();
            node6.clearPreviousText = true;
            node6.displayText = "THE TOWN INSIDE ME";
            Items.RegisterShopItem(EnemyLoot.guiltyGearCase, null, null, node6, 50);
            //200
         }
      }
   }


   //[HarmonyPatch("ConnectClientToPlayerObject")]
   //[HarmonyPostfix]
   //static void loadShopClient()
   //{
   //    if (NetworkManager.Singleton.IsClient)
   //    {




   //        EnemyLoot.Instance.mls.LogMessage("areItemsInShop for Client? " + Config.Instance.areItemsInShop.Value);

   //        if (Config.Instance.areItemsInShop.Value)
   //        {
   //            //TerminalNode node1 = ScriptableObject.CreateInstance<TerminalNode>();
   //            //node1.clearPreviousText = true;
   //            //node1.displayText = "Info test zu Case";
   //            //Items.RegisterShopItem(guiltyGearCase, null, null, node1, 0);

   //            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
   //            node.clearPreviousText = true;
   //            node.displayText = "Info test zu orb";
   //            Items.RegisterShopItem(EnemyLoot.blackOrb, null, null, node, 0);
   //            //800

   //            TerminalNode node2 = ScriptableObject.CreateInstance<TerminalNode>();
   //            node2.clearPreviousText = true;
   //            node2.displayText = "Info test zu orb";
   //            Items.RegisterShopItem(EnemyLoot.whiteOrb, null, null, node2, 0);
   //            //100

   //            TerminalNode node3 = ScriptableObject.CreateInstance<TerminalNode>();
   //            node3.clearPreviousText = true;
   //            node3.displayText = "Info test zu orb";
   //            Items.RegisterShopItem(EnemyLoot.orangeOrb, null, null, node3, 0);
   //            //200
   //        }
   //    }
   //}
}



