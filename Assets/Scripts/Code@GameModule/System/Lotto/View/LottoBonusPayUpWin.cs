using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule {
    [AssetAddress("UILottoBonusPayUpWin")]
    public class LottoBonusPayUpWin: View {
        [ComponentBinder("Button")]
        public Button PayButton;

        [ComponentBinder("Root/ExtraContentsButton")]
        public Button benefitBtn;

        [ComponentBinder("CloseButton")]
        public Button CloseButton;

        [ComponentBinder("Root/UILottoBonusPayStart/RewardGroup/Text")]
        public Text rewardText;

        [ComponentBinder("Root/Button/Text")]
        public Text priceText;

        public LottoBonusPayController parentController;
        public LottoBonusPayUpWin(string address):base(address) {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            var scaleInfo = CalculateScaleInfo();
            transform.localScale = scaleInfo;
        }

        public virtual Vector3 CalculateScaleInfo()
        {
            Vector2 contentDesignSize = new Vector2(1365, 1000);
            if(contentDesignSize == Vector2.zero)
                return Vector3.one;

            var viewSize = ViewResolution.referenceResolutionLandscape;

            if (viewSize.x < contentDesignSize.x)
            {
                var scale = viewSize.x / contentDesignSize.x;
                return new Vector3(scale, scale, scale);
            }

            return Vector3.one;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            PayButton.onClick.AddListener(OnClickPayButton);
            benefitBtn.onClick.AddListener(OnBenefitButtonClicked);
            CloseButton.onClick.AddListener(OnClickCloseButton);
        }

        public void SetBtnStatus(bool interactable)
        {
            PayButton.interactable = interactable;
            benefitBtn.interactable = interactable;
            CloseButton.interactable = interactable;
        }
        private async void OnBenefitButtonClicked()
        {
            var shopItemConfig = parentController.GetShopItemConfig();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
        }

        public void SetViewContent(PayLottoArgsData argsData)
        {
            var coinItem = XItemUtility.GetItem(argsData.freeLottoGameResult.PaidGameInfoForShow.GameCoinsMax.Items, Item.Types.Type.Coin);
            var coin = (long) coinItem.Coin.Amount;
            rewardText.text =  coin.GetCommaOrSimplify(9);  // 最终策划还是说显示这个基础值。。。。
            priceText.text = "$" + argsData.freeLottoGameResult.PaidGameInfoForShow.Price;
        }
        
        public void SetViewController(LottoBonusPayController viewController) {
            parentController = viewController;
        }

        private void OnClickPayButton() {
            // 这里应该是去付费成功再回来开始游戏
            // paySuccess();
            parentController.TryToPay();
        }

        public void paySuccess(Item item)
        {
            SetBtnStatus(false);
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", Hide);
            parentController.StartGame(item);
        }

        private void OnClickCloseButton()
        {
            SetBtnStatus(false);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushPaypop, ("OperationId", "2"));
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", ClosePayView);
        }

        private void ClosePayView() {
            Hide();
            parentController.ClosePopup();
        }

    }
}