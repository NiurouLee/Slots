// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/20/17:07
// Ver : 1.0.0
// Description : SuperSpinXPosterPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine.UI;

namespace GameModule
{
    
    [AssetAddress("UISuperSpinxCardPoster")]
    public class SuperSpinXCardPosterPopup:Popup<SuperSpinXCardPosterPopupViewController>
    {
        [ComponentBinder("__NormalButton")] 
        
        public Button letGoButton;

        public SuperSpinXCardPosterPopup(string address) : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class SuperSpinXCardPosterPopupViewController : ViewController<SuperSpinXCardPosterPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            view.letGoButton.onClick.AddListener(OnLetGoClicked);
            SubscribeEvent<EventActivityExpire>(OnActivityExpired);
        }

        protected void OnLetGoClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "SuperSpinXCardActivity")));
            view.Close();
        }

        protected void OnActivityExpired(EventActivityExpire evt)
        {
            view.Close();
            
        }
    }
    
    [AssetAddress("UISuperSpinxWildPoster")]
    public class SuperSpinXAWildPosterPopup:Popup<SuperSpinXActivityPosterPopupViewController>
    {
        [ComponentBinder("__NormalButton")] 
        
        public Button letGoButton;

        public SuperSpinXAWildPosterPopup(string address) : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class SuperSpinXActivityPosterPopupViewController : ViewController<SuperSpinXAWildPosterPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            view.letGoButton.onClick.AddListener(OnLetGoClicked);
            SubscribeEvent<EventActivityExpire>(OnActivityExpired);
        }

        protected void OnLetGoClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "SuperSpinXWildActivity")));
            view.Close();
        }

        protected void OnActivityExpired(EventActivityExpire evt)
        {
            view.Close();
        }
    }
}