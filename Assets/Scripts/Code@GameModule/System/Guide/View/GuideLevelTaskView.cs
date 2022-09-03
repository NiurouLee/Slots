//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-20 14:38
//  Ver : 1.0.0
//  Description : LevelGuideTaskView.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class GuideLevelTaskView:View<GuideLevelTaskViewController>
    {
        public Animator Animator;
        [ComponentBinder("Root/CharacterIcon")] 
        public Transform transCharacter;
        [ComponentBinder("Root/ProgressBar")] 
        public Slider ProgressBar;
        [ComponentBinder("Root/ProgressBar/Fill Area")] 
        public Transform transCoin;
        [ComponentBinder("Root/ProgressBar/Fill Area/UIParticle_jindu")] 
        public Transform transProgressEffect;
        

        [ComponentBinder("Root/DialogGroupL")]
        public Animator AnimatorTipsL;
        
        [ComponentBinder("Root/DialogGroupR")]
        public Animator AnimatorTipsR;

        [ComponentBinder("Root/WellDone")] 
        public Transform TransWellDone;

        [ComponentBinder("Root/ContentGroup")]
        public Transform TransContent;
        [ComponentBinder("Root/ContentGroup/CountText")]
        public Text txtCount;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            Animator = transform.GetComponent<Animator>();
        }
        
    }

    public class GuideLevelTaskViewController : ViewController<GuideLevelTaskView>
    {
        public bool IsFinish;
        public string StrGuide;
        public bool needShowMaxBetTip;
        private GuideLevelTaskWellDoneView viewWellDone;


        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            viewWellDone = view.AddChild<GuideLevelTaskWellDoneView>(view.TransWellDone);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.Guide);
            SubscribeEvent<EventUpdateExp>(OnUpdateUserExp);
        }
        
        private async void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (IsCurrentGuide() && !IsFinish)
            {
                if (IsSpinTask())
                {
                    await Client.Get<GuideController>().UpdateGuide(Guide, Guide.Step+1);
                    await UpdateProgress(Guide.Step*1f/Guide.TotalStep);   
                }

                if (needShowMaxBetTip)
                {
                    needShowMaxBetTip = false;
                    EventBus.Dispatch(new EventGuideShowMaxBetTip());
                }
                CheckAndSetTaskFinish();
            }
            handleEndCallback.Invoke();   
        }
        
        private async void OnUpdateUserExp(EventUpdateExp evt)
        {
            if (IsProgressFull()) return;
            if (IsCurrentGuide() && !IsSpinTask())
            {
                if (evt.updateToFull)
                {
                    if (Client.Get<UserController>().GetUserLevel() == 2)
                    {
                        needShowMaxBetTip = true;
                    }
                    await Client.Get<GuideController>().UpdateGuide(Guide, Guide.Step+1);
                    await UpdateProgress(Guide.Step*1f/Guide.TotalStep);
                }
            }
        }

        public void UpdateDirection(bool isLeft)
        {
            if (view.AnimatorTipsL)
            {
                PlayTipsOpen(view.AnimatorTipsL, isLeft);
            }
            if (view.AnimatorTipsR)
            {
                PlayTipsOpen(view.AnimatorTipsR, !isLeft);
            }
            if (viewWellDone != null)
            {
                viewWellDone.viewController.UpdateDirection(isLeft);   
            }
            view.transCharacter.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        }

        private void PlayTipsOpen(Animator animator, bool isLeft)
        {
            view.SetTransformActive(animator.transform, isLeft);
            XUtility.PlayAnimation(animator, "Open", null, this);
            XUtility.WaitSeconds(3, new CancelableCallback(() =>
            {
                if (animator != null)
                    XUtility.PlayAnimation(animator, "Close", null, this);
            }));
        }

        public void InitGuide(string strGuide)
        {
            StrGuide = strGuide;
            if (view.transform)
            {
                view.transform.gameObject.SetActive(IsCurrentGuide());
                if (IsCurrentGuide())
                {
                    InitProgressBar();
                    viewWellDone.viewController.InitReward(Guide);
                    XUtility.PlayAnimation(view.Animator,Guide.Completed?"Finish":"Idle",null,this);
                    CheckAndSetTaskFinish();
                }

                if (IsSpinTask())
                {
                    view.txtCount.text = Guide.TotalStep.ToString();
                }
            }
        }

        public void InitProgressBar()
        {
            var value = 0f;
            if (IsSpinTask())
            {
                value = Guide.Step*1f / Guide.TotalStep;
            }
            else
            {
                if (IsLevelTaskFinish())
                {
                    value = 1;
                }
                else
                {
                    value = Guide.Step*1f / Guide.TotalStep; 
                }
            }
            UpdateProgress(value,true);  
        }

        public async Task UpdateProgress(float value, bool init=false)
        {
            if (init)
            {
                view.ProgressBar.value = value;
                view.transProgressEffect.rotation = Quaternion.Euler(0, 0, -value * 360);
            }
            else
            {
                view.ProgressBar.DOValue(value,0.3f);
                view.transProgressEffect.DORotateQuaternion(Quaternion.Euler(0, 0, -value * 360), 0.3f);
            }
            await XUtility.WaitSeconds(0.31f, this);
            if (IsProgressFull())
            {
                XUtility.PlayAnimation(view.Animator,"Finish",null,this);
            }
        }

        private void CheckAndSetTaskFinish()
        {
            if (IsProgressFull())
            {
                IsFinish = true;
                var reward = Guide.Reward;
                var type = Guide.Type;
                //TODO ShowFlyFX
                EventBus.Dispatch(new EventGuideFinished(Guide, async () =>
                {
                    var coinItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Coin);
                    if (coinItem != null)
                    {
                        await XUtility.FlyCoins(view.transCoin.transform,
                            new EventBalanceUpdate((long) coinItem.Coin.Amount, type));
                    }
                }));
            }
        }

        private bool IsProgressFull()
        {
            return Mathf.Abs(view.ProgressBar.value - 1f) < float.Epsilon;
        }

        private bool IsCurrentGuide()
        {
            return Guide != null && 
                (Guide == Client.Get<GuideController>().GetCurrentGuide() || 
                Client.Get<GuideController>().GetEnterMachineGuide() != null && IsSpinTask());
        }

        private bool IsSpinTask()
        {
            return Guide != null && Guide.Type.Contains("Times");
        }
        private bool IsLevelTaskFinish()
        {
            var level = Client.Get<UserController>().GetUserLevel();
            if (Guide != null && (Guide.Type == "ReachLevel3" && level >= 3 ||
                                  Guide.Type == "ReachLevel4" && level >= 4))
                return true;
            return false;
        }

        private Guide Guide => Client.Get<GuideController>().GetGuideByName(StrGuide);
    }
}