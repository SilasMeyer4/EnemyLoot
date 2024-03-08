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
    internal class WhiteOrbBehaviour : PhysicsProp
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
                if (activationCounter >= 2)
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                    audioSource.clip = EnemyLoot.whiteOrbDestroySFX;
                    audioSource.Play();
                    return;
                }
                player = playerHeldBy;


                if (!isTimerRunning)
                {
                    if (player.health < 100)
                    {
                        activationCounter++;
                        StartCoroutine(heal());
                    } else
                    {
                        CoolDown();
                    }
                } else
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
                } else
                {
                    player.health += healAmount;
                }
                player.DamagePlayer(1);
                player.health += 1;
            }
         


            yield return new WaitForSeconds(2.8f);

            if (activationCounter >= 2)
            {
                audioSource = gameObject.GetComponent<AudioSource>();
                audioSource.clip = EnemyLoot.whiteOrbDestroySFX;
                audioSource.Play();

                yield return new WaitForSeconds(4.2f);
                   
            }

            isTimerRunning = false;


        }

    }
}