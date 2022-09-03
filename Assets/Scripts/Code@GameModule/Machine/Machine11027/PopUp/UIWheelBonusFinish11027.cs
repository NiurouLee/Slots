using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIWheelBonusFinish11027 : MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        protected Text _textJackpotWinNum;

        [ComponentBinder("Root/BottomGroup/Button")]
        protected Button btnCollect;

        protected Animator animator;
        
        private bool isSettle = false;
        
        public UIWheelBonusFinish11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            // animator = transform.GetComponent<Animator>();
            
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
        }

        private void OnBtnCollectClick()
        {
            if (!isSettle)
            {
                isSettle = true;
                AudioUtil.Instance.PlayAudioFx("Close");
                Close();
            }
        }
    }
}
