// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/13:37
// Ver : 1.0.0
// Description : TimeBonusGoldenWheelView.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule
{
    public class TimeBonusGoldenWheelView:TimeBonusWheelView
    {
        [ComponentBinder("PriceText")] 
        public TextMeshProUGUI priceText;
        
        [ComponentBinder("CloseButton")] 
        public Button closeButton;   
        
        [ComponentBinder("BetGroup")] 
        public Transform betGroup;

        [ComponentBinder("BetText")] public TextMeshProUGUI multipleText;

        [ComponentBinder("ExtraContentsButton")]
        public Button purchaseBenefitsButton;

        private SGetGoldenWheelBonus _sGetGoldenWheelBonus;
        public virtual void InitializeExtraUI(SGetGoldenWheelBonus sGetGoldenWheelBonus)
        {
            _sGetGoldenWheelBonus = sGetGoldenWheelBonus;
            priceText.text = "$" + sGetGoldenWheelBonus.PayItem.Price;
            multipleText.text = $"X{sGetGoldenWheelBonus.GodenOdds}";
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
           
            if(purchaseBenefitsButton!= null)
                purchaseBenefitsButton.onClick.AddListener(OnPurchaseBenefitsClicked);
        }

        protected async void OnPurchaseBenefitsClicked()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(_sGetGoldenWheelBonus.PayItem.SubItemList);
        }

        public void HidePurchaseBenefitsUI()
        {
            purchaseBenefitsButton.gameObject.SetActive(false);
        }

        public void HideMultipleArrow(bool directly = true)
        {
            var multipleAnimator = betGroup.GetComponent<Animator>();
            if (multipleAnimator != null)
                multipleAnimator.Play("Close", 0, 0);

            if (directly)
            {
                betGroup.parent.gameObject.SetActive(false);
            }
        }
    }
}