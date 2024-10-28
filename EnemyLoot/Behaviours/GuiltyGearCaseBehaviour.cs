using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyLoot.Behaviours
{
    internal class GuiltyGearCaseBehaviour : GrabbableObject
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
            //audioSource.clip = EnemyLoot.guiltyGearSFX;
            //audioSource.Play();
            audioSource.PlayOneShot(EnemyLoot.guiltyGearSFX);

            yield return new WaitForSeconds(3.5f);

            isTimerRunning = false;


        }

      public override void SetControlTipsForItem()
      {
         string[] toolTips =
            {
            "Drop Guilty Gear Case : [G]",
            "Play music : [LMB]"
            };

         HUDManager.Instance.ChangeControlTipMultiple(toolTips);
      }

      
   }
}
