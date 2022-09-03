//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-12 15:58
//  Ver : 1.0.0
//  Description : RapidHitPopUp11028.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class RapidHitPopUp11028:MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/DayCountGroup/CountText")]
        private Text txtRapidHitCount;
        [ComponentBinder("Root/MainGroup/IntegralText")]
        private Text txtTotalWinChips;
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;
        [ComponentBinder("Root/MainGroup/DayCountGroup/DayIcon")]
        protected Transform transDayIcon;
        [ComponentBinder("Root/MainGroup/DayCountGroup/NightIcon")]
        protected Transform transNightIcon;
        private bool isSettle;
        public RapidHitPopUp11028(Transform transform) : base(transform)
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

        public void InitializeJackpot(bool isNight, uint jackpotCount, ulong jackpotPay)
        {
            transDayIcon.gameObject.SetActive(!isNight);
            transNightIcon.gameObject.SetActive(isNight);
            txtRapidHitCount.text = jackpotCount.ToString();
            var chips = context.state.Get<BetState>().GetPayWinChips(jackpotPay);
            txtTotalWinChips.SetText(chips.GetCommaFormat());
        }
    }
}