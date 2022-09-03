//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:01
//  Ver : 1.0.0
//  Description : SeasonPassPurchase.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UISeasonPassGoldenPassH", "UISeasonPassGoldenPassV")]
    public class SeasonPassPurchaseGolden : Popup<SeasonPassPurchaseViewController>
    {
        public SeasonPassPurchaseGolden(string assetAddress)
            : base(assetAddress)
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            if (isPortrait)
            {
                contentDesignSize = new Vector2(1097, 1365);
            }
            else
            {
                contentDesignSize = new Vector2(1365, 768);
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            for (int i = 1; i <= 2; i++)
            {
                var transItem = transform.Find($"Root/MainGroup/Content{i}");
                AddChild<SeasonPassPurchaseGoldenItem>(transItem);
            }
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventMissionPassUnlockPop,
                ("MissionPassLevel", Client.Get<SeasonPassController>().Level.ToString()));
        }

        [ComponentBinder("Root/MainGroup/Content1/BottomGroup/ExtraContentsButton")]
        private void OnBtnExtraContentsClick()
        {

        }
        [ComponentBinder("Root/MainGroup/Content2/BottomGroup/ExtraContentsButton")]
        private void OnBtnExtraContents0Click()
        {

        }

    }

    public class SeasonPassPurchaseViewController : ViewController<SeasonPassPurchaseGolden>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            int index = 0;
            var shopItemConfigs = Client.Get<SeasonPassController>().ShopItemConfigs;
            for (int i = 0; i < shopItemConfigs.Count; i++)
            {
                if (shopItemConfigs[i].ProductType.Contains("missionpass_unlock"))
                {
                    var childView = view.GetChildView(index++) as SeasonPassPurchaseGoldenItem;
                    childView.UpdateContent(shopItemConfigs[i]);
                    childView.onBtnBuyClick += OnBtnBuyClicked;
                    childView.onBtnExtraClick += OnBtnExtraClicked;
                }
            }
        }

        private void OnBtnBuyClicked(SeasonPassPurchaseGoldenItem child)
        {
            Client.Get<IapController>().BuyProduct(child.ShopItemConfig);
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventMissionPassUnlockPurchase,
                ("PaymentId", child.ShopItemConfig.PaymentId.ToString()),
                ("Price", child.ShopItemConfig.Price.ToString()),
                ("Balance", Client.Get<UserController>().GetCoinsCount().ToString()),
                ("MissionPassLevel", Client.Get<SeasonPassController>().Level.ToString()));
        }

        private async void OnBtnExtraClicked(SeasonPassPurchaseGoldenItem child)
        {
            ShopItemConfig shopItemConfig = child.ShopItemConfig;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassCloseBuyGolden>(OnSeasonPassCloseByGolden);
        }

        private void OnSeasonPassCloseByGolden(EventSeasonPassCloseBuyGolden evt)
        {
            view.Close();
        }
    }
}