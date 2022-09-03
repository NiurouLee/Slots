using System.Collections.Generic;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule.UI;

namespace GameModule
{
    public class PopupController : LogicController
    {
      // protected Queue<PopupArgs> delayPopupQueue;

       protected Dictionary<BlockLevel, Queue<PopupArgs>> delayQueueDict;

       protected Dictionary<string, List<string>> eventToPoolsMap;
       protected Dictionary<string, PopupPool> popupPoolMap;

        public PopupController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
          //  delayPopupQueue = new Queue<PopupArgs>();
            eventToPoolsMap = new Dictionary<string, List<string>>();
            popupPoolMap = new Dictionary<string, PopupPool>();

            delayQueueDict = new Dictionary<BlockLevel, Queue<PopupArgs>>();
            
            base.Initialization();
        }
        protected override void SubscribeEvents()
        { 
           SubscribeEvent<EventShowPopup>(OnShowPopup);
           SubscribeEvent<EventEnqueueFencePopup>(OnEnqueueFencePopup);
           SubscribeEvent<EventEnqueuePopup>(OnEnqueuePopup);
           SubscribeEvent<EventLastPopupClose>(OnLastPopupClose);
           SubscribeEvent<EventPopupClose>(OnPopupClose);
           
           SubscribeEvent<EventSceneSwitchEnd>(OnEnterLobbyScene, HandlerPriorityWhenEnterLobby.PopupLogic);
           SubscribeEvent<EventSceneSwitchBegin>(OnEventSwitchSceneBegin);
           SubscribeEvent<EventTriggerPopupPool>(OnTriggerEvent);
        }

        protected void OnEventSwitchSceneBegin(EventSceneSwitchBegin evt)
        {
            foreach (var queue in delayQueueDict)
            {
                queue.Value.Clear();
            }
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            var popUpConfig = sGetInfoBeforeEnterLobby.PopUpConfig;
            if (popUpConfig != null)
            {
                if (popUpConfig.Config.Count > 0)
                {
                    for (var i = 0; i < popUpConfig.Pool.Count; i++)
                    {
                        var popupPool = new PopupPool(popUpConfig.Pool[i]);
                        popupPoolMap.Add(popupPool.GetPoolId(), popupPool);
                    }
                    
                    for (var i = 0; i < popUpConfig.Config.Count; i++)
                    {
                        var triggerName = popUpConfig.Config[i].Id;
                        
                        //服务器下发的是字符串，这里需要拆分成数组
                        var poolIds = popUpConfig.Config[i].PoolId;
                        
                        var poolIdArray = poolIds.Split(',');

                        if (poolIdArray.Length > 0)
                        {
                            var validPoolId = new List<string>();
                            for (var c = 0; c < poolIdArray.Length; c++)
                            {
                                if (popupPoolMap.ContainsKey(poolIdArray[c]))
                                {
                                    validPoolId.Add(poolIdArray[c]);
                                }
                            }

                            if (validPoolId.Count > 0)
                            {
                                eventToPoolsMap.Add(triggerName, validPoolId);
                            }
                        }
                    }
                    
                    PopupFilter.InitFilterConfig(popUpConfig.Filter);
                }
            }
        }
        
        public void OnTriggerEvent(EventTriggerPopupPool eventTriggerPopupPool)
        {
            if (eventToPoolsMap != null && eventToPoolsMap.ContainsKey(eventTriggerPopupPool.triggerType))
            {
                var poolList = eventToPoolsMap[eventTriggerPopupPool.triggerType];

                if (poolList != null && poolList.Count > 0)
                {
                    for (var i = 0; i < poolList.Count; i++)
                    {
                        var popupArgs = new PopupArgs(true, poolList[i]);
                        popupArgs.source = eventTriggerPopupPool.triggerType;
                        
                        EnqueuePopup(popupArgs);
                    }
                }
            }
            OnEnqueueFencePopup(eventTriggerPopupPool.handleEndCallback);
        }

        public void OnEnterLobbyScene(Action handleEndAction, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING && eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
                var triggerEvent = new EventTriggerPopupPool();

                triggerEvent.handleEndCallback = handleEndAction;
                
                triggerEvent.triggerType = "EnterLobby";
                OnTriggerEvent(triggerEvent);
            }
            else if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_MACHINE && eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
                var triggerEvent = new EventTriggerPopupPool();
                triggerEvent.triggerType = "BackToLobby";

                triggerEvent.handleEndCallback = handleEndAction;
                
