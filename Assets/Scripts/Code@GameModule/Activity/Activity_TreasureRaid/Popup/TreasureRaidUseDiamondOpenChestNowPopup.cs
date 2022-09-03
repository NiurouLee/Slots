using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidOpenBoxBuyEmerald")]
    public class TreasureRaidUseDiamondOpenChestNowPopup : Popup<TreasureRaidUseDiamondOpenChestNowPopupController>
    {
        [ComponentBinder("Root/MainGroup/SpendEmeraldButton")]
        private Button useDiamondBtn;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/EmeraldGrid/CountText")]
        private Text useDiamondCountText;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/ClickMask")]
        private Transform btnMask;

        private MonopolyGiftBox giftBox;
        private Action<bool, MonopolyDailyTask> callback;

        private CurrencyEmeraldView _emeraldView;

        public TreasureRaidUseDiamondOpenChestNowPopup(string address)
            : base(address)
        {
            
        }
        protected override void BindingComponent()
        {
            base.BindingComponent();
            useDiamondBtn.onClick.AddListener(OnUseDiamondBtnClicked);
        }

        protected override async void EnableView()
        {
            base.EnableView();
            _emeraldView = await AddCurrencyEmeraldView();
            _emeraldView.Show();
        }

        private async void OnUseDiamondBtnClicked()
        {
            if (giftBox.OpenDiamondFill > Client.Get<UserController>().GetDiamondCount())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"Activity_TreasureRaid_OpenChest")));
                return;
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidChestopen, ("OperationId", "2"));

            SetBtnState(false);

            // 发送协议
            var activity_TreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activity_TreasureRaid == null)
            {
                Close();
                callback?.Invoke(false, null);
                return;
            }

            SMonopolyOpenGiftBox sMonopolyOpenGiftBox = await activity_TreasureRaid.MonopolyOpenGiftBox(giftBox);
            if (sMonopolyOpenGiftBox == null)
            {
                Close();
                callback?.Invoke(false, null);
                return;
            }
            if (transform == null)
                return;

            // 发送刷新ChestBox事件
            EventBus.Dispatch(new EventTreasureRaidRefreshChestBox(sMonopolyOpenGiftBox.MonopolyRoundInfo));

            EventBus.Dispatch(new EventEmeraldBalanceUpdate(-giftBox.OpenDiamondFill, "TreasureRaidOpenChest"));
            await XUtility.WaitSeconds(2);
            if (_emeraldView != null)
            {
                RemoveChild(_emeraldView);
            }

            Close();
            // 打开 开宝箱页面
            var openChestView =  await PopupStack.ShowPopup<TreasureRaidOpenChestPopup>();
            openChestView.InitRewardContent(sMonopolyOpenGiftBox, giftBox);
            openChestView.ShowRewardCollect(() =>
            {
                //这个回调是多余宝箱页面的回调，从Main页面弹出的改页面没有回调，所以要判空
                callback?.Invoke(true, sMonopolyOpenGiftBox.DailyTask);
            });
        }

        private void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            useDiamondBtn.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        protected override void OnCloseClicked()
        {
            SetBtnState(false);
            base.OnCloseClicked();
            callback?.Invoke(false, null);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidChestopen, ("OperationId", "1"));
        }

        public void SetMonopolyGiftBoxAndCallback(MonopolyGiftBox monopolyGiftBox, Action<bool, MonopolyDailyTask> inCallback)
        {
            this.giftBox = monopolyGiftBox;
            this.callback = inCallback;
            useDiamondCountText.SetText(giftBox.OpenDiamondFill.ToString());
        }
    }

    public class TreasureRaidUseDiamondOpenChestNowPopupController : ViewController<TreasureRaidUseDiamondOpenChestNowPopup>
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