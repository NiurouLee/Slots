// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/27/16:34
// Ver : 1.0.0
// Description : SeasonQuestRewardNodeView.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
      public class SeasonQuestRewardNodeView : View<SeasonQuestRewardNodeViewController>
    {
        [ComponentBinder("IntegralText")] public TextMeshProUGUI integralText;

        [ComponentBinder("CollectStateButton")]
        public Button collectStateButton;
        
        [ComponentBinder("GiftGroup/CollectStateButton/Icon2")]
        public Button collectButton;

        [ComponentBinder("CloseStateButton")] public Button closeStateButton;

        [ComponentBinder("BubbleGroup")] public Transform bubbleGroup;
        
        [ComponentBinder("SkeletonGraphic (gold)")] public Transform spineNode;

        public Animator animator;

        public  PhrasedQuest quest;
        public  PhrasedQuest activeQuest;

        public int index;
        public int activeQuestIndex;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            animator = transform.GetComponent<Animator>();
        }

        public void SetUp(PhrasedQuest inQuest, PhrasedQuest inActiveQuest, int inIndex, int inActiveQuestIndex)
        {
            quest = inQuest;
            index = inIndex;
            activeQuest = inActiveQuest;
            activeQuestIndex = inActiveQuestIndex;
            HideRewardNode(false);
            
            UpdateAnimationState();
        }
        
        public void HideRewardNode(bool hide)
        {
            var giftGroup = transform.Find("GiftGroup");
            if (giftGroup)
                giftGroup.gameObject.SetActive(!hide);
        }
        
        public  bool NeedShowEnterTipReward()
        {
            if (Client.Get<SeasonQuestController>().NeedChooseDifficultyLevel())
                return false;
            
            return index == activeQuestIndex + 1 && activeQuest != null && !activeQuest.Collectable;
        }
        
        public void RefreshReward(PhrasedQuest inQuest)
        {
            quest = inQuest;
        }
        
        public  void SetUpItemReward()
        {
            var rewardRoot = bubbleGroup.Find("Root");
            
            for(var i = rewardRoot.childCount -1;i >= 0; i--)
            {
                var child = rewardRoot.GetChild(i);
                if(child.name.Contains("CommonCell") && child.name != "CommonCell") {
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            XItemUtility.InitItemsUI(rewardRoot, quest.Reward.Items);
        }
 
        public  bool CanShouldShowTip()
        {
            if ((activeQuest.Collectable
                 || activeQuest.Collected) && activeQuestIndex == index - 1)
            {
                return false;
            }

            return true;
        }
        

        public void UpdateAnimationState()
        {
            if (index < activeQuestIndex)
                animator.Play("QuestRewardCollected");

            else if (index == activeQuestIndex)
            {
                SetUpItemReward();
                animator.Play("QuestRewardCollectable");
            }
            else
            {
                SetUpItemReward();
                if (NeedShowEnterTipReward())
                {
                    viewController.ShowTip();
                }
                animator.Play("QuestRewardLock");
            }
        }
    }

    public class SeasonQuestRewardNodeViewController : ViewController<SeasonQuestRewardNodeView>
    {
        private Action<SeasonQuestRewardNodeView> _rewardCollectHandler;

        private bool _isTipShowing = false;
        private bool inSwitching = false;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.closeStateButton.onClick.AddListener(OnCloseStateButtonClicked);
           
            if (view.collectButton && view.collectStateButton)
            {
                view.collectButton.onClick.AddListener(OnRewardCollectClicked);
                view.collectStateButton.onClick.AddListener(OnRewardCollectClicked);
            }
        }

        public void BindCollectAction(Action<SeasonQuestRewardNodeView> collectHandler)
        {
            _rewardCollectHandler = collectHandler;
        }

        protected void OnRewardCollectClicked()
        {
            _rewardCollectHandler.Invoke(this.view);
        }
 
        protected void OnCloseStateButtonClicked()
        {
            // if (inSwitching)
            // {
            //     return;
            // }
            
            if (!_isTipShowing)
            {
                SoundController.PlaySfx("General_DropdownWindow");
                ShowTip();
            }
            else
            {
                HideTip();
            }
        }

        public void HideTip()
        {
            inSwitching = true;
            var animator = view.bubbleGroup.GetComponent<Animator>();
           
            XUtility.PlayAnimation(animator,"Close", () =>
            {
                _isTipShowing = false;
                inSwitching = false;
            });
        }
 
        public void ShowTip()
        {
            if (!view.CanShouldShowTip())
            {
                return;
            }
            
            if (!_isTipShowing)
            {
                inSwitching = true;
                var animator = view.bubbleGroup.GetComponent<Animator>();
                animator.Play("Open",-1, 0);
            
                WaitForSeconds(0.5f, () =>
                {
                    _isTipShowing = true;
                  //  inSwitching = false;
                });
            }
        }
    }
}