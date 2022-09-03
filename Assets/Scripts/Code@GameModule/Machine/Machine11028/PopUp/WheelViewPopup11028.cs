//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-10 14:57
//  Ver : 1.0.0
//  Description : WheelView11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelViewPopup11028:MachinePopUp
    {
        [ComponentBinder("Root")]
        private Transform transRoot;
        public Action WheelEndAction;
        private WheelView11028 wheelDay;
        private WheelView11028 wheelNight;
        private WheelView11028 wheelMultiplier;
        public WheelViewPopup11028(Transform transform) :
            base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            wheelDay = new WheelView11028(transRoot.Find("WheelMainDayGroup"),true);
            wheelNight = new WheelView11028(transRoot.Find("WheelMainNightGroup"),true);
            wheelMultiplier = new WheelView11028(transRoot.Find("WheelMainSmallGroup"),false);
        }

        public override async void OnOpen()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            wheelDay.PlayAnimation("Idle");
            wheelNight.PlayAnimation("Idle");
            wheelMultiplier.PlayAnimation("Idle");
            AudioUtil.Instance.UnPauseMusic();
            AudioUtil.Instance.PlayMusic("Wheel_BG");
        }

        public override async void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var showWheel = wheelDay;
            wheelDay.Initialize(inContext);
            wheelNight.Initialize(inContext);
            wheelMultiplier.Initialize(inContext);
            var extraState = context.state.Get<ExtraState11028>();
            wheelDay.transform.gameObject.SetActive(false);
            wheelNight.transform.gameObject.SetActive(false);
            if (extraState.IsNight)
            {
                showWheel = wheelNight;
            }

            showWheel.Show();
            showWheel.InitializeWheelView();
            showWheel.wheelEndAction = async () =>
            {
                await CheckAndStartBonusWheel(extraState);
                WheelEndAction?.Invoke();
            };
        }

        public void ReInitialize()
        {
            EnableButton();
            AudioUtil.Instance.UnPauseMusic();
            AudioUtil.Instance.PlayMusic("Wheel_BG");
            wheelDay.PlayAnimation("Idle");
            wheelNight.PlayAnimation("Idle");
            wheelMultiplier.PlayAnimation("Idle");
        }

        public void EnableButton(bool enable=true)
        {
            wheelDay.EnableButton(enable);
            wheelNight.EnableButton(enable);
        }

        private async Task CheckAndStartBonusWheel(ExtraState11028 extraState)
        {
            if (extraState.NormalHit.ToBonusWheel)
            {
                wheelMultiplier.Show();
                wheelMultiplier.InitializeWheelView();
                await wheelMultiplier.OnStartSpin();
            }  
        }

        public override void OnDestroy()
        {
            wheelDay.OnDestroy();
            wheelNight.OnDestroy();
            wheelMultiplier.OnDestroy();
            wheelDay = null;
            wheelNight = null;
            wheelMultiplier = null;
            base.OnDestroy();
        }
    }
}