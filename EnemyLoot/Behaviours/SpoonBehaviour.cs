using GameNetcodeStuff;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace EnemyLoot.Behaviours
{
   internal class SpoonBehaviour : GrabbableObject
   {

      private bool _isTimerRunning = false;
      private int _activationCounter = 0;
      private AudioSource _audioSource;
      private PlayerControllerB _player;
      private int _healthBefore;
      private bool _isSpoonActive = false;
      private bool _isSpoonBeingHeld = false;

      public bool IsSpoonActive
      {
         get { return _isSpoonActive; }
         set { _isSpoonActive = value; }
      }

      
      public bool IsSpoonBeingHeld
      {
         get { return _isSpoonBeingHeld; }
         set { _isSpoonBeingHeld = value; }
      }

      //TODO need to fix icon maybe and add holdingPlayer usage. 

      public override void ItemActivate(bool used, bool buttonDown = true)
      {

         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {
       
            _player = playerHeldBy;

            if (!_isTimerRunning)
            {
               _activationCounter++;
               StartCoroutine(Activation());
            }
            else
            {
               _audioSource = gameObject.GetComponent<AudioSource>();
               _audioSource.clip = EnemyLoot.SpoonCDSFX;
               _audioSource.Play();
            }
         }
      }

      public override void EquipItem()
      {
         base.EquipItem();
         _isSpoonBeingHeld = true;

         if (IsSpoonActive)
         {
            _healthBefore = _player.health;
            _player.health = 100;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
         }
      }

      public override void DiscardItem()
      {
         base.DiscardItem();
         this._isSpoonBeingHeld = false;

         if (IsSpoonActive)
         {
            _player.health = _healthBefore;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
         }

      }

      public override void PocketItem()
      {
         base.PocketItem();
         _isSpoonBeingHeld = false;
         if (IsSpoonActive)
         {
            _player.health = _healthBefore;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
         }

      }

      public override void OnDestroy()
      {
         base.OnDestroy();
         _isSpoonBeingHeld = false;
      }

      public override void SetControlTipsForItem()
      {
         EnemyLoot.Instance.mls.LogInfo("Setting Controltips");
         string[] toolTips =
            {
            "Drop Spoon : [G]",
            "Activate Spoon : [LMB]"
            };

         if (_isSpoonActive)
         {
            toolTips[1] = "Spoon is active";
         } 
         else if (_isTimerRunning)
         {
            toolTips[1] = "Spoon on cooldown";
         }

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);
         
      }

      public PlayerControllerB GetHoldingPlayer()
      {
         return playerHeldBy;
      }

      private IEnumerator Activation()
      {
         _isTimerRunning = true;
         _audioSource = gameObject.GetComponent<AudioSource>();
         _audioSource.clip = EnemyLoot.SpoonActivationSFX;
         _audioSource.Play();
         _healthBefore = _player.health;
         

         if (_player != null)
         {
            _isSpoonActive = true;
            _player.health = 100;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
            //EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconActive;
            SetControlTipsForItem();
         }

         //EnemyLoot_SilasMeyer.EnemyLoot.Instance.isOrangeOrbActive = true;

         yield return new WaitForSeconds(10f);

         //EnemyLoot_SilasMeyer.EnemyLoot.Instance.isOrangeOrbActive = false;


         if (_player != null)
         {
            _isSpoonActive = false;
            _player.health = _healthBefore;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
           // EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconCD;
            SetControlTipsForItem();
         }

         yield return new WaitForSeconds(30f);

         _isTimerRunning = false;
         SetControlTipsForItem();
         //EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconBase;
     

      }

   }

   [HarmonyPatch(typeof(PlayerControllerB))]
   internal class SpoonDamagePatch
   {

      [HarmonyPatch("DamagePlayer")]
      [HarmonyPostfix]
      static void Patch(PlayerControllerB __instance, int damageNumber)
      {
         if (__instance != null && __instance.currentlyHeldObjectServer is GrabbableObject heldItem && (heldItem as SpoonBehaviour).IsSpoonActive)
         {
            __instance.health += (damageNumber);
            HUDManager.Instance.UpdateHealthUI(__instance.health, false);
         }
      }
   }
}
