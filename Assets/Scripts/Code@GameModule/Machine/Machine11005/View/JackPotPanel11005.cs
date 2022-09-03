using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class JackPotPanel11005: JackPotPanel
    {

        protected List<string> listAnimName = new List<string>()
        {
            "Minor","Major","Grand"
        };

        protected Animator animator;
        
        public JackPotPanel11005(Transform inTransform) : base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
        }



        public void PlayJackpotAnim(uint jackpotId)
        {
            XUtility.PlayAnimationAsync(animator, listAnimName[(int)jackpotId],context);
        }

        public void CloseJackpotAnim()
        {
            XUtility.PlayAnimationAsync(animator, "Close",context);

        }
    }
}