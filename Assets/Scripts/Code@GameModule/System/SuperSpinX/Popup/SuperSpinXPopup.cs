// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/20/17:03
// Ver : 1.0.0
// Description : SuperSpinXPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
namespace GameModule
{
    public class PayItemView : View
    {
        [ComponentBinder("__NumberText")] 
        public Text numberText;
    }
    public class SpinXMachineView : View<ViewController>
    {
        [ComponentBinder("__NumberText","TextGroup2")] 
        public Text threeJpWinRateText;
        
        [ComponentBinder("__NumberText","TextGroup3")] 
        public Text anyThreeWinRate;
        
        [ComponentBinder("__NumberText","TextGroup4")] 
        public Text twoJackpotWinRate;

        [ComponentBinder("RewardGroup")] 
        public Transform anyTwoWinRateGroup;  
        
        [ComponentBinder("__NormalButton")] 
        public Button spinButton;

        public List<PayItemView> payItemViewList;

        [ComponentBinder("ListGroup")] 
        public Transform listGroup;

        public List<Transform> listElementTransform; 
        public List<Transform> listResultTransform; 
        public List<Transform> listStartTransform;

        public Animator animator;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            var index = anyTwoWinRateGroup.childCount;
            
            payItemViewList = new List<PayItemView>();
            
            for (var i = 0; i < index; i++)
            {
                var payItemView = AddChild<PayItemView>(anyTwoWinRateGroup.GetChild(i));
                payItemViewList.Add(payItemView);
            }

            CollectIconTransform();

            animator = transform.GetComponent<Animator>();
            spinButton.interactable = false;
        }

        public void CollectIconTransform()
        {
            listElementTransform = new List<Transform>();
            listResultTransform = new List<Transform>();
            listStartTransform = new List<Transform>();
            
            var childCount = listGroup.childCount;
            
            for (var i = 0; i < childCount; i++)
            {
                var child = listGroup.GetChild(i);
               
                var content = child.Find("Viewport/Content");

                if (content != null)
                {
                    int contentChildCount = content.childCount;

                    for (var j = 7; j < 7 + contentChildCount; j++)
                    {
                        var index = j % contentChildCount;
                        
                        listElementTransform.Add(content.GetChild(index));

                        if (index == 1)
                        {
                            listResultTransform.Add(content.GetChild(index));
                        }

                        if (index == 7)
                        {
                            listStartTransform.Add(content.GetChild(index));
                        }
                    }
                }
            }
        }
        
        public void InitReels(RepeatedField<uint> reels, SpriteAtlas spriteAtlas)
        {
            var colCount = listElementTransform.Count / 3;
            for (var i = 0; i < listElementTransform.Count/3; i++)
            {
                var spriteName = $"ui_superSpinx_machine_lottery_icon{reels[i % reels.count]}";
                var sprite = spriteAtlas.GetSprite(spriteName);
                if (sprite)
                {
                    listElementTransform[i].Find("__LightIcon").GetComponent<Image>().sprite = sprite;
                    listElementTransform[colCount + i].Find("__LightIcon").GetComponent<Image>().sprite = sprite;
                    listElementTransform[colCount * 2 + i].Find("__LightIcon").GetComponent<Image>().sprite = sprite;
                }
            }
        }

        public void SetSpinResult(RepeatedField<uint> result, SpriteAtlas spriteAtlas)
        {
            for (var i = 0; i < listResultTransform.Count; i++)
            {
                var spriteName = $"ui_superSpinx_machine_lottery_icon{result[i % result.count]}";
                var sprite = spriteAtlas.GetSprite(spriteName);
                if (sprite)
                {
                    listResultTransform[i].Find("__LightIcon").GetComponent<Image>().sprite = sprite;
                }
            }
        }

        public void SetUpPayRate(RepeatedField<SuperSpinXGameInItem.Types.SpinXGameRule> gameRules)
        {
            if (gameRules.count > 0)
            {
                for (int i = 0; i < gameRules.count; i++)
                {
                    var gameRule = gameRules[i];
                    
                    if (gameRule.WordId != 0 && gameRule.WordId != 1 && gameRule.Count == 3)
                    {
                        anyThreeWinRate.text = "X" + gameRule.Odds.ToString();
                    } 
                    
                    if (gameRule.WordId == 1 && gameRule.Count == 3)
                    {
                        threeJpWinRateText.text = "X" + gameRule.Odds.ToString();
                    } 
                    
                    if (gameRule.WordId == 1 && gameRule.Count == 2)
                    {
                        twoJackpotWinRate.text = "X" +  gameRule.Odds.ToString();
                    }
                    
                    if (gameRule.Count == 2 || (gameRule.WordId == 0 && gameRule.Count == 3))
                    {
                        var payItem = anyTwoWinRateGroup.Find("PayItem" + gameRule.WordId);
                        if (payItem != null)
                        {
                            var numberTextTrans = payItem.Find("__NumberText");
                            if (numberTextTrans != null)
                            {
                                var numberText = numberTextTrans.GetComponent<Text>();
                                if (numberText != null)
                                    numberText.text = "X" + gameRule.Odds.ToString();
                            }
                        }
                    }
                }
            }
        }

