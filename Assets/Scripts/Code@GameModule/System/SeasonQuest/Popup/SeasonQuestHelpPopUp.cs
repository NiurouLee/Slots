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

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOneHelp")]
    public class SeasonQuestHelpPopUp:Popup<SeasonQuestHelpPopUpController>
    {
        public SeasonQuestHelpPopUp(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1200,768);
        }
        
    }

    public class SeasonQuestHelpPopUpController : ViewController<SeasonQuestHelpPopUp>
    {
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventSeasonQuestSeasonFinish>((evt) =>
            {
                view.Close();
            });
        }
    }
}