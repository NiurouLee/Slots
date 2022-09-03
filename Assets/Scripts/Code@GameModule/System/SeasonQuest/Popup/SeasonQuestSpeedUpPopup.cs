// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/18/12:23
// Ver : 1.0.0
// Description : SeasonQuestSpeedUpPopup.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOneBuffSpeedUp")]
    public class SeasonQuestSpeedUpPopup : Popup<SeasonQuestSpeedUpPopupViewController>
    {
        [ComponentBinder("PriceButton")] 
        public Button button;  
        
        [ComponentBinder("Root/BottomGroup/PriceButton/ContentGroup/CountText")] 
        public TMP_Text countText;   
        
        [ComponentBinder("Root/MainGroup/DescriptionGroup1/Cell1/DescriptionText2")] 
        public TextMeshProUGUI descriptionText2; 
        
        [ComponentBinder("BubbleGroup")] 
        public Transform bubbleGroup; 
         
        public SeasonQuestSpeedUpPopup(string address)
            : base(address)
        {
             contentDesignSize = ViewResolution.designSize;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            AdaptUI();
        }

        public void AdaptUI()
        {
            var scale = CalculateScaleInfo();
            transform.localScale = scale;
            contentDesignSize = Vector2.zero;
        }
    }

    public class SeasonQuestSpeedUpPopupViewController : ViewController<SeasonQuestSpeedUpPopup>
    {
        private CurrencyEmeraldView _currencyEmeraldView;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.button.onClick.AddListener(OnBuyBoosterClicked);
        }

        public override  async Task LoadExtraAsyncAssets()
        {
            _currencyEmeraldView = await view.AddCurrencyEmeraldView();
            _currencyEmeraldView.transform.SetParent(view.transform.parent,false);
            await base.LoadExtraAsyncAssets();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            string source = (string)extraData;
            
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonSpeedupCheck, ("source",source));
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            var emeraldCost = Client.Get<SeasonQuestController>().GetDiamondCostBuySpeedUpBuff();
            
            view.countText.text = emeraldCost.GetCommaFormat();
            
            view.descriptionText2.text = $"in {Client.Get<SeasonQuestController>().GetSpeedUpBuffDuration()} minutes";

        }

        private bool isWaitTip = false;
        protected async void OnBuyBoosterClicked()
        {
         
            var emeraldCost = Client.Get<SeasonQuestController>().GetDiamondCostBuySpeedUpBuff();
           
            if(Client.Get<UserController>().GetDiamondCount() < emeraldCost) 
            {
                if (isWaitTip)
                {
                    return;
                }
                
                XUtility.ShowTipAndAutoHide(view.bubbleGroup,2,0.2f,true,this);
                isWaitTip = true;
                await WaitForSeconds(0.5f);
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond,"DailyMission")));
                isWaitTip = false;
                return;
            }

            view.button.interactable = false;
            view.closeButton.interactable = false;
            string source = (string)extraData;
            
            Client.Get<SeasonQuestController>().BuySpeedUpBuff((success) =>
            {
                if (success)
                {
                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonSpeedupBuy, ("source",source));
                    view.Close();
                }
                else
                {
                    view.button.interactable = true;
                    view.closeButton.interactable = true;
                }
            });
        }
    }
}