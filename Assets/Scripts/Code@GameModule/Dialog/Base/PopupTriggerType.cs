// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-02 11:04 AM
// Ver : 1.0.0
// Description : PopupTriggerType.cs
// ChangeLog :
// **********************************************


namespace GameModule
{
    public class PopupTriggerType
    {
        public const string TRIGGER_ENTER_LOBBY = "EnterLobby";
        public const string TRIGGER_FROM_SLOT_ENTER_LOBBY = "FromSlotEnterLobby";
        public const string TRIGGER_OUT_OF_CHIPS = "OutOfChips";
        public const string TRIGGER_DEAL_CLICK = "DealClick";
        public const string TRIGGER_CLOSE_MAIN_SHOP = "CloseMainShop";
        
        //TODO QUEST 用的，不应改定义到这里，这里定义的Trigger是PopupLogic触发的弹板的Trigger
        public const string TRIGGER_ENTRANCE_CLICK = "EntranceClick";
        public const string TRIGGER_TASK_FINISHED = "TaskFinished";
        public const string TRIGGER_TASK_CLAIMED = "TaskClaimed";
    }
}