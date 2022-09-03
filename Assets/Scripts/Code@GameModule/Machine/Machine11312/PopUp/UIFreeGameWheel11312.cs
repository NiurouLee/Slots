using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class UIFreeGameWheel11312 : MachinePopUp
    {
        [ComponentBinder("AnimCenter")]
        protected Button AnimBtn;
        
        [ComponentBinder("WheelMainGroup")]
        protected Transform WheelMainGroup;

        private int angle;
        private bool IsHit=false;
        public UIFreeGameWheel11312(Transform transform) : base(transform)
        {
            if(AnimBtn){
                AnimBtn.onClick.AddListener(ClickAnimBtnCallBack);
            }
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            WheelMainGroup.transform.localRotation = Quaternion.Euler(0,0,0);
        }
        /// <summary>
        /// 设置轮盘旋转结果
        /// </summary>
        public void SetWheelRes(int index){
            angle = (index-1) * 30;
        }
        private async void ClickAnimBtnCallBack(){
            if(IsHit)return;
            IsHit = true;
            AudioUtil.Instance.PlayAudioFx("Wheel_ClickSpin");
            AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
            AnimBtn.interactable = false;
            var anim = transform.GetComponent<Animator>();
            anim.Play("BOpen");
            await context.WaitSeconds(2);
            WheelMainGroup.transform.localRotation = Quaternion.Euler(0,0,angle);
            await XUtility.PlayAnimationAsync(anim,"BStop");
            AudioUtil.Instance.PlayAudioFx("Wheel_Stop");
            await context.WaitSeconds(1.5f);
            
            Close();
            IsHit = false;
        }
    }
}   

