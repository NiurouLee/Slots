using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using System;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class AmazingHatController : LogicController
    {
        private AmazingHatModel _model;

        private UserController _userController;
        private AlbumController _albumController;
        private float _gameCountDown;
        private float _lastUpdateGameCountDown;

        public AmazingHatController(Client client) : base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new AmazingHatModel();
            _userController = Client.Get<UserController>();
            _albumController = Client.Get<AlbumController>();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumInfoDataUpdate>(OnEventAlbumInfoUpdated);
            SubscribeEvent<EventLevelChanged>(OnLevelChangeExp);
            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget, HandlerPrioritySystemCollectWidget.AmazingHatWidget);
        }

        private async void OnLevelChangeExp(EventLevelChanged evt)
        {
            if (_userController.GetUserLevel() == GetUnlockLevel())
            {
                var hatWidget = await View.CreateView<AmazingHatWidget>();
                EventBus.Dispatch(new EventSystemWidgetNeedAttach(hatWidget,0));
                hatWidget.viewController.InitViewUI();
            }
        }

        private async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt, IEventHandlerScheduler eventHandlerScheduler)
        {
            if (!IsUnlock())
            {
                handleEndAction.Invoke();
                return;
            }
            var hatWidget = await View.CreateView<AmazingHatWidget>();
            evt.viewController.AddSystemWidget(hatWidget);
            hatWidget.viewController.InitViewUI();
            handleEndAction.Invoke();
        }
        
        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            EnableUpdate(1);
            finishCallback?.Invoke();
        }
        
        public override void Update()
        {
            if (_gameCountDown != 0 && IsUnlock())
            {
                float countDown = GetCountDown();
                if (countDown == 0)
                {
                    UpdateAmazingHatGameCountDown(0);
                }
            }
        }

        protected void OnEventAlbumInfoUpdated(EventAlbumInfoDataUpdate evt)
        {
            UpdateAmazingHatGameCountDown(_albumController.GetAmazingHatCountDown());
        }

        public async Task<SHatGameInfo> GetHatGameData()
        {
            SHatGameInfo sHatGameInfo = await _model.GetModalDataFromServer();
            UpdateAmazingHatGameCountDown(sHatGameInfo.HatGameInfo.HatGameCountDown);
            return sHatGameInfo;
        }

        public async Task<SHatGameSelect> HatGameSelect(uint selectIndex)
        {
            SHatGameSelect sHatGameSelect = await _model.SendSelectHat(selectIndex);
            if (sHatGameSelect == null)
                return null;
            UpdateAmazingHatGameCountDown(sHatGameSelect.HatGameInfo.HatGameCountDown);
            return sHatGameSelect;
        }

        public async Task<SHatGameCollectRewards> HatGameCollectHat(Action<bool, SHatGameCollectRewards> callback)
        {
            CHatGameCollectRewards cHatGameCollectRewards = new CHatGameCollectRewards();
            var sHatGameCollectRewards =
                await APIManagerGameModule.Instance.SendAsync<CHatGameCollectRewards, SHatGameCollectRewards>(
                    cHatGameCollectRewards);
            if (sHatGameCollectRewards.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sHatGameCollectRewards.Response.UserProfile));
                callback?.Invoke(true, sHatGameCollectRewards.Response);
                // EventBus.Dispatch(new EventAmazingHatStateUpdate());
                UpdateAmazingHatGameCountDown(sHatGameCollectRewards.Response.HatGameInfo.HatGameCountDown);
                return sHatGameCollectRewards.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sHatGameCollectRewards.ErrorInfo));
                // XDebug.LogWarning("SendCollectHat Response Error:" + sHatGameCollectRewards.ErrorInfo);
            }

            callback?.Invoke(false, null);
            return null;
        }

        /// <summary>
        /// 发送复活协议
        /// </summary>
        /// <returns></returns>
        public async Task<SHatGameHandleRabbit> HatGameRevive(Action<bool> callback, bool hasWatchedAdv, bool revive)
        {
            SHatGameHandleRabbit sHatGameHandleRabbit = await _model.SendHatGameRevive(callback, hasWatchedAdv, revive);
            if (sHatGameHandleRabbit != null)
            {
                UpdateAmazingHatGameCountDown(sHatGameHandleRabbit.HatGameInfo.HatGameCountDown);
            }
            return sHatGameHandleRabbit;
        }

        public void UpdateAmazingHatGameCountDown(uint countDown)
        {
            _gameCountDown = countDown;
            _lastUpdateGameCountDown = Time.realtimeSinceStartup;
            EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
        }

        public bool IsOpen()
        {
            return _gameCountDown == 0 && IsUnlock() && _albumController.IsOpen();
        }

        public bool IsUnlock()
        {
            return _userController.GetUserLevel() >= GetUnlockLevel();
        }
        
        public uint GetUnlockLevel()
        {
            return _albumController.GetUnlockLevel();
        }
        
        public float GetCountDown()
        {
            return _gameCountDown - (Time.realtimeSinceStartup - _lastUpdateGameCountDown);
        }

        public int Level => _model.Level;
        public HatGameInfo.Types.HatColor HatColor => _model.HatColor;
        public bool HasRabitTip => _model.HasRabitTip;
        public bool HasLuckyCardTip => _model.HasLuckyCardTip;
        public RepeatedField<Reward> Rewards => _model.Rewards;
        public int CostDiamond => _model.CostDiamond;
        public RepeatedField<uint> SelectedHatIndex => _model.SelectedHatIndex;
        public HatGameInfo.Types.HatGameStat HatGameStat => _model.HatGameStat;
    }
}