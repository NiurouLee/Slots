using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class JackpotView11312 : JackPotPanel
    {
        private Transform JpMask;
        private Transform Jackpot_Unlock;
        private Vector3 normalScale = new Vector3(0.85f,0.85f,0);
        private float normalPosy = 3.29f;
        private Vector3 baseNormalScale = new Vector3(0.9f,0.9f,0);
        private float baseNormalPosy = 3.45f;
        private bool isHasHit=false;
        public JackpotView11312(Transform inTransform) : base(inTransform)
        {
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            JpMask = transform.Find("Level5Group/JP_mask");
            Jackpot_Unlock = transform.Find("Level5Group/Jackpot_Unlock");
            var mouseProxy = JpMask.gameObject.GetComponent<MonoMouseProxy>();
            if(mouseProxy==null)
                mouseProxy = JpMask.gameObject.AddComponent<MonoMouseProxy>();
            mouseProxy.BindingMouseUpAction(() =>
            {
                HitJackpotCallBack(5);
            });
        }
        public override void UpdateJackpotValue()
        {
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listTextJackpot[i])
                {
                    listTextJackpot[i].SetText(numJackpot.GetAbbreviationFormat(2,true));   
                }
            }
            for (int i = 0; i < listTextJackpotPro.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listTextJackpotPro[i])
                {
                    listTextJackpotPro[i].SetText(numJackpot.GetAbbreviationFormat(2,true));   
                }
            }
        }
        public void InitSetJackpotStatus(bool isUnLocked){
            if(!isUnLocked)
                JpMask.gameObject.SetActive(true);
            else
                JpMask.gameObject.SetActive(false);
        }
        /// <summary>
        /// 根据等级解锁相应的jackpotview状态
        /// </summary>
        /// <param name="unlockIndex"></param>
        public async void UpdateUnJackpotStatus(bool isUnLocked,bool isPlayAudio = true){
            if(!isUnLocked){
                JpMask.gameObject.SetActive(true);
            }
            else{
                if(JpMask.gameObject.activeSelf){
                    Jackpot_Unlock.GetComponent<Animator>().Play("Open");
                    AudioUtil.Instance.PlayAudioFx("Grand_Unlock");
                    await context.WaitSeconds(0.25f); 
                    JpMask.gameObject.SetActive(false);
                    isHasHit = false;
                }
            }   
        }
        /// <summary>
        /// 点击jackpot框解锁回调
        /// </summary>
        /// <param name="index"></param>
        private void HitJackpotCallBack(int index){
            var nextLogic = context.GetLogicStepProxy(LogicStepType.STEP_NEXT_SPIN_PREPARE) as NextSpinPrepareProxy11312;
            var isUnlocked = nextLogic.JudgeUnlockJackpotLevel();
            if(isUnlocked)
                return;

            var respin = context.state.Get<ReSpinState>();
            if(respin.NextIsReSpin)
                return;
            var freeSpin = context.state.Get<FreeSpinState>();
            if(freeSpin.NextIsFreeSpin)
                return;
            bool isAutoSpin = context.state.Get<AutoSpinState>().IsAutoSpin;
            if(isAutoSpin)
                return;
            var jackpotState = context.state.Get<JackpotInfoState>();
            var jackpotInfo = jackpotState.HasJackpotWin();
            if(jackpotInfo)
                return;
            if(isHasHit)
                return;
            isHasHit = true;
            nextLogic.HitJackpotUnlockTotalBet();
            UpdateUnJackpotStatus(true);
        }
        /// <summary>
        /// 调整jackpot缩放值
        /// </summary>
        public async void RefreshJackpotScale(uint level,bool isPlayAnim=false){
            Vector3 targetScale = normalScale;
            float targetPosy = normalPosy;
            if(level==5){
                targetScale = new Vector3(0.7f,0.7f,0);
                targetPosy = 3.25f;
            }
            else if(level == 6){
                targetScale = new Vector3(0.57f,0.57f,0);
                targetPosy = 3.21f;
            }
            else if(level == 7){
                targetScale = new Vector3(0.49f,0.49f,0);
                targetPosy = 3.18f;
            }

            float delayTime = 0;
            if(isPlayAnim){
                delayTime = 0.4f;
                await context.WaitSeconds(0.6f);
            }
                
            transform.DOScale(targetScale,delayTime).OnComplete(()=>{
                transform.localScale = targetScale;
            });
            transform.DOLocalMoveY(targetPosy,delayTime).OnComplete(()=>{
                transform.localPosition = new Vector3(0,targetPosy,0);
            });
            await context.WaitSeconds(delayTime);
        }

        public void SetJackpotBaseValue(){
            transform.localScale = baseNormalScale;
            transform.localPosition = new Vector3(0,baseNormalPosy,0);
        }
    }

}
