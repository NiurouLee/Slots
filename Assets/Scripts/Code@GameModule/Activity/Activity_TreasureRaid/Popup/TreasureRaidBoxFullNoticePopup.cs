using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidBoxFullNotice")]
    public class TreasureRaidBoxFullNoticePopup : Popup<TreasureRaidBoxFullNoticePopupController>
    {
        [ComponentBinder("Root/MainGroup/BoxGroup")]
        private Transform boxGroup;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/CountText")]
        private Text useDiamondCountText;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton")]
        private Button useDiamondBtn;

        [ComponentBinder("Root/MainGroup/SpendEmeraldButton/ClickMask")]
        private Transform btnMask;

        private MonopolyGiftBox giftBox;
        private Action<MonopolyDailyTask> callback;

        private CurrencyEmeraldView _emeraldView;

        private MonopolyDailyTask dailyTask;
        public TreasureRaidBoxFullNoticePopup(string address)
            : base(address)
        {
            
        }
        
        protected override void BindingComponent()
        {
            base.BindingComponent();
            useDiamondBtn.onClick.AddListener(OnCollectBtnClicked);
        }

        protected override async void EnableView()
        {
            base.EnableView();
            _emeraldView = await AddCurrencyEmeraldView();
            _emeraldView.Show();
        }

        protected override void OnCloseClicked()
        {
            SetBtnState(false);
            base.OnCloseClicked();
            callback?.Invoke(dailyTask);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidChestmore, ("OperationId", "1"));
        }

        private void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            useDiamondBtn.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        private async void OnCollectBtnClicked()
        {
            SetBtnState(false);
            if (giftBox.OpenDiamondFill > Client.Get<UserController>().GetDiamondCount())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"Activity_TreasureRaid_OpenChest")));
                SetBtnState(true);
                return;
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidChestmore, ("OperationId", "2"));
            // 发送协议
            var activity_TreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activity_TreasureRaid == null)
            {
                Close();
                return;
            }

            SMonopolyOpenGiftBox sMonopolyOpenGiftBox = await activity_TreasureRaid.MonopolyOpenGiftBox(giftBox);
            if (sMonopolyOpenGiftBox == null)
            {
                Close();
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
            
            var task = sMonopolyOpenGiftBox.DailyTask;
            if (dailyTask.TaskRewardsGot != null && dailyTask.TaskRewardsGot.Items.Count > 0)
            {
                task = dailyTask;
            }

            openChestView.ShowRewardCollect(() =>
            {
                //这个回调是多余宝箱页面的回调，从Main页面弹出的改页面没有回调，所以要判空
                Close();
                callback.Invoke(task);
            });
        }

        public void SetMonopolyGiftBoxAndCallback(MonopolyGiftBox monopolyGiftBox, Action<MonopolyDailyTask> closeCallback, MonopolyDailyTask inDailyTask)
        {
            giftBox = monopolyGiftBox;
            callback = closeCallback;
            dailyTask = inDailyTask;
            useDiamondCountText.SetText(giftBox.OpenDiamondFill.ToString());
            for (int i = 0; i < boxGroup.childCount; i++)
            {
                boxGroup.GetChild(i).gameObject.SetActive((i+1) == giftBox.Level);
            }
        }
    }

    public class TreasureRaidBoxFullNoticePopupController : ViewController<TreasureRaidBoxFullNoticePopup>
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