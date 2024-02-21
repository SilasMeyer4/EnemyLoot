using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyLoot.Behaviours
{
    internal class GuiltyGearCaseBehaviour : PhysicsProp
    {

        public override void ItemActivate(bool used, bool buttonDown = true)
        {

            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = EnemyLoot_SilasMeyer.EnemyLoot.newAudio;

            EnemyLoot_SilasMeyer.EnemyLoot.Instance.mls.LogMessage(audioSource.clip.ToString());

            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {

                audioSource.Play();
               
                
             
            }
        }
    }
}
