// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/13:36
// Ver : 1.0.0
// Description : WheelBonusCollectView.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class WheelBonusCollectView:View
    {
        [ComponentBinder("DescriptionText")]
        public TextMeshProUGUI descriptionText;

        [ComponentBinder("VipDescriptionText")]
        public TextMeshProUGUI vipDescriptionText;
         
        [ComponentBinder("WheelIntegralText")]
        public TextMeshProUGUI wheelIntegralText; 
        
        [ComponentBinder("TotalIntegralGroup")]
        public RectTransform totalIntegralGroup;
     
        [ComponentBinder("BottomGroup")]
        public Transform vipGroup;
 
        [ComponentBinder("CoinIntegralText")]
        public TextMeshProUGUI coinIntegralText;
        
        [ComponentBinder("VIPIntegralText")]
        public TextMeshProUGUI vipIntegralText; 
        
        [ComponentBinder("DiamondIntegralText")]
        public TextMeshProUGUI diamondIntegralText;
        
        [ComponentBinder("CollectButton")]
        public Button collectButton;

        [ComponentBinder("BoostBuff")] 
        public Transform boostBuff; 
         
        [ComponentBinder("BoostBuffFx")] 
        public Transform boostBuffFx;

        private Action _finishCallback;
        private Action<Action<RepeatedField<Item>>> _claimRequestHandler;
        private Action _rewardIcons;

        private Item coin;
        private Item emerald;

        private string wheelName;

        private Popup parentPopup;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            collectButton.onClick.AddListener(OnCollectClicked);

            var rootTransform = (RectTransform) transform.Find("Root");

            if (rootTransform.sizeDelta.x > ViewResolution.referenceResolutionLandscape.x)
            {
                //rootTransform.sizeDelta.x;
                var scale = ViewResolution.referenceResolutionLandscape.x / rootTransform.sizeDelta.x;
                rootTransform.localScale = new Vector3(scale,scale,scale);
            }
        }
        
        public async void SetCollectViewData(Popup popup, RepeatedField<Item> rewardItem, Action finishCallback, string inWheelName, Action<Action<RepeatedField<Item>>> claimRequestHandler, ulong buffMultiplier = 1)
        {
            parentPopup = popup;
            
            wheelName = inWheelName;
            _claimRequestHandler = claimRequestHandler;
            
            _finishCallback = finishCallback;
 
            boostBuff.gameObject.SetActive(buffMultiplier > 1);
            
            coin = XItemUtility.GetItem(rewardItem, Item.Types.Type.Coin);
            emerald = XItemUtility.GetItem(rewardItem, Item.Types.Type.Emerald);

            if (emerald == null)
            {
                diamondIntegralText.gameObject.SetActive(false);
                totalIntegralGroup.anchoredPosition = new Vector2(0, totalIntegralGroup.anchoredPosition.y);
            }
            else
            {
                diamondIntegralText.gameObject.SetActive(true);
                diamondIntegralText.text = emerald.Emerald.Amount.GetCommaFormat();
            }
            
            vipDescriptionText.text = Client.Get<VipController>().GetVipName();
            var currentVipLevel = Client.Get<VipController>().GetVipLevel();
            var vipLevelConfig = Client.Get<VipController>().GetCurrentVipLevelConfig();
            //TODO GetVipAddition From VipSystem;
            
            double vipAddition = vipLevelConfig.SystemTimerBonus /100d;
           
            descriptionText.text = wheelName;
            
            var vipLevel = 1;
          
            while (vipGroup.Find($"VIP{vipLevel}Group") != null)
            {
                var levelConfig = Client.Get<VipController>().GetVipLevelConfig(vipLevel);
                if (levelConfig == null)
                {
                    break;
                }
                var vipLevelUI = vipGroup.Find($"VIP{vipLevel}Group");
                vipLevelUI.Find("Circle").gameObject.SetActive(vipLevel == currentVipLevel);
                vipLevelUI.Find("VIPText").GetComponent<TextMeshProUGUI>().text = levelConfig.VipName;
                vipLevelUI.Find("PercentText").GetComponent<TextMeshProUGUI>().text =
                    "+" + levelConfig.SystemTimerBonus + "%";
                vipLevel++;
            }

            var finalRewardCoin = ((long) (coin.Coin.Amount/buffMultiplier));
            var wheelRewardCoin = (long) Math.Round(finalRewardCoin/ (1 + vipAddition));
            var vipRewardCoin = (long) Math.Round(wheelRewardCoin * vipAddition);

            if (vipRewardCoin == 0)
            {
                var anchorPosition = ((RectTransform) wheelIntegralText.transform).anchoredPosition;
                ((RectTransform) wheelIntegralText.transform).anchoredPosition = new Vector2(0, anchorPosition.y);
                vipIntegralText.gameObject.SetActive(false);
            }
            else
            {
                var anchorPosition = ((RectTransform) wheelIntegralText.transform).anchoredPosition;
                ((RectTransform) wheelIntegralText.transform).anchoredPosition = new Vector2(-291, anchorPosition.y);
                vipIntegralText.gameObject.SetActive(true);
            }
            
            collectButton.interactable = false; 
          //  if (wheelName == "SUPER WHEEL")
            {
                coinIntegralText.text = finalRewardCoin.GetCommaOrSimplify(10);
                vipIntegralText.text = vipRewardCoin.GetCommaFormat();
                wheelIntegralText.text = wheelRewardCoin.GetCommaFormat();
            } 
            // else
            // {
            //     coinIntegralText.text = "0";
            //     vipIntegralText.text = "0";
            //     wheelIntegralText.text = "0";
            //     
            //     await  DoCountUpdate(wheelIntegralText, wheelRewardCoin);
            //     await  DoCountUpdate(vipIntegralText, vipRewardCoin);
            //     await  DoCountUpdate(coinIntegralText, finalRewardCoin);
            // }

            if (buffMultiplier > 1)
            {
                boostBuffFx.gameObject.SetActive(true);

                await XUtility.WaitSeconds(1);
                
                coinIntegralText.text = (finalRewardCoin * (long)buffMultiplier).GetCommaOrSimplify(10);
            }
            
            collectButton.interactable = true; 
        }

        public async Task DoCountUpdate(TextMeshProUGUI valueText, long endValue)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            long v = 0;

            DOTween.To(() => v, (x) =>
            {
                v = x;
                valueText.text = v.GetCommaFormat();
            }, endValue, 1.0f).OnComplete(() =>
            {
                valueText.text = endValue.GetCommaFormat();
                waitTask.SetResult(true);
            });

            await waitTask.Task;
        }

        public void OnCollectClicked()
        {
            collectButton.interactable = false;
            SoundController.PlayButtonClick();
            _claimRequestHandler.Invoke(OnClaimRequestCallback);
        }
        
        public async void OnClaimRequestCallback(RepeatedField<Item> items)
        {
            if (items != null)
            {
                var currencyCoinView = await parentPopup.AddCurrencyCoinView();
                
                await XUtility.FlyCoins(collectButton.transform,
                    new EventBalanceUpdate(coin.Coin.Amount, wheelName), currencyCoinView);
                
                var emeraldItem = XItemUtility.GetItem(items, Item.Types.Type.Emerald);
               
                if (emeraldItem != null)
                {
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate((long) emeraldItem.Emerald.Amount, "wheelName"));
                }
               
                parentPopup.RemoveChild(currencyCoinView);
 
                _finishCallback?.Invoke();
            }
            else
            {
                XDebug.LogError("Claim WheelBonus Failed");
            }
        }
    }
}