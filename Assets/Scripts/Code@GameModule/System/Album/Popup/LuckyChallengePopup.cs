// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/16:31
// Ver : 1.0.0
// Description : LuckyChallengePopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;


namespace GameModule 
{
    public class ChallengeRewardView : View
    {
        [ComponentBinder("LuckySpinNum")] public Text luckySpinNum;

        [ComponentBinder("Num")] public Text num;

        [ComponentBinder("Check")] public RectTransform check;

        public void SetUpRewardView(SGetCardAlbumInfo.Types.LuckyChallengeConfig challengeConfig, bool isClaimed)
        {
            num.text = challengeConfig.IndependentLuckyCardCount.ToString();
            luckySpinNum.text = "+" + challengeConfig.LuckySpinCount.ToString();
            check.gameObject.SetActive(isClaimed);
        }
    }
    
    
    [AssetAddress("UILuckyChallengePopup")]
    public class LuckyChallengePopup : Popup<LuckyChallengePopupViewController>
    {
        [ComponentBinder("Root/ProgressBarbg/ChallengeNode")]
        public RectTransform challengeNode;

        [ComponentBinder("Root/ProgressBarbg/ProgressBar")]
        public Image progressBar;

        [ComponentBinder("Root/ChallengeDescription/NumText")]
        public Text numText;    
       
        [ComponentBinder("Root/ProgressBarbg/LuckyCardCount")]
        public Text luckyCardCount; 
        
        [ComponentBinder("Root/KeepGoing")]
        public Button keepGoingButton;
        
        [ComponentBinder("Root")]
        public Transform root; 

        public List<ChallengeRewardView> challengeRewardViews;

        public LuckyChallengePopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            challengeRewardViews = new List<ChallengeRewardView>();
            base.OnViewSetUpped();

            AdaptScaleTransform(root, new Vector2(1300, 768));
        }
        
    }
    public class LuckyChallengePopupViewController: ViewController<LuckyChallengePopup>
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected LuckyChallengeUpdateInfo luckyChallengeUpdateInfo;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);

            if (inExtraData is PopupArgs popupArgs)
            {
                luckyChallengeUpdateInfo = popupArgs.extraArgs as LuckyChallengeUpdateInfo;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.keepGoingButton.onClick.AddListener(OnKeepGoingButtonClicked);
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }
        
        protected  void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();
        }

        public override void OnViewEnabled()
        {
            if (luckyChallengeUpdateInfo != null)
            {
                SetUpChallengeProgress((int)luckyChallengeUpdateInfo.oldProgress,(int)luckyChallengeUpdateInfo.newProgress);
            }
            else
            {
                SetUpChallengeProgress();
            }
        }

        public void OnKeepGoingButtonClicked()
        {
            EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
            view.Close();
        }
        
        public void SetUpChallengeProgress(int oldProgress = -1, int newProgress = -1)
        {
            var challengeConfigs = albumController.GetLuckyChallengeConfig();

            if (oldProgress < 0)
            {
                oldProgress = (int) albumController.GetLuckyChallengeProgress();
                newProgress = (int) albumController.GetLuckyChallengeProgress();
            }
            
            var maxProgress = (int) albumController.GetLuckyChallengeMaxProgress();

            var uiMaxWidth = view.challengeNode.sizeDelta.x;

            view.numText.text = maxProgress.ToString();
            
            view.luckyCardCount.text = $"{oldProgress.ToString()}/{maxProgress.ToString()}";

            List<int> canClaimViewIndexes = new List<int>();

            if (challengeConfigs != null && challengeConfigs.count > 0)
            {
                var rewardNode = view.challengeNode.Find("RewardNode");
                for (var i = 0; i < challengeConfigs.count; i++)
                {
                    if (i == challengeConfigs.count - 1)
                    {
                        view.challengeRewardViews.Add(view.AddChild<ChallengeRewardView>(rewardNode));
                    }
                    else
                    {
                        var rewardCopy = GameObject.Instantiate(rewardNode.gameObject, rewardNode.parent);
                        view.challengeRewardViews.Add(view.AddChild<ChallengeRewardView>(rewardCopy.transform));
                    }

                    bool isClaimed = challengeConfigs[i].IndependentLuckyCardCount <= oldProgress;
                    view.challengeRewardViews[i].SetUpRewardView(challengeConfigs[i], isClaimed);

                    if (challengeConfigs[i].IndependentLuckyCardCount <= oldProgress)
                    {
                        view.challengeRewardViews[i].transform.GetComponent<Animator>().Play("Normal");
                    }
                    else if (challengeConfigs[i].IndependentLuckyCardCount <= newProgress)
                    {
                        canClaimViewIndexes.Add(i);
                    }

                    var anchoredPosition = view.challengeRewardViews[i].rectTransform.anchoredPosition;
                    anchoredPosition.x =
                        (float) challengeConfigs[i].IndependentLuckyCardCount / maxProgress * uiMaxWidth;
                    view.challengeRewardViews[i].rectTransform.anchoredPosition = anchoredPosition;
                }
            }
            
            view.progressBar.fillAmount = (float) oldProgress / maxProgress;

            if (oldProgress != newProgress)
            {
                view.progressBar.DOFillAmount((float) newProgress / maxProgress, 1.0f).OnComplete(() =>
                {
                    view.luckyCardCount.text = $"{newProgress.ToString()}/{maxProgress.ToString()}";
                }).OnUpdate(() =>
                {
                    var currentProgress = view.progressBar.fillAmount * maxProgress;
                    if (canClaimViewIndexes.Count > 0)
                    {
                        for (var i = 0; i < canClaimViewIndexes.Count; i++)
                        {
                            if (challengeConfigs != null &&
                                challengeConfigs[canClaimViewIndexes[i]].IndependentLuckyCardCount <= currentProgress)
                            {
                                view.challengeRewardViews[canClaimViewIndexes[i]].transform.GetComponent<Animator>()
                                    .Play("Collect");
                                canClaimViewIndexes.Remove(i);
                                break;
                            }
                        }
                    }
                });
            }
        }
    }
}