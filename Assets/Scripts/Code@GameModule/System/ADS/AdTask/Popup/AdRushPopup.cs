// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/11:19
// Ver : 1.0.0
// Description : AdRushPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule
{
    public class AdTaskView : View
    {
        [ComponentBinder("BG0")] 
        public Transform bg0;

        [ComponentBinder("BG0/Mask_lizi")] 
        public Transform maskLizi1;
        
        [ComponentBinder("BG0/Mask_lizi")] 
        public Transform maskLizi2;

        [ComponentBinder("BG1")]
        public Transform bg1;
        
        [ComponentBinder("RewardGroup")] 
        public Transform rewardGroup;

        [ComponentBinder("ProgressBar")]
        public Slider progressBar;
 
        [ComponentBinder("NumberGroup")] 
        public Transform numberGroup; 
        
        [ComponentBinder("StateGroup/FinishState")] 
        public Transform finishState;
     
        [ComponentBinder("NumberGroup/CountText")] 
        public TMP_Text indexText;

        public int taskIndex;

        public void SetUpTaskView(Reward reward, int index, int claimedStep, int totalCount)
        {
            XItemUtility.InitItemUI(rewardGroup, reward.Items[0]);

            taskIndex = index;
            indexText.text = (index + 1).ToString();
            
            finishState.gameObject.SetActive(claimedStep > index);

            if (claimedStep > index)
            {
                maskLizi1.gameObject.SetActive(false);
                maskLizi2.gameObject.SetActive(false);
            }
            else if (index == 4 || index == 9)
            {
                maskLizi1.gameObject.SetActive(true);
                maskLizi2.gameObject.SetActive(true);
            }

            progressBar.value = index < claimedStep ? 1:0;

            if (index == totalCount - 1)
            {
                progressBar.gameObject.SetActive(false);
            }

            if (index == 4 || index == 9)
            {
                bg0.gameObject.SetActive(false);
                bg1.gameObject.SetActive(true);
            }
            else
            {
                bg0.gameObject.SetActive(true);
                bg1.gameObject.SetActive(false);
            }
        }

        public void RefreshUI(int claimedStep)
        {
            if (taskIndex == claimedStep - 1)
            {
                finishState.gameObject.SetActive(true);
                
                maskLizi1.gameObject.SetActive(false);
                maskLizi2.gameObject.SetActive(false);
               
                if (progressBar.gameObject.activeSelf)
                {
                    progressBar.DOValue(1, 0.5f);
                }
            }
        }
    }
    
    [AssetAddress("UIADTaskMain")]
    public class AdRushPopup:Popup<AdRushPopupViewController>
    {
        [ComponentBinder("Root/Scroll View/Viewport/Content")]
        public Transform adTaskContent;  
        
        [ComponentBinder("Root/Scroll View")]
        public ScrollRect scrollView;

        [ComponentBinder("Root/TimerGroup/TimerText")]
        public TMP_Text timerText;  
        
        [ComponentBinder("Root/WatchButton")]
        public Button watchButton;

        public List<AdTaskView> taskViewList;

        public AdRushPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1150,768);
        }

        protected override void OnViewSetUpped()
        {
            var template = adTaskContent.Find("TaskMainCell");
            template.gameObject.SetActive(false);
            
            base.OnViewSetUpped();
        }

        public AdTaskView AddTaskNode(Reward reward, int index, int claimedStep, int totalCount)
        {
            var taskNodeTemplate = adTaskContent.Find("TaskMainCell");

            var taskGameObject = GameObject.Instantiate(taskNodeTemplate.gameObject, adTaskContent);
            taskGameObject.gameObject.SetActive(true);
            
            var taskView = AddChild<AdTaskView>(taskGameObject.transform);

            if (taskViewList == null)
            {
                taskViewList = new List<AdTaskView>();
            }

            taskViewList.Add(taskView);

            taskView.SetUpTaskView(reward, index, claimedStep, totalCount);
            return taskView;
        }

        public void RemoveAllTaskNodeView()
        {
            var taskNodeCount = taskViewList.Count;
            for (var i = taskNodeCount - 1; i >= 0; i--)
            {
                taskViewList[taskNodeCount].Destroy();
            }
            
            taskViewList.Clear();
        }
        
        public AdTaskView GetAddTaskView(int index)
        {
            if(index < taskViewList.Count)
                return taskViewList[index];
            return null;
        }
    }
    
    public class AdRushPopupViewController:ViewController<AdRushPopup>
    {
        private SGetRVAdvertisingConfig.Types.AdTaskInfo _adTaskInfo;

        protected bool rvCallbackProcessed = false;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.watchButton.onClick.AddListener(OnWatchAdClicked);
        }
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
         
            EnableUpdate(2);

            SetUpAdRushTaskView();
            Update();
        }

        protected void SetUpAdRushTaskView()
        {
            _adTaskInfo = AdController.Instance.GetAdTaskInfo();
             
            var adTaskCount = _adTaskInfo.Rewards.Count;

            for (var i = 0; i < adTaskCount; i++)
            {
                view.AddTaskNode(_adTaskInfo.Rewards[i], i, (int) _adTaskInfo.TaskStep, adTaskCount);
            }

            if (adTaskCount == _adTaskInfo.TaskStep)
            {
                view.watchButton.interactable = false;
            }
            
            view.scrollView.horizontalNormalizedPosition = _adTaskInfo.TaskStep/(float)adTaskCount;
        }

        protected void OnWatchAdClicked()
        {
            view.watchButton.interactable = false;
           
            if (AdController.Instance.ShouldShowRV(eAdReward.AdTask, false))
            {
                rvCallbackProcessed = false;
                
                AdController.Instance.TryShowRewardedVideo(eAdReward.AdTask, OnRvWatchEnd);
                AdController.Instance.enableAdConfigRefresh = false;
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                view.watchButton.interactable = true; 
            }
        }
        
        protected void RefreshAllAdTaskUI()
        {
            view.RemoveAllTaskNodeView();

            SetUpAdRushTaskView();
           
            if(!updateEnabled)
                EnableUpdate(2);
        }

        protected async void OnRvWatchEnd(bool isSuccess, string reason)
        {
            if (rvCallbackProcessed)
            {
                return;
            }
            
            rvCallbackProcessed = true;
            
            if (isSuccess)
            {
                var claimResult = await AdController.Instance.ClaimRvReward(eAdReward.AdTask);

                if (claimResult != null)
                {
                    _adTaskInfo = AdController.Instance.GetAdTaskInfo();

                    var claimTaskNode = view.GetAddTaskView((int) _adTaskInfo.TaskStep);

                    _adTaskInfo.TaskStep++;

                    if (_adTaskInfo.TaskStep >= 10)
                    {
                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCompleteAdrush10Ads);
                    }

                    var claimPopup = await PopupStack.ShowPopup<AdRewardClaimPopup>();

                    claimPopup.viewController.SetUpClaimUI(claimResult, eAdReward.AdTask, () =>
                    {
                        if(view.transform != null)
                             claimTaskNode.RefreshUI((int) _adTaskInfo.TaskStep);
                        
                        EventBus.Dispatch(new EventAdTaskStepUpdated());
                        ResetAdRushState();
                        
                    });
                }
                else
                {
                    ResetAdRushState();
                    XDebug.Log("ClaimFailed");
                }
            }
            else
            {
                XDebug.Log($"AdRushPopup:OnRvWatchEnd:isSuccess{isSuccess}|reason{reason}");
                ResetAdRushState();
            }
        }

        protected void ResetAdRushState()
        {
            if (view.transform != null)
            {
                _adTaskInfo = AdController.Instance.GetAdTaskInfo();
                view.watchButton.interactable = _adTaskInfo.TaskStep < _adTaskInfo.Rewards.Count;
            }
            
            AdController.Instance.enableAdConfigRefresh = true;
            
            CheckRefreshAdTaskConfig();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAdTaskConfigRefreshed>(OnAdTaskConfigRefreshed);
        }

        protected void OnAdTaskConfigRefreshed(EventAdTaskConfigRefreshed evt)
        {
            RefreshAllAdTaskUI();
        }
       
        public async void CheckRefreshAdTaskConfig()
        {
            var refreshLeftTime = AdController.Instance.GetAdTaskRefreshLeftTime();

            if (refreshLeftTime <= 0)
            {
                await AdController.Instance.RefreshAdLogicConfig();
            }
        }

        public override void Update()
        {
            var refreshLeftTime = AdController.Instance.GetAdTaskRefreshLeftTime();

            if (refreshLeftTime > 0)
            {
                view.timerText.text = XUtility.GetTimeText(refreshLeftTime);
            }
            else
            {
                view.watchButton.interactable = false;
                view.timerText.text = "00:00:00";
            }
        }
    }
}