// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/16:30
// Ver : 1.0.0
// Description : NewBieQuestController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class NewBieQuestController : LogicController
    {
        private NewBieQuestModel _model;
        
        public bool IsNewUnlocked { get; set; }
        public bool NeedShowUnlockTip { get; set; }
        public NewBieQuestController(Client client) :
            base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new NewBieQuestModel();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget,
                HandlerPrioritySystemCollectWidget.NewBieQuestWidget);

            SubscribeEvent<EventQuestUnlock>(OnQuestUnlock);
        }

        protected async void OnQuestUnlock(EventQuestUnlock evt)
        {
            var userLevel = Client.Get<UserController>().GetUserLevel();
            
            if(userLevel >= _model.GetUnlockLevel() && IsLocked())
            { 
                await _model.FetchModalDataFromServerAsync();

                if (!IsLocked() && !IsTimeOut())
                {
                    IsNewUnlocked = true;
                    NeedShowUnlockTip = true;
                }

                 PopupStack.ShowPopupNoWait<QuestUnlockPopup>();

                if (!IsTimeOut())
                {
                    var questWidget = await View.CreateView<QuestWidget>();
                    EventBus.Dispatch(new EventSystemWidgetNeedAttach(questWidget, 0));
                }
            }
        }


        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            var newBieQuest = GetSystemData<NewbieQuest>(evt.systemContent, "NewbieQuest");
            if (newBieQuest != null)
            {
                UpdateQuestContent(newBieQuest);
                EventBus.Dispatch(new EventQuestDataUpdated());
            }
        }

        protected void UpdateQuestContent(NewbieQuest newbieQuest)
        {
            _model.UpdateModelData(newbieQuest);
        }

        protected async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt,
            IEventHandlerScheduler eventHandlerScheduler)
        {
            if (IsLocked() || IsQuestFinished() || IsTimeOut())
            {
                handleEndAction.Invoke();
                return;
            }
            
            
            var questWidget = await View.CreateView<QuestWidget>();
            evt.viewController.AddSystemWidget(questWidget);
            handleEndAction.Invoke();
        }


        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetNewbieQuest != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetNewbieQuest.NewbieQuest);
                beforeEnterLobbyServerDataReceived = true;
            } 
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await _model.FetchModalDataFromServerAsync();
            }

            finishCallback?.Invoke();

            if (!_model.IsLocked())
            {
                UnsubscribeEvent<EventQuestUnlock>(OnQuestUnlock);
            }
        }

        public ulong GetTotalPrize()
        {
            return _model.GetTotalPrize();
        }

        public float GetQuestCountDown()
        {
            return _model.GetQuestCountDown();
        }

        public uint GetUnlockLevel()
        {
            return _model.GetUnlockLevel();
        }

        public bool IsLocked()
        {
            return _model.IsLocked();
        }

        public Quest GetCurrentQuest()
        {
            return _model.GetCurrentQuest();
        }
        
        public Quest GetQuest(int index)
        {
            return _model.GetQuestNode(index);
        }
 
        public bool IsInQuestMachineScene()
        {
            var currentQuest = GetCurrentQuest();

            if (currentQuest == null)
                return false;

            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

            if (machineScene != null)
            {
                return machineScene.viewController.GetMachineContext().assetProvider.MachineId == currentQuest.GameId;
            }
            
            return false;
        }

        public ulong GetFinalPrize()
        {
            var quest = _model.GetQuestNode(_model.GetTotalQuestCount() - 1);
            var coinItem = XItemUtility.GetItem(quest.Reward.Items, Item.Types.Type.Coin);
            if (coinItem != null)
            {
                return coinItem.Coin.Amount;
            }

            return 0;
        }

        public int GetQuestTotalCount()
        {
            return _model.GetTotalQuestCount();
        }

        public uint GetCurrentQuestIndex()
        {
            return _model.GetCurrentQuestIndex();
        }

        public async void ClaimCurrentQuest(Action<bool> callback)
        {
            var send = new CCollectNewbieQuest();
            send.Index = _model.GetCurrentQuestIndex();
            var currentQuest = _model.GetCurrentQuest();

            var receive = await APIManagerGameModule.Instance.SendAsync<CCollectNewbieQuest, SCollectNewbieQuest>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(receive.Response.UserProfile));
       
                _model.UpdateModelData(receive.Response.NewbieQuest);
                callback.Invoke(true);
                return;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(receive.ErrorInfo));
            }

            callback.Invoke(false);
        }

        public bool IsQuestFinished()
        {
            if (_model == null)
                return true;
            var totalQuestCount = _model.GetTotalQuestCount();
            if (totalQuestCount <= 0)
                return true;

            var quest = _model.GetQuestNode(_model.GetTotalQuestCount() - 1);
           
            if (quest.Collected)
            {
                return true;
            }
            
            return false;
        }

        public bool IsTimeOut()
        {
             return GetQuestCountDown() <= 0;
        }

        public List<MissionController> GetCurrentMission()
        {
            return _model.GetCurrentMission();
        }

        public async Task<SGetNewbieQuestPaymentItems> GetNewBieQuestPayItem()
        {
            var send = new CGetNewbieQuestPaymentItems();
            var receive = await APIManagerGameModule.Instance
                .SendAsync<CGetNewbieQuestPaymentItems, SGetNewbieQuestPaymentItems>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                return receive.Response;
            }
            else
            {
                return null;
            }
        }
    }
}