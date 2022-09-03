namespace GameModule
{
    [AssetAddress("UICashBackTimeStoreIcon")]
    public class UIActivity_CashBack_StoreWidgetView : View<CashBackStoreWidgetViewContoller>
    {
        public UIActivity_CashBack_StoreWidgetView(string address) : base(address)
        {
            
        }
    }

    public class CashBackStoreWidgetViewContoller : ViewController<UIActivity_CashBack_StoreWidgetView>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventBus.Subscribe<EventCashBackUserDateChanged>(OnEventCashBackUserDateChanged);
        }
        
        private void OnEventCashBackUserDateChanged(EventCashBackUserDateChanged obj)
        {
            var activityController = Client.Get<ActivityController>();
            var activity = activityController.GetDefaultActivity(ActivityType.CashBack) as Activity_CashBack;
            if (activity == null || activity.isStoreCommodityValid == false)
            {
                view.GetParentView().RemoveChild(view);
            }
        }
    }
}