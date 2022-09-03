using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameModule
{
    [AssetAddress("UIValentinesDay2022Reward")]
    public class UIActivity_ValentinesDay_CollectPopup : Popup<UIActivity_Valentine2022_CollectPopupController>
    {
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/UIValentinesDay2022RewardCell")]
        public Transform transformFreePrefab;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup")]
        public Transform transformFreeRoot;

        [ComponentBinder("Root/RewardGroup/ValentineGroup/Viewport/Content/UIValentinesDay2022RewardCell")]
        public Transform transformSpecialPrefab;

        [ComponentBinder("Root/RewardGroup/ValentineGroup/Viewport/Content")]
        public Transform transformSpecialRoot;



        [ComponentBinder("Root/SpecialRewardLockStateBottomGroup")]
        public Transform transformFreeButtonsGroup;

        [ComponentBinder("Root/RewardGroup/ValentineGroup/TitleGroup/SpecialRewardLockState")]
        public Transform transformFreeText;

        [ComponentBinder("Root/SpecialRewardLockStateBottomGroup/CollectButton")]
        public Button buttonFreeCollect;

        [ComponentBinder("Root/SpecialRewardLockStateBottomGroup/PriceButton")]
        public Button buttonFreePurchase;

        [ComponentBinder("Root/SpecialRewardLockStateBottomGroup/ExtraContentsButton")]
        public Button buttonBenefit;

        [ComponentBinder("Root/SpecialRewardLockStateBottomGroup/PriceButton/PriceText")]
        public Text textFreePrice;



        [ComponentBinder("Root/RewardGroup/ValentineGroup")]
        public ScrollRect scrollRect;


        [ComponentBinder("Root/FreePassStateBottomGroup")]
        public Transform transformSpecialButtonsGroup;

        [ComponentBinder("Root/RewardGroup/ValentineGroup/TitleGroup/FreePassState")]
        public Transform transformSpecialText;

        [ComponentBinder("Root/FreePassStateBottomGroup/CollectButton")]
        public Button buttonSpecialCollect;


        [ComponentBinder("Root/RewardGroup/ValentineGroup/PreviousButton")]
        public Transform transformPrevious;

        [ComponentBinder("Root/RewardGroup/ValentineGroup/NextButton")]
        public Transform transformNext;



        private readonly List<UIActivity_ValentinesDay_RewardCellView> _cells
            = new List<UIActivity_ValentinesDay_RewardCellView>();

        public UIActivity_ValentinesDay_CollectPopup(string address) : base(address)
        {
            // contentDesignSize = new Vector2(1365, 768);
        }

        public override string GetOpenAudioName()
        {
            return "General_GiftBoxOpen";
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            transformFreePrefab.gameObject.SetActive(false);
            transformSpecialPrefab.gameObject.SetActive(false);
        }

        private void Clear()
        {
            if (_cells.Count > 0)
            {
                foreach (var cell in _cells)
                {
                    RemoveChild(cell);
                    GameObject.Destroy(cell.transform.gameObject);
                }

                _cells.Clear();
            }
        }

        public bool ScrollRectContentOverflow()
        {
            return scrollRect.viewport.rect.width < scrollRect.content.rect.width;
        }

        public void SetStyle(bool purchased)
        {
            if (purchased)
            {
                transformFreeButtonsGroup.gameObject.SetActive(false);
                transformFreeText.gameObject.SetActive(false);
                transformSpecialButtonsGroup.gameObject.SetActive(true);
                transformSpecialText.gameObject.SetActive(true);
            }
            else
            {
                transformFreeButtonsGroup.gameObject.SetActive(true);
                transformFreeText.gameObject.SetActive(true);
                transformSpecialButtonsGroup.gameObject.SetActive(false);
                transformSpecialText.gameObject.SetActive(false);
            }
        }

        public void SetButtonInteractable(bool interactable)
        {
            if (buttonFreePurchase != null)
            {
                buttonFreePurchase.interactable = interactable;
            }
            if (buttonFreeCollect != null)
            {
                buttonFreeCollect.interactable = interactable;
            }
            if (buttonSpecialCollect != null)
            {
                buttonSpecialCollect.interactable = interactable;
            }
            if (buttonBenefit != null)
            {
                buttonBenefit.interactable = interactable;
            }
        }


        public void Set()
        {
            Clear();
            var activity = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
            if (activity == null) { return; }

            // set items
            var freeItems = activity.availableFreeItems;
            var specialItems = activity.availableSpecialItems;

            if (freeItems != null && freeItems.Length > 0)
            {
                transformFreeRoot.gameObject.SetActive(true);
                for (int i = 0; i < freeItems.Length; i++)
                {
                    var item = freeItems[i];
                    var go = Object.Instantiate(transformFreePrefab.gameObject, transformFreeRoot);
                    var cell = AddChild<UIActivity_ValentinesDay_RewardCellView>(go.transform);
                    cell.Set(item);
                    _cells.Add(cell);
                }
            }
            else
            {
                transformFreeRoot.gameObject.SetActive(false);
            }

            if (specialItems != null && specialItems.Length > 0)
            {
                for (int i = 0; i < specialItems.Length; i++)
                {
                    var item = specialItems[i];
                    var go = Object.Instantiate(transformSpecialPrefab.gameObject, transformSpecialRoot);
                    var cell = AddChild<UIActivity_ValentinesDay_RewardCellView>(go.transform);
                    cell.Set(item);
                    _cells.Add(cell);
                }
            }

            // set scroll rect
            scrollRect.normalizedPosition = Vector2.up;
            if (ScrollRectContentOverflow() == false)
            {
                transformPrevious.gameObject.SetActive(false);
                transformNext.gameObject.SetActive(false);
            }
            else
            {
                transformPrevious.gameObject.SetActive(false);
                transformNext.gameObject.SetActive(true);
            }

            // set price
            var shopItemConfig = activity.shopItemConfig;
            if (shopItemConfig == null) { return; }
            textFreePrice.text = "$" + shopItemConfig.Price;

            SetStyle(activity.purchased);
        }
    }

    public class UIActivity_Valentine2022_CollectPopupController : ViewController<UIActivity_ValentinesDay_CollectPopup>
    {
        private Activity_ValentinesDay _activity;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SetButtonInteractable(true);
            _activity = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
            if (_activity.hasRewardToCollect == false)
            {
                PopupStack.ClosePopup<UIActivity_ValentinesDay_CollectPopup>();
                return;
            }
            Refresh();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            view.buttonBenefit.onClick.AddListener(OnBenefit);
            view.buttonFreeCollect.onClick.AddListener(OnCollect);
            view.buttonSpecialCollect.onClick.AddListener(OnCollect);
            view.buttonFreePurchase.onClick.AddListener(OnPurchase);
            SubscribeEvent<Event_Activity_Valentine2022_ReceiveMainPageInfo>(OnEvent_Activity_Valentine2022_ReceiveMainPageInfo);
        }

        private void OnEvent_Activity_Valentine2022_ReceiveMainPageInfo(Event_Activity_Valentine2022_ReceiveMainPageInfo obj)
        {
            view.SetStyle(_activity.purchased);
            SetButtonInteractable(true);
        }

        private void Refresh() { view.Set(); }

        private void SetButtonInteractable(bool interactable)
        {
            if (view == null) { return; }
            view?.SetButtonInteractable(interactable);
        }

        private void OnPurchase()
        {
            if (_activity == null || _activity.shopItemConfig == null) { return; }
            var shopItemConfig = _activity.shopItemConfig;
            SetButtonInteractable(false);
            _activity.purchaseSource = 2;
            Client.Get<IapController>().BuyProduct(shopItemConfig);
            SetButtonInteractable(true);
        }

        private async void OnCollect()
        {
            if (_activity == null) { return; }
            SetButtonInteractable(false);
            await _activity.SendCCollectValentineRewards();
            var s = _activity.sCollectValentineRewards;
            if (s != null && s.Success)
            {
                var profile = s.UserProfile;
                EventBus.Dispatch(new EventUserProfileUpdate(profile));

                if (s.Rewards != null && s.Rewards.count > 0)
                {
                    var reward = s.Rewards[0];
                    if (reward != null && reward.Items != null && reward.Items.count > 0)
                    {

                        Item coinItem = null;
                        foreach (var item in reward.Items)
                        {
                            if (item.Type == Item.Types.Type.Coin)
                            {
                                coinItem = item;
                                break;
                            }
                        }

                        if (coinItem != null)
                        {
                            var coin = coinItem.Coin.Amount;

                            var from = _activity.purchased
                                        ? view.buttonSpecialCollect.transform
                                        : view.buttonFreeCollect.transform;

                            await XUtility.FlyCoins(
                                    from,
                                    new EventBalanceUpdate(coin, "Activity_Valentine2022_Collect")
                                );
                        }

                        ItemSettleHelper.SettleItems(
                            reward.Items,
                            () => { EventBus.Dispatch(new EventRefreshUserProfile()); }, 0, "Activity_Valentine2022_Collect");
                    }
                }
            }

            await _activity.PrepareMainPageData();
            PopupStack.ClosePopup<UIActivity_ValentinesDay_CollectPopup>();
            SetButtonInteractable(true);
        }

        private async void OnBenefit()
        {
            if (_activity == null || _activity.shopItemConfig == null) { return; }
            var shopItemConfig = _activity.shopItemConfig;
            if (shopItemConfig == null || shopItemConfig.SubItemList == null
                || shopItemConfig.SubItemList.count == 0) { return; }
            SetButtonInteractable(false);
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
            SetButtonInteractable(true);
        }

        private void OnScrollRectValueChanged(Vector2 position)
        {
            if (view.ScrollRectContentOverflow() == false)
            {
                view.transformPrevious.gameObject.SetActive(false);
                view.transformNext.gameObject.SetActive(false);
            }
            else if (position.x <= 0.01f)
            {
                view.transformPrevious.gameObject.SetActive(false);
                view.transformNext.gameObject.SetActive(true);
            }
            else if (position.x < 0.99f)
            {
                view.transformPrevious.gameObject.SetActive(true);
                view.transformNext.gameObject.SetActive(true);
            }
            else
            {
                view.transformPrevious.gameObject.SetActive(true);
                view.transformNext.gameObject.SetActive(false);
            }
        }
    }
}