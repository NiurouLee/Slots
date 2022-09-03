using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxCellView_Operation : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("DetailGroup/DescriptionText")]
        public TMP_Text tmpTextTitle;

        [ComponentBinder("DetailGroup/RewardGroup/InboxRewardCell")]
        public Transform transformRewardPrefab;

        [ComponentBinder("DetailGroup/RewardGroup")]
        public Transform transformRewardRoot;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;
        private RepeatedField<Item> _items;

        private UIInboxRewardCellView[] _itemCells;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            transformRewardPrefab.gameObject.SetActive(false);
        }
 
        public override void ParseData()
        {
            base.ParseData();
            
            if (mailData == null || mailData.ItemList == null)
            {
                return;
            }

            var data = MailRewardsPB.Parser.ParseFrom(mailData.ItemList.ToByteArray());
            if (data == null || data.Rewards == null || data.Rewards.Count == 0)
            {
                return;
            }

            var reward = data.Rewards[0];
            if (reward == null)
            {
                return;
            }

            _items = data.Rewards[0].Items;
        }

        public override void UpdateView()
        {
            base.UpdateView();
            
            ClearRewardItems();
            
            if (button != null)
            {
                button.interactable = true;
            }
 
            tmpTextTitle.text = mailData.MailInfo.Title;

            if (_items != null && _items.Count > 0)
            {
                var count = _items.Count;
                _itemCells = new UIInboxRewardCellView[count];

                for (int i = 0; i < count; i++)
                {
                    var item = _items[i];
                    if (item == null)
                    {
                        continue;
                    }

                    var go = GameObject.Instantiate(transformRewardPrefab.gameObject, transformRewardRoot);
                    go.SetActive(true);
                    var cell = AddChild<UIInboxRewardCellView>(go.transform);
                    _itemCells[i] = cell;
                    cell.Set(_items[i]);
                }
            }
        }

        private void ClearRewardItems()
        {
            if (_itemCells != null)
            {
                foreach (var item in _itemCells)
                {
                    if (item != null)
                    {
                        GameObject.Destroy(item.transform.gameObject);
                        RemoveChild(item);
                    }
                }
            }

            _itemCells = null;
        }

        public override void Update()
        {
            base.Update();
            UpdateTimeLeft();
        }

        public override void UpdateTimerText(string remainString)
        {
            if (tmpTextTimer != null) { tmpTextTimer.text = remainString; }
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

        protected override async void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                var content = sClaimMail.Content;

                var contentData = InboxController.ParseMailContent(content);
                Item coinItem = XItemUtility.GetCoinItem(contentData.Item);

                if (coinItem != null && coinItem.Coin != null)
                {
                    await XUtility.FlyCoins(
                        button.transform,
                        new EventBalanceUpdate(coinItem, "MailOperationReward")
                    );
                }

                ItemSettleHelper.SettleItems(
                    contentData.Item,
                    () => { EventBus.Dispatch(new EventRefreshUserProfile()); }, 0, "InBox");

                EventBus.Dispatch(new EventInBoxItemUpdated());
            }
            else
            {
                button.interactable = true;
            }
        }
        
        protected override void OnButtonClick()
        {
            button.interactable = false;
            base.OnButtonClick();
        }
    }
}
