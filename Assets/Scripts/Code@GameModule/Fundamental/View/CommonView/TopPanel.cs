// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/03/16:44
// Ver : 1.0.0
// Description : TopPanel.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Account;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class TopPanel : View<TopPanelViewController>
    {
        private static List<TopPanel> _topPanelList = new List<TopPanel>();

        [ComponentBinder("CurrencyGroup/Emerald/CountText")]
        public TextMeshProUGUI emeraldCountText;

        [ComponentBinder("CurrencyGroup/Coin/CountText")]
        public TextMeshProUGUI coinCountText;

        [ComponentBinder("CurrencyGroup/Emerald/Icon")]
        public Transform emeraldIcon;

        [ComponentBinder("CurrencyGroup/Emerald")]
        public Transform emeraldGroup;

        [ComponentBinder("CurrencyGroup/Coin")]
        public Transform coinGroup;

        [ComponentBinder("CurrencyGroup/Coin/Icon")]
        public Transform coinIcon;

        [ComponentBinder("DealTimerText")]
        public TextMeshProUGUI dealTimeText;

        [ComponentBinder("HomeButton")] public Button homeButton;

        [ComponentBinder("MenuButton")] public Transform menuButton;
        [ComponentBinder("UISet")] public Transform uiSet;
        [ComponentBinder("TimerBonus")] public Transform timerBonus;

        [ComponentBinder("AvatarButton")]
        public Button btnAvatar;

        [ComponentBinder("AvatarButton/AvatarMask/Icon")]
        public RawImage rawImageAvatar; 
        
        [ComponentBinder("AvatarButton/ReminderGroup")]
        public Transform reminderGroup;

        [ComponentBinder("CurrencyGroup/Coin/BG")]
        public Button coinButton;

        [ComponentBinder("CurrencyGroup/Emerald/BG")]
        public Button emeraldButton;

        [ComponentBinder("CenterGroup/BuyButtonLarge")]
        public Button buyButtonLarge;

        [ComponentBinder("CenterGroup")] 
        public Transform centerGroup;

        [ComponentBinder("CenterGroup/BuyButton")]
        public Button buyButton;

        [ComponentBinder("CenterGroup/DealButton")]
        public Button dealButton;

        [ComponentBinder("CenterGroup/NoticeButton")]
        public Button noticeButton;
        
        [ComponentBinder("CenterGroup/SuperSpinX")]
        public Button superSpinXButton;

        [ComponentBinder("PigButton")]
        public Button btnPiggyBank;

        [ComponentBinder("Exp")]
        public Transform expTransform;

        [ComponentBinder("TipAttach")] public Transform tipAttach;

        public NextLevelInfoView nextLevelInfoView;

        public TopPanelExtraAttachViewContainerView topPanelExtraAttachViewContainerView;

        public SettingMenuView settingMenuView;

        public TopPanelTimeBonusView topPanelTimeBonusView;

        public ExpProgressView expProgressView;

        public static Transform GetCoinIcon()
        {
#if !PRODUCTION_PACKAGE
            if (_topPanelList.Count == 0)
            {
                XDebug.LogError("LogicError:Current Time No TopPanel Available");
            }
#endif
            return _topPanelList[_topPanelList.Count - 1].coinIcon;
        }
        
        public async void ShowNewAvatarGuide()
        {
            var guide = await View.CreateView<View>("LobbyTextBubbleTL", btnAvatar.transform);
  
            var canvas = btnAvatar.GetComponent<Canvas>();

            if (canvas == null)
            {
                canvas = btnAvatar.gameObject.AddComponent<Canvas>();
            }
            
            guide.transform.Find("Root/ContentGroup/ContentText").GetComponent<TMP_Text>().text = "NEW HEAD PORTRAIT";
           
            if (!btnAvatar.GetComponent<GraphicRaycaster>())
            {
                btnAvatar.gameObject.AddComponent<GraphicRaycaster>();
            }

            var guideMask = new GameObject("GuideMask");
            guideMask.AddComponent<RectTransform>().sizeDelta = new Vector2(5000,2000);
            guideMask.AddComponent<Image>().color = new Color(0,0,0,0.8f);
             
            guide.transform.SetAsFirstSibling();
            guideMask.transform.SetParent(btnAvatar.transform,false);
            guideMask.transform.SetAsFirstSibling();

            var selectEventCustomHandler = guide.transform.gameObject.AddComponent<SelectEventCustomHandler>();
        
            selectEventCustomHandler.BindingDeselectedAction((eventData) =>
            { 
                guide.Destroy();
                GameObject.Destroy(guideMask);
            });
            
            canvas.overrideSorting = true;
            canvas.sortingOrder = -1;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            
            EventSystem.current.SetSelectedGameObject(guide.transform.gameObject);
        }
        
        public async void ShowStoreBonusGuide()
        {
           var guide = await View.CreateView<View>("UIGuideStoreBonus", centerGroup);

           var canvas = centerGroup.GetComponent<Canvas>();

           if (canvas == null)
           {
               canvas = centerGroup.gameObject.AddComponent<Canvas>();
           }
           
           if (!centerGroup.GetComponent<GraphicRaycaster>())
           {
              centerGroup.gameObject.AddComponent<GraphicRaycaster>();
           }
           
           guide.transform.SetAsFirstSibling();

           var selectEventCustomHandler = guide.transform.gameObject.AddComponent<SelectEventCustomHandler>();
        
           selectEventCustomHandler.BindingDeselectedAction((eventData) =>
           { 
               guide.Destroy();
           });
            
           canvas.overrideSorting = true;
           canvas.sortingOrder = -1;
           canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
           
           if (buyButtonLarge.gameObject.activeInHierarchy)
           {
               (guide.rectTransform.anchoredPosition) = ((RectTransform)buyButtonLarge.transform).anchoredPosition;
           }
           else
           {
               var buyButtonTransform = ((RectTransform) buyButton.transform);
               (guide.transform.localPosition) = buyButtonTransform.localPosition + new Vector3(buyButtonTransform.sizeDelta.x * 0.5f, 0,0);
           }
           
           EventSystem.current.SetSelectedGameObject(guide.transform.gameObject);
        }

        public static long GetCoinCountNum()
        {
            if (_topPanelList.Count > 0)
            {
                return _topPanelList[_topPanelList.Count - 1].viewController.coinCountNum;
            }

            return (long)Client.Get<UserController>().GetCoinsCount();
        }
        
        public static long GetEmeraldCountNum()
        {
            if (_topPanelList.Count > 0)
            {
                return _topPanelList[_topPanelList.Count - 1].viewController.emeraldCountNum;
            }

            return (long)Client.Get<UserController>().GetDiamondCount();
        }


        public static Transform GetEmeraldIcon()
        {
#if !PRODUCTION_PACKAGE
            if (_topPanelList.Count == 0)
            {
                XDebug.LogError("LogicError:Current Time No TopPanel Available");
            }
#endif
            return _topPanelList[_topPanelList.Count - 1].emeraldIcon;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            topPanelExtraAttachViewContainerView = AddChild<TopPanelExtraAttachViewContainerView>(tipAttach);

            settingMenuView = AddChild<SettingMenuView>(uiSet);

            if (timerBonus.gameObject.activeSelf)
                topPanelTimeBonusView = AddChild<TopPanelTimeBonusView>(timerBonus);

            if (transform.gameObject.name.Contains("V"))
            {
                homeButton.transform.SetAsFirstSibling();
                menuButton.transform.SetAsFirstSibling();
            }

            expProgressView = AddChild<ExpProgressView>(expTransform);
 
            AdaptUI();
        }

        protected void AdaptUI()
        {
            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    transform.localScale = new Vector3(localScale, localScale, localScale);
                }
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _topPanelList.Add(this);
            SetUpCoinCanvasSortingLayer();
            SetUpEmeraldCanvasSortingLayer();
        }

        protected void SetUpEmeraldCanvasSortingLayer()
        {
            var canvas = coinGroup.GetComponent<Canvas>();
            canvas.overrideSorting = false;
            canvas.sortingOrder = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        public void ModifyEmeraldGroupSortingOrder(bool toTop)
        {
            var canvas = emeraldGroup.GetComponent<Canvas>();

            if (toTop)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 5;
                canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }

            canvas.overrideSorting = false;
            canvas.sortingOrder = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        protected void SetUpCoinCanvasSortingLayer()
        {
            var canvas = coinGroup.GetComponent<Canvas>();
            canvas.overrideSorting = false;
            canvas.sortingOrder = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        public void ModifyCoinGroupSortingOrder(bool toTop)
        {
            var canvas = coinGroup.GetComponent<Canvas>();

            if (toTop)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 5;
                canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }

            canvas.overrideSorting = false;
            canvas.sortingOrder = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        public bool IsCoinGroupOnSystemPopupLayer()
        {
            var canvas = coinGroup.GetComponent<Canvas>();
            if (canvas != null)
            {
                return canvas.sortingLayerID == SortingLayer.NameToID("SystemPopup");
            }
            return false;
        }

        private Tween _emeraldTween;
        public void UpdateEmeraldCount(long startEmeraldNum, long delta, bool hasRollAnimation)
        {
            long target = startEmeraldNum + delta;

            if (_emeraldTween != null)
            {
                _emeraldTween.Kill();
                _emeraldTween = null;
            }

            DOTween.Kill(emeraldCountText);

            if (hasRollAnimation)
            {
                emeraldCountText.text = startEmeraldNum.GetCommaFormat();

                double v = startEmeraldNum;

                _emeraldTween = DOTween.To(() => v, (x) =>
                {
                    v = x;
                    emeraldCountText.text = v.GetCommaFormat();
                }, target, 2.0f).OnComplete(() => { emeraldCountText.text = target.GetCommaFormat(); });
            }
            else
            {
                emeraldCountText.text = target.GetCommaFormat();
            }
        }

        public void UpdateUserAvatar(uint avatarID)
        {
            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetSelfAvatar(avatarID, (t) =>
                {
                    var controller = Client.Get<UserController>();
                    if (rawImageAvatar != null && controller != null && avatarID == controller.GetUserAvatarID())
                    {
                        rawImageAvatar.texture = t;
                    }
                });
            }
        }

        private Tween _coinTween;
        public void UpdateCoinCount(long startCoinNum, long delta, bool hasRollAnimation)
        {
            long target = startCoinNum + delta;

            //避免由于玩家在上一把升级钱还没有加完的情况下，点击开始下一把Spin，导致提现扣除BET，造成Balance变成负数的情况
            if (target < 0)
            {
                target = 0;
            }
            
            var serverCoinsCount = Client.Get<UserController>().GetCoinsCount();

            if (target > (long)serverCoinsCount)
            {
                XDebug.LogError($"BalanceCoinException [client:{target},server:{serverCoinsCount}]");
                target = (long)serverCoinsCount;
                startCoinNum = target - delta;
            }

            if (_coinTween != null)
            {
                _coinTween.Kill();
                _coinTween = null;
            }

            DOTween.Kill(coinCountText);

            if (hasRollAnimation)
            {
                coinCountText.text = startCoinNum.GetCommaFormat();

                double v = startCoinNum;
                
                _coinTween = DOTween.To(() => v, (x) =>
                {
                    v = x;
                    coinCountText.text = v.GetCommaFormat();
                }, target, 2.0f).OnComplete(() => { coinCountText.text = target.GetCommaFormat(); });
            }
            else
            {
                coinCountText.text = target.GetCommaFormat();
            }
        }

        public override void Destroy()
        {
            _topPanelList.Remove(this);
            base.Destroy();
        }
    }

    public class TopPanelViewController : ViewController<TopPanel>
    {
        private BindableProperty<Button, Action, ButtonActionUpdater> _homeButton;

        private BindableProperty<TextMeshProUGUI, long, CommaNumberUpdater> _coinText;

        private BindableProperty<TextMeshProUGUI, long, CommaNumberUpdater> _diamondText;

        public long coinCountNum = 0;
        public long emeraldCountNum = 0;
        public uint avatarID = 0;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            BindingUIEventHandler();

            InitializeViewData();

            view.btnAvatar.interactable = true;
        }

        protected void InitializeViewData()
        {
            coinCountNum = (long)Client.Get<UserController>().GetCoinsCount();
            emeraldCountNum = (long)Client.Get<UserController>().GetDiamondCount();
            avatarID = Client.Get<UserController>().GetUserAvatarID();

            view.UpdateCoinCount(coinCountNum, 0, false);
            view.UpdateEmeraldCount(emeraldCountNum, 0, false);
            view.UpdateUserAvatar(avatarID);

            view.AddChild<PiggyBankEntranceView>(view.btnPiggyBank.transform);

            RefreshBuyDealButtonState(true);
        }

        public void BindingUIEventHandler()
        {
            view.homeButton.onClick.AddListener(OnHomeClicked);

            view.buyButton.onClick.AddListener(OnBuyButtonClicked);
            view.buyButtonLarge.onClick.AddListener(OnBuyButtonClicked);
            view.dealButton.onClick.AddListener(OnDealButtonClicked);
            view.noticeButton.onClick.AddListener(OnStoreBonusNoticeButtonClicked);
            view.superSpinXButton.onClick.AddListener(OnBuyButtonClicked);


            view.coinButton.onClick.AddListener(OnCoinButtonClicked);
            view.emeraldButton.onClick.AddListener(OnEmeraldButtonClicked);

            view.btnAvatar.onClick.AddListener(OnAvatarButtonClicked);
        }

        private async void OnAvatarButtonClicked()
        {
            view.btnAvatar.interactable = false;

            SoundController.PlayButtonClick();

            await Client.Get<UserProfileInRoleController>().RequestCGetUserProfileInRole();

            view.btnAvatar.interactable = true;
        }

        public override void Update()
        {
            RefreshTopUIState();
        }

        public async void OnExpInfoButtonClicked()
        {
            SoundController.PlayButtonClick();

            if (view.nextLevelInfoView == null)
            {
                view.nextLevelInfoView = await view.AddChild<NextLevelInfoView>();
                view.nextLevelInfoView.Hide();
            }

            if (view.nextLevelInfoView != null && !view.nextLevelInfoView.IsActive())
            {
                view.nextLevelInfoView.ShowLevelInfoView();
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(2);
            RefreshTopUIState();

            UpdateAvatarNewState();
        }

        public void UpdateAvatarNewState()
        {
            view.reminderGroup.gameObject.SetActive(Client.Get<UserController>().HasNewAvatar());
        }
        
        protected void RefreshTopUIState()
        {
            //StoreBonus状态更新
            var collectable = Client.Get<IapController>().IsStoreBonusCollectable();
            if (view.noticeButton.gameObject.activeSelf != collectable)
                view.noticeButton.gameObject.SetActive(collectable);

            var storePaymentHandler = Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>();
            bool spinXAvailable = storePaymentHandler != null && storePaymentHandler.spinXAvailable;
            
            view.superSpinXButton.gameObject.SetActive(!collectable && spinXAvailable);
  
            RefreshBuyDealButtonState();
            
          
        }

        private bool _lastDealEnabled = false;

        private void RefreshBuyDealButtonState(bool forceUpdate = false)
        {
            var bannerController = Client.Get<BannerController>();

            var dealEnabled = bannerController != null && bannerController.DealButtonEnabled();

            if (dealEnabled != _lastDealEnabled || forceUpdate)
            {
                if (dealEnabled)
                {
                    view.buyButtonLarge.gameObject.SetActive(false);
                    view.buyButton.gameObject.SetActive(true);
                    view.dealButton.gameObject.SetActive(true);
                }
                else
                {
                    view.buyButtonLarge.gameObject.SetActive(true);
                    view.buyButton.gameObject.SetActive(false);
                    view.dealButton.gameObject.SetActive(false);
                }

                _lastDealEnabled = dealEnabled;
            }

            if (dealEnabled)
            {
                view.dealTimeText.text = XUtility.GetTimeText(bannerController.GetActiveDealAdvertisementCountDown());
            }
        }

        private void OnBuyButtonClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "Buy")));
        }

        private void OnCoinButtonClicked()
        {
            SoundController.PlayButtonClick();
            if (!view.IsCoinGroupOnSystemPopupLayer())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "CoinIcon")));
            }
        }

        private void OnEmeraldButtonClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond, "EmeraldIcon")));
        }
        private void OnDealButtonClicked()
        {
            var bannerController = Client.Get<BannerController>();
            if (bannerController != null)
                bannerController.TriggerActiveDealOffer("DealButton");
            SoundController.PlayButtonClick();
        }

        private void OnStoreBonusNoticeButtonClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "StoreBonus")));
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventBalanceUpdate>(OnBalanceUpdate);
            SubscribeEvent<EventEmeraldBalanceUpdate>(OnEmeraldBalanceUpdate);
            SubscribeEvent<EventCoinCollectFx>(OnCoinCollectFx);
            SubscribeEvent<EventRefreshUserProfile>(OnRefreshUserProfile);
            SubscribeEvent<EventUpdateRoleInfo>(OnUpdateRoleInfo);
            SubscribeEvent<EventCurrencyUpdate>(OnCurrencyUpdate);
            SubscribeEvent<EventCheckAndShowStoreBonusGuide>(CheckAndShowStoreBonusGuide);
            SubscribeEvent<EventShowNewAvatarGuide>(OnEventShowNewAvatarGuide);
            SubscribeEvent<EventUserNewAvatarStateChanged>(OnNewAvatarStateChanged);

            base.SubscribeEvents();
        }
        
        protected void OnNewAvatarStateChanged(EventUserNewAvatarStateChanged evt)
        {
            UpdateAvatarNewState();
        }

        protected void OnEventShowNewAvatarGuide(EventShowNewAvatarGuide evt)
        {
            view.ShowNewAvatarGuide();
        }

        protected void CheckAndShowStoreBonusGuide(EventCheckAndShowStoreBonusGuide evt)
        {
            view.ShowStoreBonusGuide();
        }

        protected void OnRefreshUserProfile(EventRefreshUserProfile evt)
        {
            coinCountNum = (long)Client.Get<UserController>().GetCoinsCount();
            emeraldCountNum = (long)Client.Get<UserController>().GetDiamondCount();

            view.UpdateCoinCount(coinCountNum, 0, false);
            view.UpdateEmeraldCount(emeraldCountNum, 0, false);
        }

        protected void OnUpdateRoleInfo(EventUpdateRoleInfo evt)
        {
            var newAvatarID = Client.Get<UserController>().GetUserAvatarID();
            if (newAvatarID != avatarID)
            {
                avatarID = newAvatarID;
                view.UpdateUserAvatar(avatarID);
            }
        }

        protected void OnCoinCollectFx(EventCoinCollectFx evt)
        {
            view.ModifyCoinGroupSortingOrder(evt.isFxBegin);
            if (evt.isFxBegin)
            {
                var animator = view.coinGroup.transform.GetComponent<Animator>();
                animator.Play("Open");
            }
        }

        protected void OnBalanceUpdate(EventBalanceUpdate eventBalanceUpdate)
        {
            view.UpdateCoinCount(coinCountNum, eventBalanceUpdate.delta, eventBalanceUpdate.hasAnimation);
            coinCountNum += eventBalanceUpdate.delta;
        }
        protected void OnEmeraldBalanceUpdate(EventEmeraldBalanceUpdate eventBalanceUpdate)
        {
            view.UpdateEmeraldCount(emeraldCountNum, eventBalanceUpdate.delta, eventBalanceUpdate.hasAnimation);
            emeraldCountNum += eventBalanceUpdate.delta;
        }

        
        protected void OnCurrencyUpdate(EventCurrencyUpdate evt)
        {
            view.UpdateCoinCount(coinCountNum, evt.coinDelta, evt.hasAnimation);
            coinCountNum += evt.coinDelta;

            view.UpdateEmeraldCount(emeraldCountNum, evt.emeraldDelta, evt.hasAnimation);
            emeraldCountNum += evt.emeraldDelta;

            view.ModifyEmeraldGroupSortingOrder(true);
            view.ModifyCoinGroupSortingOrder(true);

            WaitForSeconds(evt.delayTime, () =>
            {
                view.ModifyEmeraldGroupSortingOrder(false);
                view.ModifyCoinGroupSortingOrder(false);
            });

        }

        public void OnHomeClicked()
        {
            SoundController.PlayButtonClick();

            if (Client.Get<NewBieQuestController>().IsInQuestMachineScene())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPopup), "HomeBack")));
            }
            else if (Client.Get<SeasonQuestController>().IsInQuestMachineScene())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup), "HomeBack")));
            }
            else
            {
                EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));
            }
        }
    }
}