        public void PlayGuideAnimation()
        {
            animator.Play("Guide");
            SoundController.PlaySfx("SuperSpinX_Arrow");
        }
        
        public async Task StartSpin(RepeatedField<uint> spinResult, SpriteAtlas spriteAtlas)
        {
            viewController.WaitForSeconds(1, () => { SetSpinResult(spinResult, spriteAtlas); });
            SoundController.PlaySfx("SuperSpinX_WheelScroll");
            await XUtility.PlayAnimationAsync(animator, "Spin_Start");
        }
        
        public async Task ShowWinRule(SuperSpinXGameInItem.Types.SpinXGameRule gameRule)
        {
            if (gameRule.WordId != 0 &&  gameRule.WordId != 1 && gameRule.Count == 3)
            {
                anyThreeWinRate.transform.parent.GetComponent<Animator>().Play("Show");
                return;
            }

            if (gameRule.WordId == 1 && gameRule.Count == 3)
            {
                threeJpWinRateText.transform.parent.GetComponent<Animator>().Play("Show");
                return;
            }

            if (gameRule.WordId == 1 && gameRule.Count == 2)
            {
                twoJackpotWinRate.transform.parent.GetComponent<Animator>().Play("Show");
                return;
            }

            if (gameRule.Count == 2 || (gameRule.WordId == 0 && gameRule.Count == 3))
            {
                var payItem = anyTwoWinRateGroup.Find("PayItem" + gameRule.WordId);
                if (payItem != null)
                {
                    payItem.GetComponent<Animator>().Play("Show");
                }
            }
        }
    }

    public class SpinXCollectView : View
    {
        [ComponentBinder("__NumberText", "CollectGroup")]
        public Text baseNumberText;

        [ComponentBinder("__NumberText")] 
        public Text multipleText;
    }
    public class SpinXBeforePurchaseView : View
    {
        [ComponentBinder("__Normal")] public Transform normalView;
        [ComponentBinder("__Normal/Text")] public Text maxOddsText;
        [ComponentBinder("__Activity")] public Transform activityView;
        [ComponentBinder("__GreenButton")] public Button buyButton;
        [ComponentBinder("__BlueButton")] public Button benefitsButton;
        [ComponentBinder("__Label")] public Text priceText;

        public void SetUpBeforePurchaseView(float prize, float maxOdds, bool hasWildActivity, bool hasCardActivity)
        {
            if (hasWildActivity || hasCardActivity)
            {
                normalView.gameObject.SetActive(false);
                activityView.gameObject.SetActive(true);

                activityView.Find("__WildActivity").gameObject.SetActive(hasWildActivity);
                activityView.Find("__CardActivity").gameObject.SetActive(hasCardActivity);
            }
            else
            {
                normalView.gameObject.SetActive(true);
                activityView.gameObject.SetActive(false);
            }

            maxOddsText.text = "X" + maxOdds;
            priceText.text = $"${prize}";
        }
    }

    public class SpinXStateGroupView : View
    {
        [ComponentBinder("__PaymentTip")] 
        public Transform paymentTip;

        [ComponentBinder("__Collect")]
        public Transform collectGroup;
        
        [ComponentBinder("__Collect/CollectGroup/__NumberText")] 
        public Text finalMultiplierText;

        [ComponentBinder("__BeforePurchase")]
        public Transform beforePurchase;

        public SpinXBeforePurchaseView beforePurchaseView;
        public SpinXCollectView collectView;
        protected override void SetUpExtraView()
        {
            beforePurchaseView = AddChild<SpinXBeforePurchaseView>(beforePurchase);
            collectView = AddChild<SpinXCollectView>(collectGroup);
            base.SetUpExtraView();
        }
    }
    
    [AssetAddress("UISuperSpinxMainPanel","","UISuperSpinxMainPanel_Pad","UISuperSpinxMainPanel_Pad")]
    public class SuperSpinXPopup:Popup<SuperSpinXPopupViewController>
    {
        [ComponentBinder("MachineGroup")] 
        public Transform machineGroup;
        
        [ComponentBinder("StateGroup")]
        public Transform stateTransform;
        
        [ComponentBinder("HelpButton")] 
        public Button helpButton;

        public SpinXMachineView machineView;
        public SpinXStateGroupView stateView;

        public SuperSpinXPopup(string address)
            : base(address)
        {
            
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            machineView = AddChild<SpinXMachineView>(machineGroup);
            stateView = AddChild<SpinXStateGroupView>(stateTransform);
        }

        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }
        
    }

    public class SuperSpinXPopupViewController : ViewController<SuperSpinXPopup>
    {
        private VerifyExtraInfo _verifyExtraInfo;
        private FulfillExtraInfo _fulfillExtraInfo;
        private Action _fulfillFxEndCallback;

        private bool _closeClicked = false;

        private ShopItemConfig _shopItemConfig;

        private Action<Action<FulFillCallbackArgs>> _fullFillRequestHandler;

        private SpriteAtlas iconSpriteAtlas;

        public override async  Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {"SuperSpinXSlotIconAtlas"};
            await base.LoadExtraAsyncAssets();
            
            iconSpriteAtlas = GetAssetReference("SuperSpinXSlotIconAtlas").GetAsset<SpriteAtlas>();
            
            //view.machineView.InitReels(null, iconSpriteAtlas);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            if (_shopItemConfig != null)
            {
                var superSpinxGameItem = XItemUtility.GetItem(_shopItemConfig.SubItemList, Item.Types.Type.SuperSpinxGame);
              
                if (superSpinxGameItem != null)
                {
                    SetUpViewData(superSpinxGameItem.SuperSpinxGame.GameInfo);
                    
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSuperspinxPlay,("Price",_shopItemConfig.Price.ToString()));
                }
            }
            
            SoundController.PlayBgMusic("SuperSpinX_Bg");
        }
        

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
           
            if (inExtraData != null)
            {
                var popupArgs = inExtraData as PopupArgs;
                if (popupArgs != null && popupArgs.extraArgs != null)
                {
                    _shopItemConfig = popupArgs.extraArgs as ShopItemConfig;
                }
            }
        }

        public void SetUpViewData(SuperSpinXGameInItem superSpinXGameInItem)
        {
            //if(superSpinXGameInItem.Result.WordsResult)
            if (_shopItemConfig != null)
            {
                var hasWildActivity = false;//Client.Get<ActivityController>().GetDefaultActivity(ActivityType.SuperSpinGiveCard);
                var hasCardActivity = superSpinXGameInItem.CardRewardExtra != null && superSpinXGameInItem.CardRewardExtra.Items.count > 0;
                view.stateView.beforePurchaseView.SetUpBeforePurchaseView(_shopItemConfig.Price,superSpinXGameInItem.OddsMax, hasWildActivity, hasCardActivity);
            }

            view.machineView.InitReels(superSpinXGameInItem.WordIds, iconSpriteAtlas);    
           // superSpinXGameInItem.OddsMax;
            view.machineView.SetUpPayRate(superSpinXGameInItem.RuleList);
            
            view.stateView.collectView.Hide();
        }

        public void SetPurchaseContent(VerifyExtraInfo verifyExtraInfo,
            Action<Action<FulFillCallbackArgs>> fullFillRequestHandler, bool isReplenishmentOrder = false)
        {
            _verifyExtraInfo = verifyExtraInfo;
            _fullFillRequestHandler = fullFillRequestHandler;
            
            view.stateView.beforePurchaseView.Hide();
            view.stateView.paymentTip.gameObject.SetActive(true);
            view.closeButton.gameObject.SetActive(false);
            view.machineView.spinButton.interactable = true;
            view.machineView.PlayGuideAnimation();
            

            if (verifyExtraInfo != null)
            {
                _shopItemConfig = verifyExtraInfo.Item;
                if (_shopItemConfig != null && isReplenishmentOrder)
                {
                    var superSpinxGameItem =
                        XItemUtility.GetItem(_shopItemConfig.SubItemList, Item.Types.Type.SuperSpinxGame);

                    if (superSpinxGameItem != null)
                    {
                        SetUpViewData(superSpinxGameItem.SuperSpinxGame.GameInfo);
                    }
                }
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.machineView.spinButton.onClick.AddListener(OnSpinButtonClicked);
            view.helpButton.onClick.AddListener(OnHelpButtonClicked);
            view.stateView.beforePurchaseView.buyButton.onClick.AddListener(OnPurchaseButtonClicked);
            view.stateView.beforePurchaseView.benefitsButton.onClick.AddListener(OnPurchaseBenefitsButtonClicked);

            SubscribeEvent<EventOnSuperSpinXPurchaseSucceed>(OnEventOnSuperSpinXPurchaseSucceed);
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
            SubscribeEvent<EventPaymentFinish>(OnEventPaymentFinish);
        }

        protected void OnHelpButtonClicked()
        {
            PopupStack.ShowPopupNoWait<Popup>("UISuperSpinxTipPanel");
        }

        protected void OnEventPaymentFinish(EventPaymentFinish evt)
        {
            if (evt.verifyExtraInfo != null && evt.verifyExtraInfo.Item.ShopType == ShopType.SuperSpinX)
            {
                view.Close();
            }
        }
        
        protected void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType == ActivityType.SuperSpinGiveCard)
            {
                if (_shopItemConfig != null)
                {
                    var superSpinxGameItem =
                        XItemUtility.GetItem(_shopItemConfig.SubItemList, Item.Types.Type.SuperSpinxGame);

                    if (superSpinxGameItem != null)
                    {
                        var hasWildActivity = false;
                        var hasCardActivity = false;
                        view.stateView.beforePurchaseView.SetUpBeforePurchaseView(_shopItemConfig.Price,
                            superSpinxGameItem.SuperSpinxGame.GameInfo.OddsMax, hasWildActivity, hasCardActivity);
                    }
                }
            }
        }

        public void OnEventOnSuperSpinXPurchaseSucceed(EventOnSuperSpinXPurchaseSucceed evt)
        {
            SetPurchaseContent(evt.verifyExtraInfo, evt.fulFillRequestHandler,false);
        }
        
        public async void OnSpinButtonClicked()
        {
            view.machineView.spinButton.interactable = false;
          
            SoundController.PlaySfx("SuperSpinX_ClickSpin");
            //TestCode
            // var result = new SuperSpinXGameInItem.Types.SpinXGameResult();
            // result.Odds = 10;
            // result.CoinsBase = 9810120120000;
            // result.WordIdsResult.Add(1);
            // result.WordIdsResult.Add(1);
            // result.WordIdsResult.Add(2);
           
            //TestCode
            var shopItemConfig = _verifyExtraInfo.Item;
            var spinXGameItem = XItemUtility.GetItem(shopItemConfig.SubItemList, Item.Types.Type.SuperSpinxGame);
            var result = spinXGameItem.SuperSpinxGame.GameInfo.Result;
          
           if (result != null)
            {
                var wordIdsResult = result.WordIdsResult;
                if (wordIdsResult != null)
                {
                    view.stateView.paymentTip.gameObject.SetActive(false);
                    view.stateView.collectView.Show();
                    view.stateView.collectView.baseNumberText.text = result.CoinsBase.GetCommaFormat();
                    view.stateView.collectView.multipleText.text = "X" + result.Odds;
                    
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSuperspinxWinmultiple, ("ruleIndex", result.RuleIndex.ToString()));
                    SoundController.PlaySfx("SuperSpinX_Wait");
                    await view.machineView.StartSpin(wordIdsResult, iconSpriteAtlas);
                    SoundController.PlaySfx("SuperSpinX_StopWin");
                    //

                    //testCode    
                    // var gameRule = new SuperSpinXGameInItem.Types.SpinXGameRule();
                    // gameRule.Count = 2;
                    // gameRule.WordId = 3;

                    var rule = spinXGameItem.SuperSpinxGame.GameInfo.RuleList[(int)result.RuleIndex];
                    
                    await view.machineView.ShowWinRule(rule);
                    
                    await  WaitForSeconds(2);
 
                    var collectPopup = await PopupStack.ShowPopup<SuperSpinXRewardCollectPopup>();
                    collectPopup.viewController.SetUpViewContent(_verifyExtraInfo, (callback) =>
                    {
                        _fullFillRequestHandler.Invoke((fulFillArgs) =>
                            {
                                collectPopup.SubscribeCloseAction(() =>
                                {
                                    if (fulFillArgs.isSuccess)
                                    {
                                        fulFillArgs.fullFillFxEndCallback?.Invoke();
                                    }
                                    else
                                    {
                                        view.Close();
                                    }
                                });
                                
                                callback.Invoke(fulFillArgs.isSuccess, fulFillArgs.fulfillExtraInfo);
                            }
                        );
                    });
                }
            }
            else
            {
                XDebug.Log("SuperSpinX:GameResult IsNull");
            }

            //Show X
        }

        public void OnPurchaseButtonClicked()
        {
            if(_shopItemConfig!= null)
                Client.Get<IapController>().BuyProduct(_shopItemConfig);
            
            //TestCode
            //SetPurchaseContent(null,null);
        }
        
        public async void OnPurchaseBenefitsButtonClicked()
        {
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(_shopItemConfig.SubItemList);
        }

        public override void OnViewDestroy()
        {
            SoundController.RecoverLastMusic();
            base.OnViewDestroy();
        }
    }
}