﻿using GameNetcodeStuff;
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


                } else
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                    audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.orangeOrbCDSFX;
                    audioSource.Play();
                }

            }
        }

 

        private IEnumerator Activation()
        {
            isTimerRunning = true;
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.orangeOrbActivationSFX;
            audioSource.Play();
            if (player != null)
            {
                player.movementSpeed += bonusMovementSpeed;
                player.sprintMeter = 1f;
            }

            yield return new WaitForSeconds(15f);
            

            if (player != null)
            {
                player.movementSpeed -= bonusMovementSpeed;       
            }

            yield return new WaitForSeconds(45f);

            isTimerRunning = false;


        }

    }
}