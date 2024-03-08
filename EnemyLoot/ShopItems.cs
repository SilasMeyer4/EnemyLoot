using BepInEx.Configuration;
using CSync.Lib;
using CSync.Util;
using GameNetcodeStuff;
using HarmonyLib;
using LethalLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using EnemyLoot;
using System.Runtime.Serialization;
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
                EnemyLoot.Instance.mls.LogMessage("areItemsInShop for Host? " + Config.Instance.areItemsInShop.Value);

                if (Config.Instance.areItemsInShop.Value)
                {

                    //TerminalNode node1 = ScriptableObject.CreateInstance<TerminalNode>();
                    //node1.clearPreviousText = true;
                    //node1.displayText = "Info test zu Case";
                    //Items.RegisterShopItem(guiltyGearCase, null, null, node1, 0);

                    TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
                    node.clearPreviousText = true;
                    node.displayText = "Info test zu orb";
                    Items.RegisterShopItem(EnemyLoot.blackOrb, null, null, node, 0);
                    //800



                    TerminalNode node2 = ScriptableObject.CreateInstance<TerminalNode>();
                    node2.clearPreviousText = true;
                    node2.displayText = "Info test zu orb";
                    Items.RegisterShopItem(EnemyLoot.whiteOrb, null, null, node2, 0);
                    //100

                    TerminalNode node3 = ScriptableObject.CreateInstance<TerminalNode>();
                    node3.clearPreviousText = true;
                    node3.displayText = "Info test zu orb";
                    Items.RegisterShopItem(EnemyLoot.orangeOrb, null, null, node3, 0);
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



