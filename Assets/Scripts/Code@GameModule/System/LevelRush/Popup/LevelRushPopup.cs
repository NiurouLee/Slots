// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/09/15:15
// Ver : 1.0.0
// Description : LevelRushFailPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using SRF;
using Tool;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class RewardInfoBubble : View
    {
        public void InitBubbleContent(Reward reward)
        {
            var root = transform.Find("Root");

            //bubble代码复用，这里需要前做清除
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                if (root.GetChild(i).name != "BGGroup" && root.GetChild(i).name != "CommonCell")
                {
                    GameObject.Destroy(root.GetChild(i).gameObject);
                }
            }

            XItemUtility.InitItemsUI(root, reward.Items, root.Find("CommonCell"),
                XItemUtility.GetItemRewardSimplyDescText);
        }
    }


    public class RushNodeView : View
    {
        [ComponentBinder("Lock")] public Button lockState;

        [ComponentBinder("Finish")] public Transform finishState;

        [ComponentBinder("CommonExpandableRewardTip")]
        public Transform commonExpandableRewardTip;

        [ComponentBinder("LvText")] public Text lvText;

        public RewardInfoBubble bubble;

        protected Animator _animator;
        public bool claimed;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            if (commonExpandableRewardTip != null)
                bubble = AddChild<RewardInfoBubble>(commonExpandableRewardTip);

            _animator = transform.GetComponent<Animator>();
        }

        public void SetUpNodeInfo(LevelRushPopupInfo.Types.LevelRewardInfo levelRewardInfo, bool isNeedClaimNode)
        {
            if (levelRewardInfo != null)
            {
                lvText.text = "Lv" + levelRewardInfo.Level;

                claimed = levelRewardInfo.Received && !isNeedClaimNode;

                if (levelRewardInfo.Reward != null)
                {
                    bubble.InitBubbleContent(levelRewardInfo.Reward);
                }
            }
        }

        public void PlayOpenAnimation()
        {
            if (_animator)
            {
                if (claimed)
                    _animator.Play("FinishIdle");
                else
                {
                    _animator.Play("Open");
                }
            }
        }

        public async Task PlayFinishAnimation()
        {
            await XUtility.PlayAnimationAsync(_animator, "Finish");
            //_animator.Play("Finish");
        }

        public async Task PlayShowRewardAnimation()
        {
            await XUtility.PlayAnimationAsync(_animator, "Show");
        }
    }

    [AssetAddress("UILevelRushPopup", "UILevelRushPopupV")]
    public class LevelRushPopup : Popup<LevelRushPopupViewController>
    {
        [ComponentBinder("Root/NodeGroup/LottoBonusTips/Text")]
        public Text winUpToText;

        [ComponentBinder("TimeText")] public Text timeText;

        [ComponentBinder("TitleText")] public Text titleText;

        [ComponentBinder("Root/RushText")] public Transform transformRushText;
        
        [ComponentBinder("Root/Switch_Animator/Slider/Slider_1")] public Transform rushPassSlider;

        [ComponentBinder("NodeGroup")] public Transform nodeGroup;

        [ComponentBinder("Root/Switch_Animator/Slider/Button_RushPass")] public Button rushPassButton;

        [ComponentBinder("Root/Switch_Animator/RushText")] public Transform rushTextTrf;

        [ComponentBinder("Root/NodeGroup/FlyEff")]
        public Transform flyEff;

        [ComponentBinder("Root/Switch_Animator")]
        public Animator switchTitleAnimator;
        
        public List<RushNodeView> rushNodeViews;

        public LevelRushPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1300, 1300);
        }

        public override void Close()
        {
            base.Close();
            SoundController.RecoverLastMusic();
        }

        protected override void OnViewSetUpped()
        {
            var index = 1;

            rushNodeViews = new List<RushNodeView>();
            var node = nodeGroup.Find("Node" + index);

            while (node != null)
            {
                var nodeView = AddChild<RushNodeView>(node);
                rushNodeViews.Add(nodeView);

                index++;
                node = nodeGroup.Find("Node" + index);
            }

            base.OnViewSetUpped();
        }

        public override Vector3 CalculateScaleInfo()
        {
            if (contentDesignSize == Vector2.zero)
                return Vector3.one;

            var viewSize = ViewResolution.referenceResolutionLandscape;

            if (ViewManager.Instance.IsPortrait)
            {
                viewSize = ViewResolution.referenceResolutionPortrait;

                if (viewSize.y < contentDesignSize.y)
                {
                    var scale = viewSize.y / contentDesignSize.y;
                    return new Vector3(scale, scale, scale);
                }
            }
            else
            {
                if (viewSize.x < contentDesignSize.x)
                {
                    var scale = viewSize.x / contentDesignSize.x;
                    return new Vector3(scale, scale, scale);
                }
            }

            return Vector3.one;
        }
    }

    public class LevelRushPopupViewController : ViewController<LevelRushPopup>
    {
        protected LevelRushController _levelRushController;

        private bool waitForClaiming = false;

        private bool needShowDoubleXp = true;

        private Coroutine titleLoopCoroutine;      
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            var count = view.rushNodeViews.Count;

            for (int j = 0; j < count; j++)
            {
                view.rushNodeViews[j].PlayOpenAnimation();
            }

            CheckAndClaimReward();

            EnableUpdate(2);
            
            SoundController.PlayBgMusic("LevelRush_BGM");

            SetTitleAnim();
            
            Update();
        }

     

        public void CheckAndClaimReward()
        {
            var reward = _levelRushController.GetReward();

            if (reward != null)
            {
                view.closeButton.gameObject.SetActive(false);

                waitForClaiming = true;

                WaitForSeconds(1f, async () =>
                {
                    var rewardNodeIndex = _levelRushController.GetRewardNodeIndex();
                    if (rewardNodeIndex < view.rushNodeViews.Count)
                    {
                        await view.rushNodeViews[rewardNodeIndex].PlayShowRewardAnimation();
                        var activityRushPass =
                            Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as
                                Activity_LevelRushRushPass;
                        if (activityRushPass!=null &&(rewardNodeIndex == view.rushNodeViews.Count - 1))
                        {
                          await  RushPassScheduleAdd();
                        }
                    }
                    
                    var popup = await PopupStack.ShowPopup<UICommonGetRewardPopup>();
                    
                    if (popup != null)
                    {
                        var rewards = new RepeatedField<Reward>();
                        rewards.Add(reward);
                        popup.viewController.SetUpReward(rewards, "LevelRushReward", PlayClaimFinishAnimation);
                    }
                });
            }
        }

        public  async void PlayClaimFinishAnimation()
        {
            var rewardNodeIndex = _levelRushController.GetRewardNodeIndex();

            await view.rushNodeViews[rewardNodeIndex].PlayFinishAnimation();


            _levelRushController.OnRewardClaimFinished();

            needShowDoubleXp = false;
            waitForClaiming = false;

            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventLevelrushNodecomplete,
                ("index", rewardNodeIndex.ToString()));
            if (rewardNodeIndex == view.rushNodeViews.Count - 1)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                    .GameEventLevelrushComplete);

                if (_levelRushController.GetFreeLottoGameInfo() != null)
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LevelRushLottoPlayPopup),
                        view.performCategory)));
                }

                EventBus.Dispatch(new EventLevelRushStateChanged());
                view.Close();
            }
            else
            {
                view.closeButton.gameObject.SetActive(true);
            }
        }



        private void SetTitleAnim()
        {  var rewardNodeIndex = _levelRushController.GetRewardNodeIndex();
            var rushPassActivity =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            if (rushPassActivity != null&& rushPassActivity.IsUnlockState())
            {
                
                if (rewardNodeIndex == view.rushNodeViews.Count - 1)
                {
                    view.switchTitleAnimator.CrossFade("Slider",0);
                }
                else
                {
                    view.switchTitleAnimator.CrossFade("qiehuan",0);
                }
            }
            else
            {
                view.switchTitleAnimator.CrossFade("RushText",0);

            }
        }


        public override void OnViewDidLoad()
        {
            _levelRushController = Client.Get<LevelRushController>();
            base.OnViewDidLoad();

            var count = view.rushNodeViews.Count;
            var reward = _levelRushController.GetReward();
            var index = _levelRushController.GetRewardNodeIndex();


            for (int j = 0; j < count; j++)
            {
                view.rushNodeViews[j]
                    .SetUpNodeInfo(_levelRushController.GetRewardInfo(j), reward != null && j == index);
                view.rushNodeViews[j]
                    .SetUpNodeInfo(_levelRushController.GetRewardInfo(j), reward != null && j == index);
            }


            long freeWinUpTo = (long) _levelRushController.GetFreeWinUpTo();

            view.winUpToText.text = freeWinUpTo.GetCommaOrSimplify();

            view.titleText.text = _levelRushController.GetTargetLevel().ToString();

            SetUpRushPassEntrance();

           // Update();
        }

        public void SetUpRushPassEntrance()
        {
            //TODO if has Activity show ActivityEntrance and hide Text
            view.flyEff.gameObject.SetActive(false);
            var rushPassActivity =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            bool hasActivity = false;

            if (rushPassActivity != null)
            {
                hasActivity = rushPassActivity.IsUnlockState();
            }

            if (!hasActivity)
            {
                view.rushPassSlider.gameObject.SetActive(false);
            }
            else
            {
                view.rushPassSlider.gameObject.SetActive(true);
                var slider = view.rushPassSlider.GetComponent<Slider>();
                float s = ((float) rushPassActivity.CollectSchedule + 1) / 6;
                slider.value = s;
                var text = view.rushPassSlider.Find("Num").GetComponent<Text>();
                text.text = (rushPassActivity.CollectSchedule + 1).ToString() + "/6";
            }
        }
        // public void 

        
        public override void Update()
        {
            
            var leftTime = _levelRushController.GetLevelRushLeftTime();
            if (leftTime > 0)
            {
                if (view.transform)
                {
                    view.timeText.text = XUtility.GetTimeText(leftTime);
                }
            }
            else
            {
                DisableUpdate();

                if (!waitForClaiming)
                {
                    needShowDoubleXp = false;
                    view.Close();
                }
            }
        }

        public override void OnViewDestroy()
        {
            if (needShowDoubleXp)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LevelDoubleXpPopup), view.performCategory)));
                base.OnViewDestroy();
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.rushPassButton.onClick.AddListener(OnRushPassBtnClicked);
            SubscribeEvent<EventActivityExpire>(OnRushPassExpire);
            SubscribeEvent<EventActivityCreate>(OnRushPassCreat);
        }

        private void OnRushPassCreat(EventActivityCreate inActivityData)
        {
            if (inActivityData.activityType==ActivityType.RushPass)
            {
                view.rushPassSlider.gameObject.SetActive(true);
                view.switchTitleAnimator.CrossFade("qiehuan",0);
            }
        }


        private void OnRushPassExpire(EventActivityExpire inActivityData)
        {
            if (inActivityData.activityType==ActivityType.RushPass)
            {
                view.rushPassSlider.gameObject.SetActive(false);
                view.switchTitleAnimator.CrossFade("RushText",0);
            }
        }
        
        
        

        private void OnRushPassBtnClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRushpassEnter);
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(RushPassPopup))));
        }


        private async Task RushPassScheduleAdd()
        {
            //飞粒子
            view.flyEff.gameObject.SetActive(true);
            view.flyEff.DOMove(view.rushPassSlider.position, 0.5f);
            await WaitForSeconds(0.5f);
            var activityLevelRushPass =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            var slider = view.rushPassSlider.GetComponent<Slider>();
            slider.DOValue((activityLevelRushPass.CollectSchedule + 2)/6f, 1);
            var text = view.rushPassSlider.Find("Num").GetComponent<Text>();
            text.text = (activityLevelRushPass.CollectSchedule + 2).ToString() + "/6";
            //开启轮播
            await WaitForSeconds(1f);
            view.flyEff.gameObject.SetActive(false);
            await WaitForSeconds(1f);
                view.switchTitleAnimator.CrossFade("qiehuan",0);
        }
    }
}
