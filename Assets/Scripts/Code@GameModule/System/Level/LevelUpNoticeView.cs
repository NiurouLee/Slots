// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/19/21:16
// Ver : 1.0.0
// Description : LevelUpNoticeView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    
    [AssetAddress("UILevelUpNotice")]
    public class LevelUpNoticeView:View
    {
        [ComponentBinder("LevelNumberText")] 
        public TextMeshProUGUI levelNumberText;    
        
        [ComponentBinder("Icon")] 
        public Transform coinIcon; 
        
        [ComponentBinder("IntegralText")] 
        public TextMeshProUGUI integralText;   
        
        [ComponentBinder("VIPPointText")] 
        public TextMeshProUGUI vipPointText; 
        
        [ComponentBinder("MaxBetIntegralText")] 
        public TextMeshProUGUI maxBetIntegralText;  
        
        [ComponentBinder("MaxBetGroup")] 
        public Transform maxBetGroup; 
        
        [ComponentBinder("BubbleGroup")] 
        public Transform bubbleGroup;
        
        [ComponentBinder("NumberText")] 
        public TextMeshProUGUI multiplierText;
        
        [ComponentBinder("BottomGroup")] 
        public Transform bottomGroup;    
        
        [ComponentBinder("ShareButton")] 
        public Button shareButton;
        
        [ComponentBinder("Root")] 
        public RectTransform rootNode;

        public Vector2 horizontalStartPosition = new Vector2(320, 170);
        public Vector2 horizontalStopPosition = new Vector2(320, -70);
        
        public Vector2 verticalStartPosition = new Vector2(230, 170);
        public Vector2 verticalStopPosition = new Vector2(230, -55);
        public Vector3 verticalScale = new Vector3(0.8f,0.8f,0.8f);

        public Vector2 startPosition;
        public Vector2 stopPosition;

        public LevelUpInfo levelUpInfo;
        
        public LevelUpNoticeView(string address)
            :base(address)
        {
            
        }
        
        public void ShowNoticeView(LevelUpInfo inLevelUpInfo, bool isVertical)
        {
            levelUpInfo = inLevelUpInfo;
            
            levelNumberText.text = $"LEVEL {levelUpInfo.Level}";
            long rewardCoinCount =
                (long) XItemUtility.GetItem(levelUpInfo.RewardItems, Item.Types.Type.Coin).Coin.Amount;
            integralText.text = rewardCoinCount.GetCommaOrSimplify(9);

            var itemVip = XItemUtility.GetItem(levelUpInfo.RewardItems, Item.Types.Type.VipPoints);
          
            if (itemVip != null)
            {
                long vipPointCount = itemVip.VipPoints.Amount;
                vipPointText.text = vipPointCount.GetCommaFormat();
            }

            if (levelUpInfo.MaxBet > 0)
            {
                maxBetGroup.gameObject.SetActive(true);
                maxBetIntegralText.text = ((long) levelUpInfo.MaxBet).GetCommaOrSimplify(9);
                maxBetGroup.gameObject.SetActive(true);
            }
            else
            {
                maxBetGroup.gameObject.SetActive(false);
            }

            if (levelUpInfo.RewardMultiplier > 1)
            {
                bubbleGroup.gameObject.SetActive(true);
                multiplierText.text = "X" + levelUpInfo.RewardMultiplier;
            }
            else
            {
                bubbleGroup.gameObject.SetActive(false);
            }
            
            bottomGroup.gameObject.SetActive(false);
            
            ShowNoticeAnimation(isVertical);
        }
        
        private void ShowNoticeAnimation(bool isVertical)
        {
            if (isVertical)
            {
                startPosition = verticalStartPosition;
                stopPosition = verticalStopPosition;
                rootNode.localScale = verticalScale;
            }
            else
            {
                startPosition = horizontalStartPosition;
                stopPosition = horizontalStopPosition;
                rootNode.localScale = Vector3.one;
            }

            rootNode.anchoredPosition = startPosition;
            
            DOTween.To(() => rootNode.anchoredPosition, 
                (x => rootNode.anchoredPosition = x),
                    stopPosition, 0.5f).SetEase(Ease.InBack).OnComplete(
                    async () =>
                    {
                        long rewardCoinCount =
                            (long) XItemUtility.GetItem(levelUpInfo.RewardItems, Item.Types.Type.Coin).Coin.Amount;
                        await XUtility.FlyCoins(coinIcon, new EventBalanceUpdate(rewardCoinCount, "LevelUp"),null,false);

                        HideNoticeView(levelUpInfo);
                    });
        }

        private void HideNoticeView(LevelUpInfo inLevelUpInfo)
        {
            DOTween.To(() => rootNode.anchoredPosition,
                (x => rootNode.anchoredPosition = x),
                startPosition, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    EventBus.Dispatch(new EventUpdateExp(false));
                    if (inLevelUpInfo != null && inLevelUpInfo.UnlockedMachines.Count > 0)
                    {
                        EventBus.Dispatch(new EventLevelUpUnlockNewMachine(inLevelUpInfo));
                    }

                    if (inLevelUpInfo != null)
                    {
                        var vipItem = XItemUtility.GetItem(inLevelUpInfo.RewardItems, Item.Types.Type.VipPoints);
                        if (vipItem != null && vipItem.VipPoints.LevelUpRewardItems != null)
                        {
                            ItemSettleHelper.SettleItem(vipItem, null, "LevelUp");
                        }
                        
                        if (inLevelUpInfo.MaxBet > 0)
                        {
                            EventBus.Dispatch(new EventMaxBetUnlocked(inLevelUpInfo.MaxBet));
                        }
                    }
                }
            );
        }
    }
}