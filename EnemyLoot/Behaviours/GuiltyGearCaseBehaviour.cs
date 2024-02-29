using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyLoot.Behaviours
{
    internal class GuiltyGearCaseBehaviour : PhysicsProp
    {

        private bool isTimerRunning = false;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {

            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                if (!isTimerRunning)
                {
                    StartCoroutine(play());


                }

            }
        }

        private IEnumerator play()
        {
            isTimerRunning = true;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.guiltyGearSFX;
            audioSource.Play();

            yield return new WaitForSeconds(3.5f);

            isTimerRunning = false;


        }

    }
}
