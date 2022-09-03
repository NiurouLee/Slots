//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-14 14:29
//  Ver : 1.0.0
//  Description : UIPiggyBankMainPopup.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIPiggyBankMain")]
    public class PiggyBankMainPopup: Popup<PiggyBankMainPopupViewController>
    {
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        public TextMeshProUGUI textBaseChips;
        [ComponentBinder("Root/MainGroup/IntegralGroup/DescriptionText")]
        public TextMeshProUGUI textFirstBonus;
        [ComponentBinder("Root/MainGroup/IntegralGroup/ExtraIntegralText")]
        public TextMeshProUGUI textExtraChips;
        [ComponentBinder("Root/MainGroup/InformationButton")]
        public Button btnInformation;

        [ComponentBinder("Root/MainGroup/BreakItButton")]
        public Button btnBreakBank;
        [ComponentBinder("Root/MainGroup/BreakItButton/PriceText")]
        public Text txtBankPrice;

        [ComponentBinder("Root/MainGroup/PurchaseButton")]
        public Button btnPurchase;
        
        [ComponentBinder("Root/MainGroup")]
        public Transform transMainGroup;

        [ComponentBinder("Root/BGParticle")]
        public Transform transBGParticle;
        
        [ComponentBinder("Root/TopGroup")]
        public Transform transTopGroup;

        [ComponentBinder("Particle")] 
        public Transform transParticle;
        
        [ComponentBinder("TagIcon")]
        public Animator animatorPigFull;

        public PiggyBankMainPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            RefreshUI();
        }

        public void RefreshUI()
        {
            var needShowFirstBonus = Client.Get<PiggyBankController>().FirstBonus > 0;
            textBaseChips.text = Client.Get<PiggyBankController>().CurrentCoins.GetCommaFormat();
            textExtraChips.text = Client.Get<PiggyBankController>().FirstBonus.GetCommaFormat();
            txtBankPrice.text = "$"+Client.Get<PiggyBankController>().ShopItemConfig.Price;
            textExtraChips.gameObject.SetActive(needShowFirstBonus);
            textFirstBonus.gameObject.SetActive(needShowFirstBonus);
        }

        public void TogglePiggy(bool visible)
        {
            transParticle.gameObject.SetActive(visible);
            transTopGroup.gameObject.SetActive(visible);
            transMainGroup.gameObject.SetActive(visible);
            transBGParticle.gameObject.SetActive(visible);
        }
        
        public async void ShowPiggyAfterBreak()
        {
            TogglePiggy(true);
            CheckAndPlayFull(false);
            SoundController.PlaySfx("General_Levelup");
            await XUtility.PlayAnimationAsync(animator, "PigAppear");
        }
        
        public void HidePiggyOnBreak()
        {
            XUtility.PlayAnimation(animator, "PigDisAppear");
        }
        

        public void CheckAndPlayFull(bool isFull)
        {
            animatorPigFull.gameObject.SetActive(isFull);
            if (isFull)
            {
                XUtility.PlayAnimation(animatorPigFull, "Appear");
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            CheckAndPlayFull(Client.Get<PiggyBankController>().IsPiggyFull);
        }

        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }

        public override void Close()
        {
            base.Close();
            SoundController.RecoverLastMusic();
        }

        public override float GetPopUpMaskAlpha()
        {
            return 0f;
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            EventBus.Dispatch(new EventTriggerPopupPool("ClosePiggyBankMainPopup"));
        }
    }

    public class PiggyBankMainPopupViewController : ViewController<PiggyBankMainPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            BindViewEvent();
            EnableUpdate(1);
            SubscribeEvent<EventPiggyBankUpdated>(OnPiggyBankUpdated);
            SubscribeEvent<EventPiggyConsumeComplete>(OnPiggyConsumeComplete);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SoundController.PlayBgMusic("PiggyBank_BGM");
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            if (inExtraData != null)
            {
                var popupArgs = inExtraData as PopupArgs;

                if (popupArgs != null)
                {
                    var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

                    string sceneType = "Lobby";

                    if (machineScene != null)
                    {
                        sceneType = Client.Get<MachineLogicController>().LastGameId;
                    }

                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPiggyBankOpen,
                        ("source", popupArgs.source), ("scene", sceneType),("isFull",Client.Get<PiggyBankController>().IsPiggyFull.ToString()));
                }
            }
        }

        protected void BindViewEvent()
        {
            view.btnInformation.onClick.AddListener(OnBtnInformationClick);
            view.btnBreakBank.onClick.AddListener(OnBtnBreakItClick);
            view.btnPurchase.onClick.AddListener(OnBtnPurchaseClick);
        }
        private async void OnBtnInformationClick()
        {
            SoundController.PlayButtonClick();
            view.btnInformation.enabled = false;
            await PopupStack.ShowPopup<PiggyBankHelpPopup>();
            view.btnInformation.enabled = true;
        }
        
        private void OnBtnBreakItClick()
        {
            var shopItemConfig = Client.Get<PiggyBankController>().ShopItemConfig;
            Client.Get<IapController>().BuyProduct(Client.Get<PiggyBankController>().ShopItemConfig);
            SoundController.PlayButtonClick();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPiggyBankPurchase,
                ("paymentId", shopItemConfig.PaymentId.ToString()),
                ("price", shopItemConfig.Price.ToString()),
                ("productType", shopItemConfig.ProductType));
            
        }
        private async void OnBtnPurchaseClick()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(Client.Get<PiggyBankController>().ShopItemConfig.SubItemList);
        }

        private void OnPiggyBankUpdated(EventPiggyBankUpdated evt)
        {
            view.RefreshUI();
            view.ShowPiggyAfterBreak();
        }
        
        private void OnPiggyConsumeComplete(EventPiggyConsumeComplete evt)
        {
            view.HidePiggyOnBreak();
        }
    }
}