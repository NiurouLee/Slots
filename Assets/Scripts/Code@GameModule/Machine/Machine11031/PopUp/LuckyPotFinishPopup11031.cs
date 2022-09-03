// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/23/18:19
// Ver : 1.0.0
// Description : MiniGameFinishPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class LuckyPotFinishPopup11031 : MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button closeButton;

        [ComponentBinder("Root/MainGroup/IntegralBg/IntegralText")] 
        protected Text _textMeshWinNum;
        
        [ComponentBinder("Root/MainGroup/Text")] 
        protected Text _textFinalShow;

        public LuckyPotFinishPopup11031(Transform transform)
            : base(transform)
        {
            if (closeButton)
            {
                closeButton.onClick.AddListener(OnBtnCloseClick);
            }
        }

        public void SetWinNum(ulong finalWin)
        {
            Debug.LogError(finalWin + "AAAAAAAAA");
            _textMeshWinNum.SetText(finalWin.GetCommaFormat());
        }

        public void SetFinalText(string finalText)
        {
            _textFinalShow.SetText(finalText);
        }

        public override void OnOpen()
        {
            if (animator != null && animator.HasState("Open"))
                animator.Play("Open");
        }

        private bool isClosing = false;

        private void OnBtnCloseClick()
        {
            if (!isClosing)
            {
                AudioUtil.Instance.PlayAudioFx("Close");
                Close();
                isClosing = true;
            }
        }
    }
}