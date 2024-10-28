using GameNetcodeStuff;
using LethalLib.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

namespace EnemyLoot.Behaviours
{
   internal class WhiteOrbBehaviour : GrabbableObject
   {

      private bool isTimerRunning = false;
      private int activationCounter = 0;
      private AudioSource audioSource;
      private PlayerControllerB player;
      private int healAmount = 30;

      public override void ItemActivate(bool used, bool buttonDown = true)
      {


         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {
            player = playerHeldBy;
            if (activationCounter >= 2)
            {
               audioSource = gameObject.GetComponent<AudioSource>();
               audioSource.clip = EnemyLoot.whiteOrbDestroySFX;
               
               audioSource.Play();
               this.DestroyObjectInHand(playerHeldBy);
               return;
            }
            


            if (!isTimerRunning)
            {
               if (player.health < 100)
               {
                  activationCounter++;
                  StartCoroutine(heal());
               }
               else
               {
                  CoolDown();
               }
            }
            else
            {
               CoolDown();
            }

         }
      }

      private void CoolDown()
      {
         audioSource = gameObject.GetComponent<AudioSource>();
         audioSource.clip = EnemyLoot.whiteOrbCDSFX;
         audioSource.Play();
      }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop White Orb : [G]",
            "Activate White Orb : [LMB]"
            };

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);

      }


      private IEnumerator heal()
      {
         isTimerRunning = true;
         audioSource = gameObject.GetComponent<AudioSource>();
         audioSource.clip = EnemyLoot.whiteOrbActivationSFX;
         audioSource.Play();

         yield return new WaitForSeconds(2.2f);

         if (player != null)
         {


            if (player.health + healAmount > 100)
            {
               player.health = 100;
            }
            else
            {
               player.health += healAmount;
            }
            HUDManager.Instance.UpdateHealthUI(player.health, false);
         }



         yield return new WaitForSeconds(2.8f);

         if (activationCounter >= 2)
         {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot.whiteOrbDestroySFX;
            audioSource.Play();

            yield return new WaitForSeconds(4.2f);

            this.DestroyObjectInHand(playerHeldBy);

         }

         isTimerRunning = false;


      }

   }
}