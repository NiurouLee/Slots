//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-27 11:15
//  Ver : 1.0.0
//  Description : BonusMultiPopUp11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class BonusMultiPopUp11007: ReSpinStartPopUp
    {
        private Animator _animator;
        [ComponentBinder("Root/Bet10")] private Transform transBet10;
        [ComponentBinder("Root/Bet15")] private Transform transBet15;
        [ComponentBinder("Root/Bet20")] private Transform transBet20;
        [ComponentBinder("Root/Bet25")] private Transform transBet25;
        [ComponentBinder("Root/Bet30")] private Transform transBet30;
        public BonusMultiPopUp11007(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animator = transform.GetComponent<Animator>();
        }

        public  async Task InitializeWith(MachineContext context, int multiply, bool hasPlayMulti)
        {
            this.context = context;

            int index = 1;
            for (int i = hasPlayMulti ? multiply : 10; i <= multiply; i+=5)
            {
                XUtility.PlayAnimation(animator, "B" + i);
                context.view.Get<LinkBonusTitle11007>().UpdateBonusMultiply((uint)i);
                AudioUtil.Instance.PlayAudioFx("Respin_Multiplier_"+index++);
                await context.WaitSeconds(1.8f);
            }
            await Task.CompletedTask;
        }
        
        
    }
}