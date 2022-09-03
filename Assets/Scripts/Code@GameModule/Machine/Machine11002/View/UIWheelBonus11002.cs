using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using TMPro;
using Tool;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIWheelBonus11002 : MachinePopUp
    {
        [ComponentBinder("Root/UIJackpotPanel/Level4Group/Text")]
        private Text txtGrand;

        [ComponentBinder("Root/UIJackpotPanel/Level3Group/Text")]
        private Text txtMajor;

        [ComponentBinder("Root/UIJackpotPanel/Level2Group/Text")]
        private Text txtMinor;

        [ComponentBinder("Root/UIJackpotPanel/Level1Group/Text")]
        private Text txtMini;

        [ComponentBinder("Root/WheelMainGroup/Root/FGMask")]
        private Transform tranFGMask;

        [ComponentBinder("Root/WheelMainGroup/Root/CenterGroup/WheelBonusButton")]
        private Button btnWheelBonus;

        [ComponentBinder("Root/WheelMainGroup/Root/MainGroup")]
        private Transform tranMainGroup;

        [ComponentBinder("Root/WheelMainGroup")]
        private Animator animatorWheel;

        private float angleOne = 0;

        public UIWheelBonus11002(Transform inTransform) : base(inTransform)
        {
            btnWheelBonus.onClick.AddListener(OnBtnWheelBonus);
            angleOne = 360f / 18;
        }

        private Coroutine coroAnim;

        private async void OnBtnWheelBonus()
        {
            btnWheelBonus.interactable = false;

            AudioUtil.Instance.StopAudioFx("Wheel_Wait");
            AudioUtil.Instance.PlayAudioFx("Wheel_Press");

            var extraState = context.state.Get<ExtraState11002>();
            /*
            var extraInfo = extraState.GetExtraInfo();
            if (extraInfo != null && extraInfo.WheelBonusInfo != null && extraInfo.WheelBonusInfo.Chosen == false)
            {
                var sBonusProcess = await extraState.SendBonusProcess();
                extraInfo = ProtocolUtils.GetAnyStruct<WinsOPlentyGameResultExtraInfo>(sBonusProcess.GameResult.ExtraInfoPb);
            }
            */
            AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
            await XUtility.PlayAnimationAsync(animatorWheel, "Start");

            animatorWheel.Play("Loop");

            await XUtility.WaitSeconds(1.5f, context);

            //------Add by Yongchuan--------
            var extraInfo = extraState.GetExtraInfo();
            //------------------------------

            var wheelBonusInfo = extraInfo.WheelBonusInfo;
            var index = wheelBonusInfo.Choice;
            float angle = angleOne * index;
            tranMainGroup.localEulerAngles = new Vector3(0, 0, angle);

            await XUtility.PlayAnimationAsync(animatorWheel, "Finish");
            AudioUtil.Instance.PlayAudioFx("Wheel_Stop");
            await XUtility.PlayAnimationAsync(animatorWheel, "Win");
            animatorWheel.Play("Winidle");
            await XUtility.WaitSeconds(2, context);

            //------Add by Yongchuan--------
            if (extraInfo != null && extraInfo.WheelBonusInfo != null && extraInfo.WheelBonusInfo.Chosen == false)
            {
                var sBonusProcess = await extraState.SendBonusProcess();
                extraInfo = ProtocolUtils.GetAnyStruct<WinsOPlentyGameResultExtraInfo>(sBonusProcess.GameResult.ExtraInfoPb);
            }
            //------------------------------

            if (wheelBonusInfo != null && wheelBonusInfo.Settled == false)
            {
                await extraState.SettleBonusProgress();
                var sSettleProcess = extraState.sSettleProcess;
                actionCallback?.Invoke(sSettleProcess.GameResult, extraInfo);
            }
        }


        private Action<GameResult, WinsOPlentyGameResultExtraInfo> actionCallback;
        public void SetShowCallback(Action<GameResult, WinsOPlentyGameResultExtraInfo> action)
        {
            actionCallback = action;
        }

        private JackpotInfoState jackpotInfoState;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            //------Add by Yongchuan--------
            UpdateJackpotLockedValue();
            //------------------------------

            //jackpotInfoState = context.state.Get<JackpotInfoState>();

            //EnableUpdate();
        }


        public override async void OnOpen()
        {
            // base.OnOpen();
            AudioUtil.Instance.PlayAudioFx("Wheel_Appear");
            AudioUtil.Instance.PlayAudioFx("Wheel_Wait", true);

            await XUtility.PlayAnimationAsync(animator, "Open");
            animatorWheel.Play("Idle");
        }

        private ulong numGrand;
        private ulong numMajor;
        private ulong numMinor;
        private ulong numMini;
        /*
        public virtual void UpdateJackpotValue()
        {
            numMini = jackpotInfoState.GetJackpotValue(1);
            numMinor = jackpotInfoState.GetJackpotValue(2);
            numMajor = jackpotInfoState.GetJackpotValue(3);
            numGrand = jackpotInfoState.GetJackpotValue(4);

            txtMini.text = numMini.GetCommaFormat();
            txtMinor.text = numMinor.GetCommaFormat();
            txtMajor.text = numMajor.GetCommaFormat();
            txtGrand.text = numGrand.GetCommaFormat();
        }
        */

        //------Add by Yongchuan--------
        public void UpdateJackpotLockedValue()
        {
            jackpotInfoState = context.state.Get<JackpotInfoState>();
            numMini = jackpotInfoState.GetJackpotValue(1);
            numMinor = jackpotInfoState.GetJackpotValue(2);
            numMajor = jackpotInfoState.GetJackpotValue(3);
            numGrand = jackpotInfoState.GetJackpotValue(4);
            
            txtMini.text = numMini.GetCommaFormat();
            txtMinor.text = numMinor.GetCommaFormat();
            txtMajor.text = numMajor.GetCommaFormat();
            txtGrand.text = numGrand.GetCommaFormat();
        }
        //-------------------------------
    }
}
