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
    internal class WhiteOrbBehaviour : PhysicsProp
    {

        private bool isTimerRunning = false;
        private int activationCounter = 0;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {

            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
           

                if (!isTimerRunning)
                {
                    activationCounter++;
                    StartCoroutine(heal());


                }

            }
        }

        private IEnumerator heal()
        {
            isTimerRunning = true;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.whiteOrbHealingSFX;
            audioSource.Play();
            if (playerHeldBy != null)
            {
                playerHeldBy.health += 20;
            }
         


            yield return new WaitForSeconds(5f);

            if (activationCounter >= 2)
            {
                Destroy(gameObject);
            }

            isTimerRunning = false;


        }

    }
}