                OnTriggerEvent(triggerEvent);
            }
            else
            {
                handleEndAction.Invoke();
            }
        }

        public void OnShowPopup(EventShowPopup eventShowPopup)
        {
            ShowPopup(eventShowPopup.popupArgs);
        }

        public void OnLastPopupClose(EventLastPopupClose eventPopupClose)
        {
            DequeuePopup();
        }
        
        public void OnPopupClose(EventPopupClose eventPopupClose)
        {
            DequeuePopup();
        }


        public void OnEnqueueFencePopup(EventEnqueueFencePopup eventEnqueueFencePopup)
        {
            OnEnqueueFencePopup(eventEnqueueFencePopup.fenceAction, eventEnqueueFencePopup.blockLevel);
        }

        private void OnEnqueueFencePopup(Action fenceAction, BlockLevel blockLevel = BlockLevel.DefaultLevel)
        {
            var popupArgs = new PopupArgs(true, fenceAction);
            
            popupArgs.blockLevel = blockLevel;
         
            EnqueuePopup(popupArgs);
            
            DequeuePopup();
        }

        public void OnEnqueuePopup(EventEnqueuePopup eventEnqueuePopup)
        {
            EnqueuePopup(eventEnqueuePopup.popupArgs);
        }

        //popup 放如队列
        public void EnqueuePopup(PopupArgs popupArgs)
        {
            BlockLevel blockLevel;

            if (popupArgs.isFence)
            {
                blockLevel = popupArgs.blockLevel;
            }
            else if (popupArgs.popupType != null)
            {
                blockLevel = popupArgs.blockLevel;
            }
            else
            {
                blockLevel = BlockLevel.DefaultLevel;
            }

            Queue<PopupArgs> delayPopupQueue;
        
            if (delayQueueDict.ContainsKey(blockLevel))
            {
                delayPopupQueue = delayQueueDict[blockLevel];
            }
            else
            {
                delayPopupQueue = new Queue<PopupArgs>();
                delayQueueDict[blockLevel] = delayPopupQueue;
            }
 
            if (popupArgs.isPopupPool || (!popupArgs.isFence && Popup.IsAllowMultiInstance(popupArgs.popupType)))
                delayPopupQueue.Enqueue(popupArgs);
            else
            {
                if (PopupStack.HasPopup(popupArgs.popupType))
                {
                    return;
                }

                if (delayPopupQueue.Count > 0)
                {
                    var popupInQueue = delayPopupQueue.ToArray();

                    for (var i = 0; i < popupInQueue.Length; i++)
                    {
                        if (!popupInQueue[i].isFence && !popupInQueue[i].isPopupPool && popupInQueue[i].popupType == popupArgs.popupType)
                        {
                            return;
                        }
                    }
                }
                delayPopupQueue.Enqueue(popupArgs);
            }
        }

        public void DequeuePopup()
        {
            foreach (BlockLevel blockLevel in Enum.GetValues(typeof(BlockLevel)))
            {
                XDebug.Log($"Search Block Level:{blockLevel}");

                if (delayQueueDict.ContainsKey(blockLevel))
                {
                    var delayPopupQueue = delayQueueDict[blockLevel];

                    if (DequeuePopup(delayPopupQueue, blockLevel))
                    {
                        break;
                    }
                }
            }
        }

        public bool DequeuePopup(Queue<PopupArgs> delayPopupQueue, BlockLevel blockLevel)
        {
            if (delayPopupQueue.Count <= 0)
                return false;

            var peekPopUpArgs = delayPopupQueue.Peek();

            if (peekPopUpArgs != null && peekPopUpArgs.isPopupPool)
            {
                var popupPool = popupPoolMap[peekPopUpArgs.poolId];
                peekPopUpArgs.popupType = popupPool.GetPopup(peekPopUpArgs);
            }

            if (delayPopupQueue.Count > 0 && PopupPopExecutor.CanPopNext(blockLevel))
            {
                PopupArgs popupArgs = delayPopupQueue.Dequeue();
                //弹板池
                if (popupArgs.isPopupPool)
                {
                    bool needDequeue = true;

                    if (popupPoolMap.ContainsKey(popupArgs.poolId))
                    {
                        var popupPool = popupPoolMap[popupArgs.poolId];
                        var popupType = popupPool.GetPopup(popupArgs);

                        if (popupType != null)
                        {
                            popupArgs = new PopupArgs(popupType, popupArgs.source);
                            needDequeue = false;
                        }
                    }

                    if (needDequeue)
                    {
                        return DequeuePopup(delayPopupQueue, blockLevel);
                    }
                }
                //fence 弹板
                else if (popupArgs.isFence)
                {
                    popupArgs.popupCloseAction?.Invoke();

                    if (delayPopupQueue.Count > 0)
                    {
                       return DequeuePopup(delayPopupQueue, blockLevel);
                    }

                    return false;
                }

                popupArgs.blockLevel = blockLevel;
                //普通弹板
                if (!ShowPopup(popupArgs))
                {
                    if (delayPopupQueue.Count > 0)
                    {
                        DequeuePopup(delayPopupQueue, blockLevel);
                    }
                }
                return true;
            }

            XDebug.Log("Debug: No Popup in Qeuee");
            return false;
            
        }

        public bool ShowPopup(PopupArgs popupArgs)
        {
            PopupPopExecutor.ShowPopUp(popupArgs);
            return true;
        }
    }
}