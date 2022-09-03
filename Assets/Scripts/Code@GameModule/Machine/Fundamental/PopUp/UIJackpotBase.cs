using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIJackpotBase : MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        protected TextMeshProUGUI _textMeshProUGUIJackpotWinNum;
        
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        protected Text _textJackpotWinNum;

        [ComponentBinder("Root/BottomGroup/Button")]
        protected Button btnCollect;

        protected Animator animator;
        
        public UIJackpotBase(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            animator = transform.GetComponent<Animator>();
            
            if (btnCollect)
            {
                btnCollect.onClick.AddListener(OnBtnCollectClick); 
            }
            else
            {
                AutoClose();
            }
        }

        public virtual async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            Close();
        }

        public virtual void SetJackpotWinNum(ulong jackpotWin)
        {
            if (_textJackpotWinNum)
            {
                _textJackpotWinNum.SetText(jackpotWin.GetCommaFormat());
            }

            if (_textMeshProUGUIJackpotWinNum)
            {
                _textMeshProUGUIJackpotWinNum.SetText(jackpotWin.GetCommaFormat());
            }
        }

        private bool isClosing = false;

        private void OnBtnCollectClick()
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
