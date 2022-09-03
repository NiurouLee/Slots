//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 15:24
//  Ver : 1.0.0
//  Description : UIFortuneChoose.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIFortuneChoose:MachinePopUp
    {
        private int nChooseIndex;
        [ComponentBinder("Root/MainGroup/Fortune1Button")]
        public Button btnFortuneBonus;

        [ComponentBinder("Root/MainGroup/Fortune2Button")]
        public Button btnFortuneFree;

        [ComponentBinder("Root/MainGroup/NoticeGroup/IntegralText")]
        public Text txtChips;
        
        private Action<int> _chooseAction;

        public UIFortuneChoose(Transform transform) :
            base(transform)
        {
            btnFortuneBonus.onClick.AddListener(OnBtnFortune1Click);
            btnFortuneFree.onClick.AddListener(OnBtnFortune2Click);
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var winRate = inContext.state.Get<ExtraState11011>().FreeGameWinRate;
            var totalWin = inContext.state.Get<BetState>().GetPayWinChips(winRate);
            txtChips.text = totalWin.GetCommaFormat();
        }

        public void SetChooseCallback(Action<int> chooseCallback)
        {
            _chooseAction = chooseCallback;
        }

        public  override async Task OnClose()
        {
            await XUtility.PlayAnimationAsync(animator, nChooseIndex == 0 ? "BonusClose" :"FreeGameClose");
            _chooseAction?.Invoke(nChooseIndex);
        }

        //[ComponentBinder("Root/MainGroup/NoticeGroup/IntegralText")]
        private void OnBtnFortune1Click()
        {
            btnFortuneBonus.interactable = false;
            btnFortuneFree.interactable = false;
            AudioUtil.Instance.PlayAudioFx("Feature_Selected");
            nChooseIndex = 0;
            Close();
        }
        //[ComponentBinder("Root/MainGroFortune2Button")]
        private void OnBtnFortune2Click()
        {
            btnFortuneBonus.interactable = false;
            btnFortuneFree.interactable = false;
            AudioUtil.Instance.PlayAudioFx("Feature_Selected");
            nChooseIndex = 1;
            Close();
        }
        
        public override bool IsCloseMustUnpauseMusic()
        {
            return true;
        }
    }
}