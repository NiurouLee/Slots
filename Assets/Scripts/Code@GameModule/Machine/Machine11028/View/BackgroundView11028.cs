//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-19 11:09
//  Ver : 1.0.0
//  Description : BackgroundView11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class BackgroundView11028: TransformHolder
    {
        private Animator _animator;
        private Animator _animatorBg;
        public BackgroundView11028(Transform inTransform) : base(inTransform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            _animator = transform.Find("Wheels").GetComponent<Animator>();
            _animatorBg = transform.Find("Background").GetComponent<Animator>();
        }

        public void PlayBackgroundAnimation(string animName)
        {
            XUtility.PlayAnimation(_animator, animName);
        }

        public void UpdateFreeBackground(bool isFree, bool isNight)
        {
            if (isFree)
            {
                XUtility.PlayAnimation(_animatorBg,isNight ? "Night" : "Day");
            }
            else
            {
                XUtility.PlayAnimation(_animatorBg, "Idle"); 
            }
        }
    }
}