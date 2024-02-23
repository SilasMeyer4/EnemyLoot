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
        private bool isTimerRunning = false;
        private AudioSource audioSource;
        private PlayerControllerB player;
        private Vector3 playerShipTeleportPosition; 
        private Vector3 playerOrbTeleportPosition;


        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            player = playerHeldBy;


            base.ItemActivate(used, buttonDown);
            if (buttonDown)


                playerShipTeleportPosition = StartOfRound.Instance.playerSpawnPositions[0].position;
            {
                if (!isTimerRunning) 
                {
                    if (activationCounter < 2)
                    {
                        activationCounter++;
                        audioSource = gameObject.GetComponent<AudioSource>();
                        audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.blackOrbTeleportSFX;
                        audioSource.Play();
                        StartCoroutine(orbTeleport());

                    } else
                    {
                        StartCoroutine(orbCooldown());
                    }
                }
                else
                {

                    audioSource = gameObject.GetComponent<AudioSource>();
                    audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.blackOrbCDSFX;
                    audioSource.Play();


                }
            }
        }

        private IEnumerator orbTeleport()
        {
            isTimerRunning = true;
            yield return new WaitForSeconds(4.5f);
            //Teleport
           
            if (activationCounter == 1)
            {
                playerOrbTeleportPosition = player.transform.position;
                player.TeleportPlayer(playerShipTeleportPosition);
            } else
            {
                player.TeleportPlayer(playerOrbTeleportPosition);
                orbCooldown();
            }
           

            isTimerRunning = false;

            


        }

        private IEnumerator orbCooldown()
        {
            isTimerRunning = true;

            yield return new WaitForSeconds(60);

            activationCounter = 0;
            isTimerRunning = false;


        }
    }
   

}
