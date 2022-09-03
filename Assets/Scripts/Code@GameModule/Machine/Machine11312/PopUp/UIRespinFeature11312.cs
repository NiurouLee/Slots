using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class UIRespinFeature11312 : MachinePopUp
    {
        [ComponentBinder("CloseButton")]
        protected Button CloseButton;
        // [ComponentBinder("click_mask")]
        // protected Button click_mask;
        private bool IsHit=false;
        public UIRespinFeature11312(Transform transform) : base(transform)
        {
            if(CloseButton){
                CloseButton.onClick.AddListener(()=>{
                    ClickBtnCallBack();
                });
            }
            // if(click_mask){
            //     click_mask.onClick.AddListener(()=>{
            //         ClickBtnCallBack();
            //     });
            // }
            
        }
        public override void OnOpen()
        {
            base.OnOpen();
            PopUpManager.Instance.SetGrayMaskState(false);
            context.view.Get<Background11312>().UpdateMaskShow(true);
        }
        private void ClickBtnCallBack(){
            if(IsHit)return;
            IsHit = true;
            CloseButton.interactable = false;
            AudioUtil.Instance.PlayAudioFx("Close");
            Constant11312.UIRespinFeature = null;
            Close();
        }
        public override async Task OnClose()
        {
            if (animator && animator.HasState("Close"))
                await XUtility.PlayAnimationAsync(animator, "Close");
            IsHit = false;
            context.view.Get<Background11312>().UpdateMaskShow(false);
        }
        public override float GetPopUpMaskAlpha()
        {
            return 0f;
        }
    }
}

