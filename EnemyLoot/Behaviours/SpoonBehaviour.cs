using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
      public bool _isHoldingSpoon;

      //TODO need to fix icon maybe and add holdingPlayer usage. 

      public override void ItemActivate(bool used, bool buttonDown = true)
      {

         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {
            _player = playerHeldBy;
            SpoonPatch.IsPlayerHoldingSpoon = true;

            if (!_isTimerRunning)
            {
               _activationCounter++;
               StartCoroutine(Activation());
            }
            else
            {
               _audioSource = gameObject.GetComponent<AudioSource>();
               _audioSource.clip = EnemyLoot.SpoonCDSFX; ;
               _audioSource.Play();
            }
         }
      }

      //public override void PocketItem()
      //{
      //   base.PocketItem();
      //   SpoonPatch.IsPlayerHoldingSpoon = false;
      //   _player.health = _healthBefore;
      //   HUDManager.Instance.UpdateHealthUI(_player.health, false);
      //}

      //public override void DiscardItem()
      //{
      //   base.DiscardItem();
      //   if (SpoonPatch.IsSpoonActive)
      //   {
      //      _healthBefore = _player.health;
      //      _player.health = 100;
      //      HUDManager.Instance.UpdateHealthUI(_player.health, false);
      //   }
      //}

      public override void EquipItem()
      {
         base.EquipItem();
         SpoonPatch.IsPlayerHoldingSpoon = true;
         if (SpoonPatch.IsSpoonActive)
         {
            _healthBefore = _player.health;
            _player.health = 100;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
         }

      }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop Spoon : [G]",
            "Activate Spoon : [LMB]"
            };

         if (SpoonPatch.IsSpoonActive)
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
            SpoonPatch.IsSpoonActive = true;
            SpoonPatch.Player = _player;
            SpoonPatch.IsPlayerHoldingSpoon = true;
            _player.health = 100;
            EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconActive;
            SetControlTipsForItem();
         }

         //EnemyLoot_SilasMeyer.EnemyLoot.Instance.isOrangeOrbActive = true;

         yield return new WaitForSeconds(10f);

         //EnemyLoot_SilasMeyer.EnemyLoot.Instance.isOrangeOrbActive = false;


         if (_player != null)
         {
            SpoonPatch.IsSpoonActive = false;
            SpoonPatch.Player = null;
            _player.health = _healthBefore;
            HUDManager.Instance.UpdateHealthUI(_player.health, false);
            EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconCD;
            SetControlTipsForItem();
         }

         yield return new WaitForSeconds(30f);

         _isTimerRunning = false;
         SetControlTipsForItem();
         EnemyLoot.ScriptSpoon.itemProperties.itemIcon = EnemyLoot.IconBase;
     

      }

   }

   [HarmonyPatch(typeof(PlayerControllerB))]
   internal class SpoonPatch
   {
      public static bool IsSpoonActive = false;
      public static PlayerControllerB Player = null;
      public static bool IsPlayerHoldingSpoon = false;

      [HarmonyPatch("DamagePlayer")]
      [HarmonyPostfix]
      static void Patch(int damageNumber)
      {
         if (IsSpoonActive && IsPlayerHoldingSpoon && Player != null)
         {
            Player.health += (damageNumber);
            HUDManager.Instance.UpdateHealthUI(Player.health, false);
         }
      }
   }


}
