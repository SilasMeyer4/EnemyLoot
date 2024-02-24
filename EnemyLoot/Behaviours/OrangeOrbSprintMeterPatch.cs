using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyLoot.Behaviours
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class OrangeOrbSprintMeterPatch
    {


        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infinite_Sprint_Patch(ref float ___sprintMeter) 
        {
            if (EnemyLoot_SilasMeyer.EnemyLoot.Instance.isOrangeOrbActive)
            {
                ___sprintMeter = 1f;
            }
           
  
        }
    }
}