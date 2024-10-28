using GameNetcodeStuff;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyLoot.Behaviours
{
   internal class OrangeOrbBehaviour : PhysicsProp
   {

      private bool isTimerRunning = false;
      private int activationCounter = 0;
      private float bonusMovementSpeed = 0;
      private AudioSource audioSource;
      private PlayerControllerB player;



      public override void ItemActivate(bool used, bool buttonDown = true)
      {

         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {
            player = playerHeldBy;
            bonusMovementSpeed = player.movementSpeed * 0.5f;

            if (!isTimerRunning)
            {
               activationCounter++;
               StartCoroutine(Activation());
            }
            else
            {
               audioSource = gameObject.GetComponent<AudioSource>();
               audioSource.clip = EnemyLoot.orangeOrbCDSFX;
               audioSource.Play();
            }
         }
      }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop Orange Orb : [G]",
            "Activate Orange Orb : [LMB]"
            };

         if (OrangeOrbSprintMeterPatch.IsOrangeOrbActive) //Need different condition
         {
            toolTips[1] = "Orange Orb is active";
         }
         else if (isTimerRunning)
         {
            toolTips[1] = "Orange on cooldown";
         }

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);

      }

      private IEnumerator Activation()
      {
         isTimerRunning = true;
         audioSource = gameObject.GetComponent<AudioSource>();
         audioSource.clip = EnemyLoot.orangeOrbActivationSFX;
         audioSource.Play();
         if (player != null)
         {
            player.movementSpeed += bonusMovementSpeed;
            player.sprintMeter = 1f;
            OrangeOrbSprintMeterPatch.IsOrangeOrbActive = true;

         }

         //Orb is active 

         SetControlTipsForItem();

         yield return new WaitForSeconds(15f);



         if (player != null)
         {
            player.movementSpeed -= bonusMovementSpeed;
            OrangeOrbSprintMeterPatch.IsOrangeOrbActive = false;
         }

         //Orb Cooldown

         SetControlTipsForItem();

         yield return new WaitForSeconds(45f);

         isTimerRunning = false;
      }


   }


   [HarmonyPatch(typeof(PlayerControllerB))]
   internal class OrangeOrbSprintMeterPatch
   {

      public static bool IsOrangeOrbActive = false;

      [HarmonyPatch("Update")]
      [HarmonyPostfix]
      static void infinite_Sprint_Patch(ref float ___sprintMeter)
      {
         if (IsOrangeOrbActive)
         {
            ___sprintMeter = 1f;
         }


      }
   }
}
