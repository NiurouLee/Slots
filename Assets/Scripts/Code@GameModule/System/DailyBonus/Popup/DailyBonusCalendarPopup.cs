// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/23/15:05
// Ver : 1.0.0
// Description : DailyBonusPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class MothRewardInfoBubble : View
    {
        public void InitBubbleContent(Reward reward)
        {
            var root = transform.Find("Root");

            //bubble代码复用，这里需要前做清除
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                if (root.GetChild(i).name != "BGGroup" && root.GetChild(i).name != "DailyBonusCell")
                {
                    GameObject.Destroy(root.GetChild(i).gameObject);
                }
            }

            XItemUtility.InitItemsUI(root, reward.Items, root.Find("DailyBonusCell"), XItemUtility.GetItemRewardSimplyDescText);
        }
    }
    
    public class MonthRewardBoxCell : View
    {
        [ComponentBinder("OpenState")] 
        public Transform openStateTransform;
        [ComponentBinder("CloseState")] 
        public Transform closeStateTransform;

        [ComponentBinder("Root/CloseState/CountGroup/CountText")]
        public TextMeshProUGUI countText;

        [ComponentBinder("Root/CloseState")] 
        public Button button;
 
        public void InitializeBoxCell(MonthReward reward, int currentStep)
        {
            countText.text = reward.Step.ToString();

            if (reward.Step <= currentStep)
            {
               transform.GetComponent<Animator>().Play("OpenState");
            }
            else
            {
                transform.GetComponent<Animator>().Play("CloseState");
            }
        }
    }
    
    public class DailyRewardCell : View
    {
        [ComponentBinder("NormalState")] 
        public Transform normalStateTransform;
        [ComponentBinder("CanGetState")] 
        public Transform canGetStateTransform;
        [ComponentBinder("FinishState")] 
        public Transform finishStateTransform;

        public Animator animator;

        public void InitializeRewardCell(Reward reward, int dayIndex)
        {
            var rewardGroup = normalStateTransform.Find("RewardGroup");
            var canGetRewardGroup = canGetStateTransform.Find("RewardGroup");

            XItemUtility.InitItemsUI(rewardGroup, reward.Items, rewardGroup.Find("DailyBonusCell"), XItemUtility.GetItemRewardSimplyDescText);
            XItemUtility.InitItemsUI(canGetRewardGroup, reward.Items, canGetRewardGroup.Find("DailyBonusCell"), XItemUtility.GetItemRewardSimplyDescText, "StandardWithFXType");
 
            if (dayIndex == 6)
            {
                if (reward.Items.Count == 3)
                {
                    rewardGroup.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    canGetRewardGroup.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                }
                else
                {
                    rewardGroup.localScale = new Vector3(0.68f, 0.68f, 0.68f);
                    canGetRewardGroup.localScale = new Vector3(0.68f, 0.68f, 0.68f);
                }
            }
            
            UpdateDayText(dayIndex);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            animator = transform.GetComponent<Animator>();
        }

        public void ChangeStateToCollecting()
        {
            if(animator)
                animator.Play("CanGetState");
        }

        public void ChangeStateToNormal()
        {
            if(animator)
                animator.Play("NormalState");
        }
        
        public void ChangeStateToFinish(int dayIndex)
        {
            if(animator)
                animator.Play("FinishState");

            UpdateDayText(dayIndex);
        }

        private void UpdateDayText(int dayIndex)
        {
            var titleText = canGetStateTransform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            titleText.text = "DAY " + (dayIndex + 1);
            titleText = normalStateTransform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            titleText.text = "DAY " + (dayIndex + 1);
            titleText = finishStateTransform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            titleText.text = "DAY " + (dayIndex + 1);
        }
    }
    
    [AssetAddress("UIDailyBonusMain")]
    public class DailyBonusCalendarPopup : Popup<DailyBonusCalendarViewController>
    {
        [ComponentBinder("TimerText")] public TextMeshProUGUI timeText;

        [ComponentBinder("DayText")] public TextMeshProUGUI dayText;

        [ComponentBinder("DaysGroup")] public Transform dayGroup;

        [ComponentBinder("BoxGroup")] public Transform boxGroup;
        
        [ComponentBinder("RewardBubble")] public Transform  rewardBubble;
        
        [ComponentBinder("TimerGroup")] public Animator  timerGroupAnimator;
        
        [ComponentBinder("NoticeGroup")] public Animator  noticeGroupAnimator;

        [ComponentBinder("ProgressBar")]
        public Slider monthRewardProgressBar;

        [ComponentBinder("Root/MainGroup/Pen")] 
        public Transform pen;

        [ComponentBinder("ProgressUpdateFx")]
        public Transform progressUpdateFx;
 
        public List<DailyRewardCell> dailyRewardCells;
 
        public List<MonthRewardBoxCell> monthRewardBoxCells;

        public MothRewardInfoBubble bubbleView;


        public DailyBonusCalendarPopup(string address)
            : base(address)
        {
        }

        
        protected override void OnViewSetUpped()
        {
            monthRewardBoxCells = new List<MonthRewardBoxCell>(4);
            for (var i = 0; i < boxGroup.childCount; i++)
            {
                monthRewardBoxCells.Add(AddChild<MonthRewardBoxCell>(boxGroup.GetChild(i)));
            }

            dailyRewardCells = new List<DailyRewardCell>(7);
            for (var i = 0; i < dayGroup.childCount; i++)
            {
                dailyRewardCells.Add(AddChild<DailyRewardCell>(dayGroup.GetChild(i)));
            }

            bubbleView = AddChild<MothRewardInfoBubble>(rewardBubble);
            
            bubbleView.transform.gameObject.SetActive(false);
            
            base.OnViewSetUpped();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyBonusPop);
        }

        public override Vector3 CalculateScaleInfo()
        {
            if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                var scale = (float) ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                return Vector3.one * scale;
            }

            return Vector3.one;
        }
    }

    public class DailyBonusCalendarViewController : ViewController<DailyBonusCalendarPopup>
    {

        private int _totalDay = 32;

        private bool _normalShow = false;
        
        public Action checkMonthRewardAction;
        
        public override void OnViewEnabled()
        {
            InitializeMonthRewardBox();
            
            InitializeDailyRewardCellInfo();
            
            BindClickEvent();
            
            EnableUpdate(2);


            if (_normalShow)
            {
                view.timerGroupAnimator.Play("Idle");
                view.noticeGroupAnimator.Play("Idle");
            }
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            
            if(inExtraData != null)
                _normalShow = (bool)(inExtraData);
        }

        protected void BindClickEvent()
        {
            var stage = Client.Get<DailyBonusController>().GetMonthStage();
            
            for (var i = 0; i < view.monthRewardBoxCells.Count; i++)
            {
                if(i < stage) 
                    continue;
                
                var index = i;
                view.monthRewardBoxCells[i].button.onClick.AddListener(() => { OnRewardBoxClicked(index); });
            }
        }
        
        protected void InitializeMonthRewardBox()
        {
            var dailyBonusController = Client.Get<DailyBonusController>();
            var monthStep = (int) Client.Get<DailyBonusController>().GetMonthStep() -  ((_normalShow) ? 0 : 1);
            
            view.dayText.text = monthStep + "/" + 30;
            view.monthRewardProgressBar.value = (float) monthStep / _totalDay;

            var deltaSize = ((RectTransform) view.boxGroup.parent).sizeDelta;

            for (var i = 0; i < view.monthRewardBoxCells.Count; i++)
            {
                var monthReward = dailyBonusController.GetMonthRewardInfo(i);
               
                if (monthReward != null)
                {
                    var x = (monthReward.Step / (float) _totalDay - 1) * deltaSize.x;

                    view.monthRewardBoxCells[i]
                        .InitializeBoxCell(dailyBonusController.GetMonthRewardInfo(i), monthStep);
                    var anchoredPosition = ((RectTransform) view.monthRewardBoxCells[i].transform).anchoredPosition;

                    ((RectTransform) view.monthRewardBoxCells[i].transform).anchoredPosition =
                        new Vector2(x, anchoredPosition.y);
                }
                else
                {
                    view.monthRewardBoxCells[i].transform.gameObject.SetActive(false);
                }
            }
        }


        public override void Update()
        {
            var leftTime = Client.Get<DailyBonusController>().GetLeftTime();
            if (leftTime > 0)
            {
                view.timeText.text = TimeSpan.FromSeconds(leftTime).ToString(@"hh\:mm\:ss");
            }
            else
            {
                view.timeText.text = "00:00:00";
                view.Close();
            }
        }

        protected void InitializeDailyRewardCellInfo()
        {
            var dailyBonusController = Client.Get<DailyBonusController>();
            var weekStep = Client.Get<DailyBonusController>().GetWeekSignStep();

            for (var i = 0; i < view.dailyRewardCells.Count; i++)
            {
                if (i < weekStep)
                {
                    view.dailyRewardCells[i].ChangeStateToFinish(i);
                }
                else
                {
                    if (i == weekStep)
                    {
                        if (_normalShow)
                        {
                            view.dailyRewardCells[i].ChangeStateToFinish(i); 
                        }
                        else
                        {
                            view.dailyRewardCells[i].ChangeStateToCollecting();
                        }
                    }
                    else
                    {
                        view.dailyRewardCells[i].ChangeStateToNormal();
                    }

                    view.dailyRewardCells[i].InitializeRewardCell(dailyBonusController.GetDailyRewardInfo(i), i);
                }
            }
        }

        protected void OnRewardBoxClicked(int index)
        {
            if (view.bubbleView.transform.gameObject.activeSelf)
                return;
            
            view.bubbleView.transform.position = view.monthRewardBoxCells[index].countText.transform.position;
          
            view.bubbleView.InitBubbleContent(Client.Get<DailyBonusController>().GetMonthRewardInfo(index).Reward);
           
            XUtility.ShowTipAndAutoHide(view.bubbleView.transform,3,0.2f,true,this);
        }
        
        public async void ShowCheckAnimation()
        {
            view.closeButton.interactable = false;
            
            //FlyPen To The correct Day;
            var dailyBonusController = Client.Get<DailyBonusController>();

            var signInDay = dailyBonusController.GetWeekSignStep() ;    
            
            var animator = view.pen.GetComponent<Animator>();

            await PlayAnimationAsync(animator, "Open");
            
            view.dailyRewardCells[signInDay].transform.SetAsLastSibling();
            
            var dayAnimator = view.dailyRewardCells[signInDay].transform.GetComponent<Animator>();
           
            
            PlayAnimation(animator, $"DAY{signInDay+1}_Open");
 
            SoundController.PlaySfx("SignIn_Signature-02");
            await PlayAnimationAsync(dayAnimator,"SignIn");
            await PlayAnimationAsync(animator, $"DAY{signInDay+1}_Close");
            
            view.timerGroupAnimator.Play("Open");
            view.noticeGroupAnimator.Play("Open");
 
            UpdateMonthProgressAndCheckClaimMonthReward();
        }

        public void SubscribeCheckMonthRewardAction(Action action)
        {
            checkMonthRewardAction = action;
        }
        
        protected void UpdateMonthProgressAndCheckClaimMonthReward()
        {
            var dailyBonusController = Client.Get<DailyBonusController>();
          
            var monthStep = Client.Get<DailyBonusController>().GetMonthStep();
            var fillAmount = (float) monthStep / _totalDay;

            view.dayText.text = monthStep + "/" + 30;
            view.progressUpdateFx.gameObject.SetActive(true);
            
            SoundController.PlaySfx("General_ExpUp");
            
            view.monthRewardProgressBar.DOValue(fillAmount, 0.5f).OnComplete(async () =>
            {
                if (dailyBonusController.CheckHasMonthRewardToClaim())
                {
                    var monthBox = view.monthRewardBoxCells[dailyBonusController.GetMonthStage()];
                    var animator = monthBox.transform.GetComponent<Animator>();
                   
                    await PlayAnimationAsync(animator, "Open");
                    
                    checkMonthRewardAction?.Invoke();
                    
                    WaitForSeconds(1, () =>
                    {
                         view.Close();
                    });
                }
                else
                {
                    view.closeButton.interactable = true;
                }
            });
        }
    }
}