using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidFireAgainNotice")]
    public class TreasureRaidFireAgainPopup : Popup<TreasureRaidFireAgainPopupController>
    {
        [ComponentBinder("Root/MainGroup/ByAdsButton")]
        private Button freeAdBtn;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton")]
        private Button useDiamondBtn;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/EmeraldGrid/CountText")]
        private Text costDiamondText;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/ClickMask")]
        private Transform btnMask;

        private Action<bool, SMonopolyShootAgain> endCallback;

        private uint costDiamond;

        private CurrencyEmeraldView _emeraldView;

        public TreasureRaidFireAgainPopup(string address) : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            freeAdBtn.onClick.AddListener(OnOpenAdBtnClicked);
            useDiamondBtn.onClick.AddListener(OnUseDiamondBtnClicked);
        }

        protected override async void EnableView()
        {
            base.EnableView();
            freeAdBtn.gameObject.SetActive(AdController.Instance.ShouldShowRV(eAdReward.TreasureRaid, false));
            _emeraldView = await AddCurrencyEmeraldView();
        }

        public void SetEndCallback(Action<bool, SMonopolyShootAgain> callback, uint needDiamond)
        {
            endCallback = callback;
            costDiamond = needDiamond;
            costDiamondText.SetText(costDiamond.ToString());
        }

        private void SetBtnState(bool interactable)
        {
            freeAdBtn.interactable = interactable;
            useDiamondBtn.interactable = interactable;
            closeButton.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        protected override void OnCloseClicked()
        {
            SetBtnState(false);
            base.OnCloseClicked();
            endCallback?.Invoke(false, null);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidFireagain, ("OperationId", "3"));
        }

        private async void OnUseDiamondBtnClicked()
        {
            if (costDiamond > Client.Get<UserController>().GetDiamondCount())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"Activity_TreasureRaid_FireAgain")));
                return;
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidFireagain, ("OperationId", "2"));
            SetBtnState(false);
            var activity_TreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                    Activity_TreasureRaid;
            if (activity_TreasureRaid == null)
                return;

            await activity_TreasureRaid.MonopolyShootAgain(async (success, sMonopolyShootAgain) =>
            {
                if (transform == null)
                    return;
                if (success)
                {
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(-costDiamond, "TreasureRaidFireAgain"));
                    await XUtility.WaitSeconds(2);

                    if (_emeraldView != null)
                    {
                        RemoveChild(_emeraldView);
                    }
                    Close();
                    await XUtility.WaitSeconds(1);
                    endCallback?.Invoke(true, sMonopolyShootAgain);
                }
                else
                {
                    SetBtnState(true);
                }
            });
        }

        private void OnOpenAdBtnClicked()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.TreasureRaid, false))
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidFireagain, ("OperationId", "1"));
                SetBtnState(false);
                AdController.Instance.TryShowRewardedVideo(eAdReward.TreasureRaid, OnAdsEnd);
                AdController.Instance.enableAdConfigRefresh = false;
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                SetBtnState(true);
            }
        }
        
        private async void OnAdsEnd(bool showAdSuccess, string reason)
        {
            if (transform == null)
                return;

            if (showAdSuccess)
            {
                var activity_TreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activity_TreasureRaid == null)
                    return;

                await activity_TreasureRaid.MonopolyShootAgain(async (success, sMonopolyShootAgain) =>
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
                        endCallback?.Invoke(true, sMonopolyShootAgain);
                    }
                    else
                    {
                        SetBtnState(true);
                    }
                }, "ad");
            }
            else
            {
                SetBtnState(true);
            }
        }

    }

    public class TreasureRaidFireAgainPopupController : ViewController<TreasureRaidFireAgainPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }
    }
}