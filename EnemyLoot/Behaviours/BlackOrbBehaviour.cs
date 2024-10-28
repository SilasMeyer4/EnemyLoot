using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace EnemyLoot.Behaviours
{
   internal class BlackOrbBehaviour : PhysicsProp
   {

      private int activationCounter = 0;
      private bool _isTimerRunning = false;
      private AudioSource audioSource;
      private PlayerControllerB player;
      private Vector3 playerShipTeleportPosition;
      private Vector3 playerOrbTeleportPosition;
      internal EntranceTeleport[] EntranceArray;
      private bool _wasInsideBeforeTeleport;
      private bool _isSavedTeleportPositionInside;
      private int EntranceIndex;
      private const int ExitIndex = 0;


      public override void ItemActivate(bool used, bool buttonDown = true)
      {
         player = playerHeldBy;
         playerShipTeleportPosition = StartOfRound.Instance.playerSpawnPositions[0].position;
         EntranceArray = UnityEngine.Object.FindObjectsOfType<EntranceTeleport>(false);
         EntranceIndex = EntranceArray.Length / 2;

         base.ItemActivate(used, buttonDown);
         if (buttonDown)
         {

            if (!_isTimerRunning)
            {

               activationCounter++;
               StartCoroutine(orbTeleport());

            }
            else
            {

               audioSource = gameObject.GetComponent<AudioSource>();
               audioSource.clip = EnemyLoot.blackOrbCDSFX;
               audioSource.Play();

            }
         }
      }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop Black Orb : [G]",
            "Activate Black Orb : [LMB]"
            };

         if (_isTimerRunning)
         {
            toolTips[1] = "Black Orb on cooldown";
         }

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);

      }

      private IEnumerator orbTeleport()
      {
         _isTimerRunning = true;
         audioSource = gameObject.GetComponent<AudioSource>();
         audioSource.clip = EnemyLoot.blackOrbTeleportSFX;
         audioSource.Play();

         yield return new WaitForSeconds(4.5f);
         //Teleport

         _wasInsideBeforeTeleport = player.isInsideFactory;

         if (activationCounter == 1)
         {
            playerOrbTeleportPosition = player.transform.position;
            _isSavedTeleportPositionInside = _wasInsideBeforeTeleport;
            
            if (_wasInsideBeforeTeleport)
            {
               EntranceArray[ExitIndex].TeleportPlayer();
            } 

            player.TeleportPlayer(playerShipTeleportPosition);
         }
         else
         {
            //Checks if it needs to teleport Inside or Outside 
            if (_wasInsideBeforeTeleport && !_isSavedTeleportPositionInside)
            {
               EntranceArray[ExitIndex].TeleportPlayer();
            } 
            else if (!_wasInsideBeforeTeleport && _isSavedTeleportPositionInside)
            {
               EntranceArray[EntranceIndex].TeleportPlayer();
            }

            player.TeleportPlayer(playerOrbTeleportPosition);

            SetControlTipsForItem();

            yield return new WaitForSeconds(60);

            

            activationCounter = 0;
         }

         _isTimerRunning = false;

      }
   }


}
