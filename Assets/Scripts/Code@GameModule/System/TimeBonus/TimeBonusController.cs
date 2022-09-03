// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/12/11:06
// Ver : 1.0.0
// Description : TimeBonusController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using ILRuntime.Runtime;

namespace GameModule
{
    public class TimeBonusController:LogicController
    {
        private TimeBonusInfoModel _model;

        public float lastTimeShowValultRV;
        public float lastWatchedTime;
        public float timeCheckShowRv = -20;
        public TimeBonusController(Client client)
            : base(client)
        {
            
        }
        protected override void Initialization()
        {
            base.Initialization();
            _model = new TimeBonusInfoModel();
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetTimerBonus != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetTimerBonus.TimerBonusInfo);
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
            
            SetUpRefreshCallback();
        }

        public override void OnPushNotification(FortunexNotification data)
        {
            var pbData = data.Pb?.Data;
            if (pbData != null && pbData.Length > 0)
            {
                var notificationData = ProtocolUtils.GetAnyStruct<FortunexNotifyficationHourlyBonus>(data.Pb);
                if (notificationData != null && _model.IsInitialized())
                {
                    _model.GetModelData().HourlyTimerBonusInfo = notificationData.HourlyTimerBonusInfo;
                    notificationData.HourlyTimerBonusInfo = null;
                }
            }
        }

        public TimerBonusStage GetTimeBonusState()
        {
            return _model.GetTimerBonusStage();
        }

        public ulong GetCurrentCoin()
        {
            return _model.GetCurrentCoinBonus();
        }
        public float GetMultiplierResetCountDown()
        {
            return _model.GetMultiplierCountDown();
        }
        
        public int GetMaxSpinBuffLevel()
        {
            return _model.GetMaxSpinBuffLevel();
        }
        
        public ulong GetSpinBuffMultiplier()
        {
            return _model.GetSpinBuffMultiplier();
        }
        public ulong GetSpinBuffMultiplier(int spinBuffLevel)
        {
            return _model.GetSpinBuffMultiplier(spinBuffLevel);
        }
        
        public ulong GetMaxCollectableCoins()
        {
            return _model.GetMaxCollectableCoins();
        }
        
        public float GetCoinBonusProgress(ulong currentCoin)
        {
            return _model.GetCoinBonusProgress(currentCoin);
        }
        
     
        public float GetWheelBonusCountDown()
        {
            return _model.GetWheelBonusCountDown();
        }     
        
        public uint GetWheelBonusProgress()
        {
            return _model.GetWheelBonusProgress();
        }

        public bool IsLuckyWheelStage()
        {
            return _model.GetTimerBonusStage() == TimerBonusStage.LuckyWheelBonus;
        }

        public bool IsBonusReady()
        {
            return _model.GetWheelBonusCountDown() <= 0;
        }
        
        public bool IsBonusReady(TimerBonusStage stage)
        {
            if (stage == TimerBonusStage.SuperWheelBonus)
            {
                return _model.GetTimerBonusStage() == TimerBonusStage.SuperWheelBonus
                       && _model.GetWheelBonusCountDown() <= 0;
            } 
            
            if (stage == TimerBonusStage.LuckyWheelBonus)
            {
                return _model.GetWheelBonusCountDown() <= 0 &&
                       _model.GetTimerBonusStage() == TimerBonusStage.LuckyWheelBonus;
            }
            
            return false;
        }

        public async void SetUpRefreshCallback()
        {
            if (_model.GetMultiplierCountDown() > 0)
            {
                var countDownTime = _model.GetMultiplierCountDown();
                countDownTime += UnityEngine.Random.Range(1, 2);

                WaitForSeconds(countDownTime, async () =>
                {
                    await _model.FetchModalDataFromServerAsync();
                    await Client.Get<BuffController>().SyncBufferData();
                });
            }
            else
            {
                await _model.FetchModalDataFromServerAsync();
                await  Client.Get<BuffController>().SyncBufferData();
                var countDownTime = _model.GetMultiplierCountDown();
                if (countDownTime > 0)
                {
                    SetUpRefreshCallback();
                }
            }
        }
         
        public async void RequestSlowDownCd(Action notifyCallback)
        {
            if (GetWheelBonusCountDown() > 1)
            {
                CNotifyAdWatchedForWheel cNotifyAdWatchedForWheel = new CNotifyAdWatchedForWheel();

                var sNotifyAdWatchedForWheel =
                    await APIManagerGameModule.Instance.SendAsync<CNotifyAdWatchedForWheel, SNotifyAdWatchedForWheel>(
                        cNotifyAdWatchedForWheel);
                if (sNotifyAdWatchedForWheel.ErrorCode == ErrorCode.Success)
                {
                    _model.UpdateModelData(sNotifyAdWatchedForWheel.Response.TimerBonusInfo);

                    EventBus.Dispatch(new EventTimeBonusStateChanged());
                }
            }

            notifyCallback.Invoke();
        }
         
