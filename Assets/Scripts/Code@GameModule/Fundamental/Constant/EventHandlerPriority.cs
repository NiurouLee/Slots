// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-01-18 6:23 PM
// Ver : 1.0.0
// Description : EventHandlerPriority.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public static class HandlerPriorityWhenEnterLobby
    {
        public const int Guide = 10000;
        public const int LobbyBanner = 10001;
        public const int SystemAssets = 20000;
        public const int CheckProcessingTransaction = 9000;
        public const int DeepLinkReward = 8000;
        public const int CheckProcessingCrazyCash = 5300;
        public const int PopupLogic = 5000;
        public const int ActivityLogic = 4900;
        public const int TravelAlbum = 5400;
        public const int DailyBonus = 5500;
        public const int FreeBonus = 6000;
        public const int CrazeSmash = 7000;
        public const int Ad = 11;
        public const int SeasonQuest = 1000;
        public const int LobbyStoreBonusGuide = 10;
        public const int SystemMessage = 110000;
    }

    public static class HandlerPriorityWhenSpinEnd
    {
        public const int AllEnd = 0;
        public const int ActivityTreasureRaid = 5;//这个活动可能会跳转到活动内，跳转进活动后面的回调不会执行了。
        public const int RateUs = 10;
        public const int Album = 11;
        public const int Banner = 100;
        public const int CashBack = 250;
        public const int Guide = 350;
        public const int Quest = 410;
        public const int LevelUp = 600;
        //DailyMission的优先级必须放在LevelRush后面，由于LevelRush的奖励是随升级下发的，如果表演放在DailyMission之后，如果DailyMission领取了卡牌奖励导致数据表现出问题
        public const int DailyMission = 630;
        public const int LevelRush = 650;
        public const int Ad = 10000;
        //金币检查，需要为最高优先级
        public const int BalanceCheck = 10000000;
    }
    
    public static class HandlerPriorityWhenSceneSwitchEnd
    {
        public const int QuestPopup = 100000;
        public const int SeasonQuestPopup = 100000;
    }

    public static class HandlerPrioritySystemCollectWidget
    {

        public const int NewBieQuestWidget = 200;
        public const int AdTaskWidget = 0;
        public const int SeasonQuestWidget = 100;
        public const int SeasonPassWidget = 300;
        public const int AmazingHatWidget = 500;
        public const int TreasureRaidWidget = 400;

    }
}