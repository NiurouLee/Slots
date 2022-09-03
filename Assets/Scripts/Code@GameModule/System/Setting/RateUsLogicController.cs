// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/27/22:09
// Ver : 1.0.0
// Description : RateUsLogicController.cs
// ChangeLog :
// **********************************************

using System;

namespace GameModule
{
    public class RateUsLogicController:LogicController
    {
        public RateUsLogicController(Client client)
            :base(client)
        {
            
        }

        protected override void SubscribeEvents()
        {
            if (Client.Storage.GetItem("RateUsChooseOpenUrl", "False") != "True")
                SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.RateUs);
        }

        protected  void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {

            if (Client.Storage.GetItem("RateUsChooseOpenUrl", "False") == "True")
            {
                handleEndCallback.Invoke();
                
                return;
            }

            if (Client.Get<UserController>().GetUserLevel() < 10)
            {
                handleEndCallback.Invoke();
                return;
            }
             
            if (eventSceneSwitchEnd.winLevel >= (uint) WinLevel.HugeWin)
            {
                var timeStamp = Convert.ToInt64(Client.Storage.GetItem("RateUsChooseClosedClicked",
                    "0"));
               
                var dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp);
                
                var offset = DateTimeOffset.UtcNow - dataOffset;

                if (offset.TotalHours > 72)
                {
                    PopupStack.ShowPopupNoWait<UIRateUsPopup>();
                }
            }
            handleEndCallback.Invoke();
            return;
        }
    }
}