using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIAmazingInTheHatStageGetRewardBreakOff")]
    public class UIAmazingHatStageGetRewardBreakOffPopup : Popup<UIAmazingHatStageGetRewardBreakOffPopupController>
    {
        [ComponentBinder("Root/MainGroup/RewardGroup")]
        private Transform _rewardGroup;  
        
        [ComponentBinder("Root/BottomGroup/FreeAgaintButton")]
        private Button _freeAgainBtn;

        [ComponentBinder("Root/TopGroup/CloseButton")]
        private Button _colseButton;
        
        [ComponentBinder("Root/TopGroup/TitleGroup/StageText")]
        private Text _stageText;
        
        [ComponentBinder("Root/BottomGroup/PlayingButton")]
        private Button _playBtn;
        
        [ComponentBinder("Root/BottomGroup/GiveUpButton")]
        private Button _giveUpBtn;
        
        [ComponentBinder("Root/BottomGroup/PlayingButton/SpendGroup/QuantityText")]
        private Text _costDiamond;
        
        [ComponentBinder("Root/BottomGroup/PlayingButton/ButtonMask")]
        private Transform _btnMask;
        
        protected Action FinishCallback;

        private Reward _reward;

        private CurrencyEmeraldView _emeraldView;
        
        public UIAmazingHatStageGetRewardBreakOffPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _freeAgainBtn.onClick.AddListener(OnShowAdBtnClicked);
            _playBtn.onClick.AddListener(OnKeepPlayBtnClicked);
            
            
            _colseButton.gameObject.SetActive(false);
            _giveUpBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        private async void OnKeepPlayBtnClicked()
        {
            var controller = Client.Get<AmazingHatController>();
            var costCount = controller.CostDiamond;
            if ((ulong)costCount > Client.Get<UserController>().GetDiamondCount())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"AmazingHat")));
                return;
            }
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatOver, ("Operation:", "ClickRevive"),("OperationId","2"));
            SoundController.PlayButtonClick();
            SetBtnState(false);
            await Client.Get<AmazingHatController>().HatGameRevive(async (success) =>
            {
                if (success)
                {
                    //TODO 继续下一关
                    FinishCallback?.Invoke();
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(-costCount, "AmazingHatRevive"));
                    await XUtility.WaitSeconds(2);
                    if (_emeraldView != null)
                    {
                        RemoveChild(_emeraldView);
                    }
                    Close();
                }
                else
                {
                    SetBtnState(true);
                }
            }, false, true);
        }

        private void OnShowAdBtnClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatOver, ("Operation:", "ClickAd"),("OperationId","1"));

            SoundController.PlayButtonClick();
            // 展示广告，监听回调
            SetBtnState(false);

            if (AdController.Instance.ShouldShowRV(eAdReward.AmazingHat, false))
            {
                AdController.Instance.TryShowRewardedVideo(eAdReward.AmazingHat, OnAdsEnd);
                AdController.Instance.enableAdConfigRefresh = false;
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                SetBtnState(true);
            }
        }

        private void SetBtnState(bool state)
        {
            _freeAgainBtn.interactable = state;
            _playBtn.interactable = state;
            _giveUpBtn.interactable = state;
            _btnMask.gameObject.SetActive(!state);
        }
        private async void OnAdsEnd(bool showAdSuccess, string reason)
        {
            if (showAdSuccess)
            {
                await Client.Get<AmazingHatController>().HatGameRevive(async (success) =>
                {
                    if (success)
                    {
                        // 继续下一关
                        FinishCallback?.Invoke();
                        if (_emeraldView != null)
                        {
                            RemoveChild(_emeraldView);
                        }
                        Close();
                    }
                    else
                    {
                        SetBtnState(true);
                    }
                }, true, true);
            }
            else
            {
                SetBtnState(true);
            }
        }

        private async void OnCloseBtnClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatOver, ("Operation:", "ClickGiveUp"),("OperationId","3"));
            SoundController.PlayButtonClick();
            SetBtnState(false);
            await Client.Get<AmazingHatController>().HatGameRevive(async (success) =>
            {
                if (transform == null)
                    return;

                if (success)
                {
                    if (_emeraldView != null)
                    {
                        RemoveChild(_emeraldView);
                    }
                    Close();
                    PopupStack.ClosePopup<AmazingHatMainPopup>();
                    SoundController.RecoverLastMusic();
                }
                else
                {
                    SetBtnState(true);
                }
            }, false, false);
        }

        public async void InitRewardContent()
        {
            var controller = Client.Get<AmazingHatController>();
            _costDiamond.text = controller.CostDiamond.ToString();
            _stageText.SetText(String.Format("{0}/{1}", controller.Level, 15));
            if (!AdController.Instance.ShouldShowRV(eAdReward.AmazingHat, false))
            {
                _freeAgainBtn.gameObject.SetActive(false);
            }
            //转换一下数据
            var rewards = Client.Get<AmazingHatController>().Rewards;
            _reward = rewards[0];
            XItemUtility.InitItemsUI(_rewardGroup, rewards, _rewardGroup.Find("AmazingInTheHatCell"), GetItemDescribe, "DisableType");
            for (int i = 0; i < _reward.Items.Count; i++)
            {
                var itemUI = XItemUtility.GetItemUI(_rewardGroup, _reward.Items[i].Type);
                itemUI.SetAsLastSibling();
            }

            _emeraldView = await AddCurrencyEmeraldView();
        }

        private string GetItemDescribe(List<Item> items)
        {
            var type = items[0].Type;
      
            if (type == Item.Types.Type.Coin || type == Item.Types.Type.ShopCoin)
            {
                ulong totalWin = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalWin += items[i].Coin.Amount;
                }
                return totalWin.GetAbbreviationFormat();   
            }
            if (type == Item.Types.Type.Emerald || type == Item.Types.Type.ShopEmerald)
            {
                ulong totalDiamond = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalDiamond += items[i].Emerald.Amount;
                }
                return totalDiamond.GetAbbreviationFormat();   
            }

            
            if (type == Item.Types.Type.LevelUpBurst)
            {
                uint totalTime = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalTime += items[i].LevelUpBurst.Amount;
                }
                return GetFormatTime(totalTime);
            }
            if (type == Item.Types.Type.DoubleExp)
            {
                uint totalTime = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalTime += items[i].DoubleExp.Amount;
                }
                return GetFormatTime(totalTime);
            }

            if (type == Item.Types.Type.CardPackage)
            {
                return "+" + items.Count;
            }

            if (type == Item.Types.Type.VipPoints)
            {
                uint amount = 0;
             
                for (int i = 0; i < items.Count; i++)
                {
                    amount += items[i].VipPoints.Amount;
                }

                return "+" + amount;
            }
            
            return XItemUtility.GetItemRewardSimplyDescText(items[0]);
            
            return string.Empty;
        }
        
        private string GetFormatTime(uint time)
        {
            uint h = time / 60;
            uint m = time % 60;
            if (h > 0 && m > 0)
            {
                return h + "H" + m + "M";
            }
            if (h>0)
            {
                return h + "H";
            }
            if (m>0)
            {
                return m + "MINS";
            }
            return "";
        }
        
        
        public void SetReviveHatGameCallBack(Action finishCallback)
        {
            FinishCallback = finishCallback;
        }
    }

    public class UIAmazingHatStageGetRewardBreakOffPopupController : ViewController<UIAmazingHatStageGetRewardBreakOffPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        private void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }
    }
}