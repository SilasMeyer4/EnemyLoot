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
        AudioSource audioSource;


        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            

            base.ItemActivate(used, buttonDown);
            if (buttonDown)
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
