//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-10 15:07
//  Ver : 1.0.0
//  Description : BonusRapidHitPopUp11028.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class BonusRapidHitPopUp11028:MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/DayCountGroup/CountText")]
        private Text txtRapidHitCount;
        [ComponentBinder("Root/MainGroup/JPConut/CountText")]
        private Text txtMultiplier;
        [ComponentBinder("Root/MainGroup/IntegralText")]
        private Text txtTotalWinChips;
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;
        [ComponentBinder("Root/MainGroup/DayCountGroup/DayIcon")]
        protected Transform transDayIcon;
        [ComponentBinder("Root/MainGroup/DayCountGroup/NightIcon")]
        protected Transform transNightIcon;
        private bool isSettle;
        public BonusRapidHitPopUp11028(Transform transform) : base(transform)
        {
            collectButton.onClick.AddListener(async () =>
            {
                if (!isSettle)
                {
                    isSettle = true;
                    AudioUtil.Instance.PlayAudioFx("Close");
                    Close();
                }
            });
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("J01_Trigger_Open");
            base.OnOpen();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var extraState = context.state.Get<ExtraState11028>();
            txtMultiplier.gameObject.SetActive(extraState.IsMultiplierWheel);
            var winRate = extraState.NormalHit.WinRate;
            winRate += extraState.NormalHit.JackpotPay;
            if (extraState.IsMultiplierWheel)
            {
                winRate *= extraState.MultiplierWheelHit;
                txtMultiplier.text = "*"+extraState.MultiplierWheelHit;
            }
            var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
            txtTotalWinChips.SetText(chips.GetCommaFormat());
            transDayIcon.gameObject.SetActive(!extraState.IsNight);
            transNightIcon.gameObject.SetActive(extraState.IsNight);
            txtRapidHitCount.text = extraState.GetJackpotCount() > 0 ? extraState.GetJackpotCount().ToString() : "";
        }
    }
}