// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/18/11:23
// Ver : 1.0.0
// Description : GuideEnterGamePopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIGuideEnterMachine")]
    public class GuideEnterGamePopup:Popup<GuideEnterGameViewController>
    {
        [ComponentBinder("UIGuideHand")] 
        public Transform uiGuideHand;

        public override void OnAlphaMaskClicked()
        {
            viewController.OnAlphaMaskClicked();
        }
        
        public GuideEnterGamePopup(string address)
            : base(address)
        {
            
        }
        
        public override float GetPopUpMaskAlpha()
        {
            return 0;
        }
    }

    public class GuideEnterGameViewController : ViewController<GuideEnterGamePopup>
    {
        protected string GuideMachine = string.Empty;
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            view.uiGuideHand.gameObject.SetActive(false);
            GuideMachine = Client.Get<GuideController>().GetEnterMachineId();

            WaitForSeconds(1, () =>
            {
                EventBus.Dispatch(new EventHighLightMachineEntrance(GuideMachine));
            });
          
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuidePopFirstMachine);
        }

        public void OnAlphaMaskClicked()
        {
            Client.Get<MachineLogicController>().EnterGame(GuideMachine,"", "Guide");
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventBeforeEnterScene>(OnEventBeforeEnterScene);
            SubscribeEvent<EventOnMachineEntranceHighlight>(OnMachineEntranceHighLight);
        }

        protected void OnMachineEntranceHighLight(EventOnMachineEntranceHighlight evt)
        {
            var handPosition = evt.containerTransform.position;
           
            view.uiGuideHand.position = handPosition;
            
            WaitForSeconds(0.1f, () =>
            {
                view.uiGuideHand.gameObject.SetActive(true);
                var canvas = view.uiGuideHand.GetComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                canvas.sortingOrder = 3;
            });
        }
        
        protected void OnEventBeforeEnterScene(EventBeforeEnterScene evt)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuideTapFirstMachine);
            view.Close();
        }
    }
}