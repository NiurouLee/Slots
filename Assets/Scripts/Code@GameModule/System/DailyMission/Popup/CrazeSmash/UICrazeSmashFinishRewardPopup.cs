using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICrazeSmashFinishReward", "UICrazeSmashFinishRewardV")]
    public class UICrazeSmashFinishRewardPopup : Popup<UICrazeSmashFinishRewardPopupController>
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button buttonCollect;

        [ComponentBinder("Root/TopGroup/SilveTitleImage")]
        public Transform transformSilverTitle;

        [ComponentBinder("Root/TopGroup/GoldenTitleImage")]
        public Transform transformGoldTitle;

        [ComponentBinder("Root/MainGroup/RewardGroup/IntegralGroup/IntegralText")]
        public Text textIntegral;

        [ComponentBinder("Root/MainGroup/RewardGroup/BoostGroup")]
        public Transform transformBoost;

        public UICrazeSmashFinishRewardPopup(string address) : base(address) { }

        private void SetCoin(ulong coin)
        {
            if (textIntegral != null) { textIntegral.text = coin.GetCommaFormat(); }
        }

        private void ClearItems()
        {
            SetCoin(0);
            for (int i = transformBoost.childCount - 1; i >= 1; i--)
            {
                GameObject.DestroyImmediate(transformBoost.GetChild(i).gameObject);
            }
        }

      
        
        public void SetItems(RepeatedField<Item> items)
        {
            ClearItems();
            ulong coinAmount = 0;
            var clone = CrazeSmashController.FilterItems(items, out coinAmount);
            if (clone == null) { return; }
            SetCoin(coinAmount);
            XItemUtility.InitItemsUI(transformBoost, clone, transformBoost.GetChild(0));
        }
    }

    public class UICrazeSmashFinishRewardPopupController : ViewController<UICrazeSmashFinishRewardPopup>
    {
        private RepeatedField<Item> rewardItems;
        
        
        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            EventBus.Dispatch(new Event_CrazeSmash_BIG_WIN());
        }
        
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            rewardItems = inExtraData as RepeatedField<Item>;
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            Refresh();
        }

        public void Refresh()
        {
            view.buttonCollect.interactable = true;
            
            var controller = Client.Get<CrazeSmashController>();
            
            if (!controller.playGoldGame)
            {
                view.transformGoldTitle.gameObject.SetActive(false);
                view.transformSilverTitle.gameObject.SetActive(true);
            }
            else
            {
                view.transformGoldTitle.gameObject.SetActive(true);
                view.transformSilverTitle.gameObject.SetActive(false);
            }
            
            view.SetItems(rewardItems);
        }

 
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonCollect.onClick.AddListener(OnButtonCollect);
        }

        private async void OnButtonCollect()
        {
            var items = rewardItems;
            if (items != null && items.count > 0)
            {
                view.buttonCollect.interactable = false;
                ulong coinAmount = 0;
               
                CrazeSmashController.FilterItems(items, out coinAmount);

                await XUtility.FlyCoins(
                        view.buttonCollect.transform,
                        new EventBalanceUpdate(coinAmount, "CrazeSmashCollect")
                    );

                ItemSettleHelper.SettleItems(
                    items,
                    () =>
                    {
                        EventBus.Dispatch(new EventRefreshUserProfile());
                        view.Close();
                    }, 0, "CrazeSmashCollect");
            }
          
        }
    }
}