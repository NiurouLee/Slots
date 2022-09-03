using UnityEngine;

namespace GameModule
{
    [AssetAddress("UICrazeSmashBuy", "UICrazeSmashBuyV")]
    public class UICrazeSmashBuy : Popup<UICrazeSmashBuyController>
    {
        [ComponentBinder("Root/MainGroup/GoldGroup")]
        public Transform transformGoldGroup;

        [ComponentBinder("Root/MainGroup/SilverGroup")]
        public Transform transformSilverGroup;

        public UICrazeSmashBuyItem[] silverItems;
        public UICrazeSmashBuyItem[] goldItems;


        public UICrazeSmashBuy(string address) : base(address) { }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            silverItems = new UICrazeSmashBuyItem[2];
            goldItems = new UICrazeSmashBuyItem[2];

            for (int i = 0; i < 2; i++)
            {
                silverItems[i] = AddChild<UICrazeSmashBuyItem>(transformSilverGroup.GetChild(i));
                goldItems[i] = AddChild<UICrazeSmashBuyItem>(transformGoldGroup.GetChild(i));
            }

            transformSilverGroup.gameObject.SetActive(false);
            transformGoldGroup.gameObject.SetActive(false);
        }
    }

    public class UICrazeSmashBuyController : ViewController<UICrazeSmashBuy>
    {
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            Refresh();
        }

        private void Refresh()
        {
            var controller = Client.Get<CrazeSmashController>();
            if (controller == null || controller.eggInfo == null) { return; }
            var eggInfo = controller.eggInfo;
            var playGoldGame = controller.playGoldGame;
            if (playGoldGame)
            {
                view.transformGoldGroup.gameObject.SetActive(true);
                view.transformSilverGroup.gameObject.SetActive(false);

                var shopItems = controller.goldEggShopItems;
                if (shopItems == null) { return; }

                for (int i = 0; i < 2; i++)
                {
                    var item = view.goldItems[i];
                    if (i < shopItems.Length)
                    {
                        var shopItem = shopItems[i];
                        item.transform.gameObject.SetActive(true);
                        item.Set(shopItem.HammerCount, shopItem.Item);
                    }
                    else
                    {
                        item.transform.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                view.transformGoldGroup.gameObject.SetActive(false);
                view.transformSilverGroup.gameObject.SetActive(true);

                var shopItems = controller.silverEggShopItems;
                if (shopItems == null) { return; }

                for (int i = 0; i < 2; i++)
                {
                    var item = view.silverItems[i];
                    if (i < shopItems.Length)
                    {
                        var shopItem = shopItems[i];
                        item.transform.gameObject.SetActive(true);
                        item.Set(shopItem.HammerCount, shopItem.Item);
                    }
                    else
                    {
                        item.transform.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}