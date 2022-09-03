// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/10/16:42
// Ver : 1.0.0
// Description : QuestFaqPopUp.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    [AssetAddress("UIQuestHelp")]
    public class QuestFaqPopUp:Popup<QuestFaqPopUpController>
    {
        public QuestFaqPopUp(string address)
            : base(address)
        {
            
        }
    }

    public class QuestFaqPopUpController : ViewController<QuestFaqPopUp>
    {
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventQuestTimeOut>((evt) =>
            {
                view.Close();
            });
        }
    }
}