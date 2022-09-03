// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/22/21:18
// Ver : 1.0.0
// Description : CashBackStoreWidget.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    [AssetAddress("UICashBackStoreWidget")]
    public class CashBackStoreWidget:View<CashBackStoreWidgetViewController>
    {
        public CashBackStoreWidget(string address)
            : base(address)
        {
            
        }
    }
    
    public class CashBackStoreWidgetViewController:ViewController<CashBackStoreWidget>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        public void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType == ActivityType.CashBack)
            {
                var parentView = view.GetParentView() as PriceButtonExtraItemView;
                if(parentView != null) 
                {
                    parentView.RemoveExtraItemView(view);        
                }
            }
        }
    }
}