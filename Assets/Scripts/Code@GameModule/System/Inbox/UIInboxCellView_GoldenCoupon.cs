using System;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine.UI;
using static DragonU3DSDK.Network.API.ILProtocol.SGetUserCoupons.Types;

namespace GameModule
{
    public class UIInboxCellView_GoldenCoupon : UIInboxCellView
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("DetailGroup/PercentageText")]
        public Text textPercent;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;

        private UserCoupon _coupon;

        protected InboxController inboxController;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            inboxController = Client.Get<InboxController>();
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private async void OnButtonClick()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                .GameEventCouponActivityCoinsRedeem);

            if (_coupon != null)
            {
                PopupStack.ClosePopup<UIInboxPopup>();

                await inboxController.SendCBindCouponToStore(_coupon.Id);

                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), $"golden coupon mail")));
            }
        }

        public override void Set(InboxItem inItemData)
        {
            base.Set(inItemData);

            if (itemData != null && itemData.data != null)
            {
                _coupon = itemData.data as UserCoupon;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            if (_coupon != null)
            {
                if (textPercent != null)
                {
                    textPercent.SetText(_coupon.BonusPersentage + "%");
                }
            }

            UpdateTimeLeft();
        }

        public virtual void UpdateTimeLeft()
        {
            if (inboxController != null)
            {
                var expire = (ulong) (_coupon?.ExpireAt ?? 0) * 1000;

                var leftTime = XUtility.GetLeftTime(expire);

                if (leftTime > 0)
                {
                    if (tmpTextTimer != null)
                    {
                        tmpTextTimer.text = XUtility.GetTimeText(leftTime);
                    }
                }
                else
                {
                    EventBus.Dispatch(new EventInBoxItemUpdated());
                }
            }
        }

        public override void Update()
        {
            UpdateTimeLeft();
        }
    }
}