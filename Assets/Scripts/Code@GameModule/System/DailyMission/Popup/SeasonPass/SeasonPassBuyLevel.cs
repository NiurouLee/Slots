//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 08:51
//  Ver : 1.0.0
//  Description : SeasonPassStore.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UISeasonPassBuyH","UISeasonPassBuyV")]
    public class SeasonPassBuyLevel:Popup<SeasonPassStoreViewController>
    {
        public SeasonPassBuyLevel(string assetAddress)
            : base(assetAddress)
        {
             contentDesignSize = new Vector2(1300, 768);
             contentDesignSizeH = new Vector2(768, 800);
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            for (int i = 1; i <= 3; i++)
            {
                var transItem = transform.Find("Root/MainGroup/ReachLevelCell" + i);
                AddChild<SeasonPassBuyLevelItem>(transItem);
                transItem.gameObject.SetActive(false);
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMissionPassBuyLevelPop, ("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
        }

        public override void OnClose()
        {
            EventBus.Dispatch(new EventSeasonPassUpdate());
        }
    }

    public class SeasonPassStoreViewController : ViewController<SeasonPassBuyLevel>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassCloseBuyLevel>(OnCloseBuyLevel);
        }

        private void OnCloseBuyLevel(EventSeasonPassCloseBuyLevel evt)
        {
            view.Close();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            int index = 0;
            var shopItemConfigs = Client.Get<SeasonPassController>().ShopItemConfigs;
            for (int i = 0; i < shopItemConfigs.Count; i++)
            {
                if (shopItemConfigs[i].ProductType.Contains("missionpass_level"))
                {
                    var childView = view.GetChildView(index++) as SeasonPassBuyLevelItem;
                    childView.Show();
                    childView.UpdateContent(shopItemConfigs[i]);
                    childView.onBtnBuyClick += OnBtnBuyClicked;
                    childView.onBtnExtraClick += OnBtnExtraClicked;
                }
            }
        }

        private void OnBtnBuyClicked(SeasonPassBuyLevelItem child)
        {
            Client.Get<IapController>().BuyProduct(child.ShopItemConfig);
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventMissionPassBuyLevelPurchase,
                ("PaymentId", child.ShopItemConfig.PaymentId.ToString()),
                ("Price", child.ShopItemConfig.Price.ToString()),
                ("Balance",Client.Get<UserController>().GetCoinsCount().ToString()),
                ("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
        }
        
        private async void OnBtnExtraClicked(SeasonPassBuyLevelItem child)
        {
            ShopItemConfig shopItemConfig = child.ShopItemConfig;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
        }
    }
}