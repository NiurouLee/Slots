using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule {
    [AssetAddress("UILottoBonusPayMultiple")]
    public class LottoBonusPayMultiple: View {
        [ComponentBinder("Button")]
        public Button PayButton;

        [ComponentBinder("Root/BGGroup/BG/MainGroup/ButtonGroup/ExtraContentsButton")]
        public Button benefitBtn;

        [ComponentBinder("CloseButton")]
        public Transform CloseButton;

        [ComponentBinder("Multiple")]
        public Text multipleText;

        [ComponentBinder("Text01")]
        public Text priceText;

        public bool isHide = false;

        public LottoBonusPayController parentController;
        public LottoBonusPayMultiple(string address):base(address) {
            
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
            CloseButton.gameObject.AddComponent<Button>().onClick.AddListener(OnClickCloseButton);
        }

        private async void OnBenefitButtonClicked()
        {
            var shopItemConfig = parentController.GetShopItemConfig();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
        }

        public void SetViewController(LottoBonusPayController viewController) {
            parentController = viewController;
        }

        public void SetViewContent(PayLottoArgsData argsData)
        {
            multipleText.text = "X" + argsData.freeLottoGameResult.PaidGameInfoForShow.MaxOdds;
            priceText.text = "$" + argsData.freeLottoGameResult.PaidGameInfoForShow.Price;
        }

        public void SetBtnStatus(bool interactable)
        {
            PayButton.interactable = interactable;
            benefitBtn.interactable = interactable;
            CloseButton.gameObject.GetComponent<Button>().interactable = interactable;
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
            parentController.TryToShowUpWin();
        }
        
        public override void Hide()
        {
            base.Hide();
            isHide = true;
        }

        public override void Show()
        {   
            base.Show();
            isHide = false;
        }
    }
}