        public async void ClaimTimeBonus(Action<Item> claimCallback, bool watchRv)
        {
            CGetHourlyBonus cGetHourlyBonus = new CGetHourlyBonus();
            cGetHourlyBonus.IsAdWatched = watchRv;
            
            var sGetHourlyBonus =
                await APIManagerGameModule.Instance.SendAsync<CGetHourlyBonus, SGetHourlyBonus>(cGetHourlyBonus);
            if (sGetHourlyBonus.ErrorCode == ErrorCode.Success)
            {
                _model.UpdateModelData(sGetHourlyBonus.Response.TimerBonusInfo);

                EventBus.Dispatch(new EventUserProfileUpdate(sGetHourlyBonus.Response.UserProfile));

                claimCallback.Invoke(sGetHourlyBonus.Response.Item);
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetHourlyBonus.ErrorInfo));
                claimCallback.Invoke(null);
            }
        }

        public async Task<SGetWheelBonus> GetWheelInfo()
        {
            CGetWheelBonus cGetWheelBonus = new CGetWheelBonus();
            var sGetWheelBonus =
                await APIManagerGameModule.Instance.SendAsync<CGetWheelBonus, SGetWheelBonus>(cGetWheelBonus);

            if (sGetWheelBonus.ErrorCode == ErrorCode.Success)
            {
                XDebug.Log(sGetWheelBonus.Response.BonusStage);
                return sGetWheelBonus.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetWheelBonus.ErrorInfo));
            }

            return null;
        }
        
        public async Task<SGetGoldenWheelBonus> GetGoldenWheelInfo()
        {
            CGetGoldenWheelBonus cGetWheelBonus = new CGetGoldenWheelBonus();
            var sGetGoldenWheelBonus =
                await APIManagerGameModule.Instance.SendAsync<CGetGoldenWheelBonus, SGetGoldenWheelBonus>(cGetWheelBonus);

            if (sGetGoldenWheelBonus.ErrorCode == ErrorCode.Success)
            {
                return sGetGoldenWheelBonus.Response;
            }
            else
            {
                  EventBus.Dispatch(new EventOnUnExceptedServerError(sGetGoldenWheelBonus.ErrorInfo));
            }

            return null;
        }
        
        public async Task<SSpinWheel> GetWheelSpinResult(TimerBonusWheelId wheelId)
        {
            CSpinWheel cSpinWheel = new CSpinWheel();

            cSpinWheel.WheelId = wheelId;
          
            var sSpinWheel =
                await APIManagerGameModule.Instance.SendAsync<CSpinWheel, SSpinWheel>(cSpinWheel);

            if (sSpinWheel.ErrorCode == ErrorCode.Success)
            {
                return sSpinWheel.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sSpinWheel.ErrorInfo));
            }

            return null;
        }

        public ulong GetLuckyWheelWinUpTo()
        {
            return _model.GetModelData().WheelBonus.MaxCoinNormalWheel;
        }
        
        public ulong GetMultWheelWinUpTo()
        {
            return _model.GetModelData().WheelBonus.MaxCoinSuperWheel;
        }
 
        public async Task<SSpinWheelBonusWithNoRecord> GetAdBonus()
        {
            CSpinWheelBonusWithNoRecord cSpinWheel = new CSpinWheelBonusWithNoRecord();
            cSpinWheel.PlaceId = (int)eAdReward.WheelBonusRV;
            
            var sAdWheelBonus =
                await APIManagerGameModule.Instance.SendAsync<CSpinWheelBonusWithNoRecord, SSpinWheelBonusWithNoRecord>(cSpinWheel);

            if (sAdWheelBonus.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sAdWheelBonus.Response.UserProfile));
                return sAdWheelBonus.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sAdWheelBonus.ErrorInfo));
            }

            return null;
        }

        public async void ClaimWheelBonus(Action<RepeatedField<Item>> claimCallback)
        {
            CCollectWheelBonus cCollectWheelBonus = new CCollectWheelBonus();
            var sCollectWheelBonus =
                await APIManagerGameModule.Instance.SendAsync<CCollectWheelBonus, SCollectWheelBonus>(
                    cCollectWheelBonus);

            if (sCollectWheelBonus.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(sCollectWheelBonus.Response.UserProfile));

                claimCallback.Invoke(sCollectWheelBonus.Response.Item);

                _model.UpdateModelData(sCollectWheelBonus.Response.TimerBonusInfo);

                EventBus.Dispatch(new EventTimeBonusStateChanged());
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sCollectWheelBonus.ErrorInfo));
                claimCallback.Invoke(null);
            }
        }
        
        public override bool IsDataReady()
        {
            return _model != null && _model.IsInitialized();
        }
    }
}