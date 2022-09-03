// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/31/21:16
// Ver : 1.0.0
// Description : CoinDashItemView.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.Protocol.BiEventFortuneX;

namespace GameModule
{
    [AssetAddress("CoinDashItem")]
    public class CoinDashItemView : View<CoinDashItemController>
    {
        [ComponentBinder("CenterContent/TextGroup/Text")]
        public Text text;

        [ComponentBinder("CenterContent")] 
        public Transform centerContent;

        [ComponentBinder("CenterContent/OriginQuantityGroup/OriginQuantityText")]
        public TMP_Text originQuantityText;

        [ComponentBinder("State/Button")] public Button button;
 
        [ComponentBinder("State/Button/PriceText")]
        public Text priceText;

        [ComponentBinder("State/Button/Lock")] public RectTransform lockTransform;

        [ComponentBinder("State/FinishState")] public RectTransform finishState;

        [ComponentBinder("Benefits")] public Button benefits;

        [ComponentBinder("Sale")] public RectTransform sale;

        [ComponentBinder("Sale/SaleText")] public TextMeshProUGUI saleText;

        public CoinDashItemView()
        {
        }
    }

    public class CoinDashItemController : ViewController<CoinDashItemView>
    {
        private Animator _animator;
        private CoinDashInfo.Types.Goods itemInfo;
        private bool isActive;
        private int viewIndex;
        private bool waitForPurchaseComplete;
        protected override void SubscribeEvents()
        {
            view.button.onClick.AddListener(OnButtonClicked);
            view.benefits.onClick.AddListener(OnPurchaseBenefitsButtonClicked);
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _animator = view.transform.GetComponent<Animator>();
        }

        public void OnItemActivated()
        {
            isActive = true;
            view.lockTransform.gameObject.SetActive(false);

            if (!itemInfo.IsFree)
            {
                view.benefits.gameObject.SetActive(true);
            }
        }

        public void SetUpDashItemView(int inViewIndex, CoinDashInfo.Types.Goods dashItemInfo, bool inIsActive)
        {
            itemInfo = dashItemInfo;
            isActive = inIsActive;
            viewIndex = inViewIndex;
            waitForPurchaseComplete = false;
            
            if (!dashItemInfo.HasBought)
            {
                if (dashItemInfo.IsFree)
                {
                    _animator.Play("Free");

                    var coinItem = XItemUtility.GetCoinItem(dashItemInfo.FreeRewards.Items);
                    view.text.text = ((long) (coinItem.Coin.Amount)).GetCommaOrSimplify();
                    view.sale.gameObject.gameObject.SetActive(false);
                    view.benefits.gameObject.SetActive(false);
                    var animator = view.centerContent.GetComponent<Animator>();
                    animator.Play("idle");

                    view.priceText.text = "FREE";
                }
                else
                {
                    _animator.Play("Pay");
                    var shopCoinItem = XItemUtility.GetItem(dashItemInfo.ShopItemConfig.SubItemList,
                        Item.Types.Type.ShopCoin);
                    view.text.text = ((long) (shopCoinItem.ShopCoin.AdditionAmount)).GetCommaOrSimplify();
                    view.originQuantityText.text = ((long) (shopCoinItem.ShopCoin.Amount)).GetCommaOrSimplify();
                    view.saleText.text = dashItemInfo.ShopItemConfig.ShowDiscount + "%";
                    view.benefits.gameObject.SetActive(isActive);
                    view.sale.gameObject.SetActive(true);
                   
                    view.priceText.text = $"${dashItemInfo.ShopItemConfig.Price}";
                    
                    var animator = view.centerContent.GetComponent<Animator>();
                    animator.Play("discount");
                }
                
                view.lockTransform.gameObject.SetActive(!isActive);
            }
            else
            {
                _animator.Play("Idle");
                view.lockTransform.gameObject.SetActive(false);
                view.benefits.gameObject.SetActive(false);
            }
        }

        public void OnButtonClicked()
        {
            if (!isActive)
            {
                return;
            }
            
            if (itemInfo.IsFree)
            {
                view.button.interactable = false;
                var dashPopup = (CoinDashPopup) view.GetParentView();
                dashPopup.viewController.OnDashViewClaimRewardBegin(viewIndex);
                dashPopup.closeButton.interactable = false;
                
                Client.Get<CoinDashController>().ClaimDashReward(itemInfo.Id, async (reward) =>
                {
                    if (!view.transform)
                        return;

                    if (reward != null)
                    {
                        var coinItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Coin);

                        if (coinItem != null)
                        {
                            await XUtility.FlyCoins(view.button.transform,
                                new EventBalanceUpdate(coinItem.Coin.Amount, "CoinDash"));
                        }
 
                        _animator.Play("Get");
                        
                        await WaitForSeconds(1.5f);
                        dashPopup.viewController.OnDashViewClaimRewardFinished(viewIndex);
                        dashPopup.closeButton.interactable = true;
                    }
                    else
                    {
                        dashPopup.viewController.OnDashViewClaimRewardWithError(viewIndex);
                        view.button.interactable = true;
                        dashPopup.closeButton.interactable = true;
                    }
                });
            }
            else
            {
                Client.Get<IapController>().BuyProduct(itemInfo.ShopItemConfig);
                waitForPurchaseComplete = true;
            }
        }

        public async void OnEventCoinDashDataUpdate()
        {
            if (waitForPurchaseComplete)
            {
                var dashItemInfo = Client.Get<CoinDashController>().GetCoinDashItemInfo(viewIndex);
                if (dashItemInfo.HasBought)
                {
                    _animator.Play("Get");
                    view.benefits.gameObject.SetActive(false);
                    await WaitForSeconds(1.5f);
                    var dashPopup = (CoinDashPopup) view.GetParentView();
                    dashPopup.viewController.OnDashViewClaimRewardFinished(viewIndex);
                    waitForPurchaseComplete = false;
                }
            }
        }

        protected async void OnPurchaseBenefitsButtonClicked()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(itemInfo.ShopItemConfig.SubItemList);
        }
    }
}