using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public interface IActivity
    {
        void OnCreate(SGetActivitiesOpenTime.Types.ActivityTimeConfig activity, ActivityController controller);

        /// <summary>
        /// 用于活动开启时进入大厅打开宣传页面
        /// </summary>
        void OnEnterLobby();
        bool IsValid();
        void OnRefreshUserData(SGetActivityUserData.Types.ActivityData activityData);

        void OnBannerJump(JumpInfo jumpInfo);

        bool OnCheckBannerValid(JumpInfo jumpInfo);
    }
    
    public abstract class ActivityBase : Controller, IActivity
    {
        private ulong _gameCountDown;
        private float _lastUpdateGameCountDown;

        protected bool inRequest;

        protected string ActivityID { get; private set; }

        protected string ServerActivityType { get; private set; }

        private ActivityController _controller;

        private CancelableCallback leftTimeCallback;

        protected SGetActivityUserData.Types.ActivityData activityData;
        
        protected Action expiredAction;

        private string activityType;

        protected bool activityPosterPopped = false;

        protected SGetActivitiesOpenTime.Types.ActivityTimeConfig activityTimeConfig;

        public ActivityBase(string inActivityType)
        {
            activityType = inActivityType;
        }

        public virtual void OnCreate(SGetActivitiesOpenTime.Types.ActivityTimeConfig inActivityTimeConfig, ActivityController controller)
        {
            _controller = controller;
            activityTimeConfig = inActivityTimeConfig;
            ActivityID = activityTimeConfig.ActivityId;
            ServerActivityType = activityTimeConfig.ActivityType;
            SubscribeEvents();
            
            EventBus.Dispatch(new EventActivityCreate(activityType, ActivityID));
        }

        /// <summary>
        /// 使用CountDown倒计时的活动按各活动需求重写此方法
        /// </summary>
        protected virtual void SetExpireTimeCallback()
        {
            var leftTime = GetCountDown();
            leftTimeCallback?.CancelCallback();
            leftTimeCallback = WaitForSecondsRealTime(leftTime, async () =>
            {
                OnExpire();
            });
        }

        public void SetActivityState(bool isRequest)
        {
            inRequest = isRequest;
            if (!isRequest && IsExpired())
            {
                expiredAction?.Invoke();
                expiredAction = null;
            }
        }

        public virtual void OnEnterLobby()
        {
            activityPosterPopped = true;
        }

        /// <summary>
        /// 获取是否过期
        /// </summary>
        protected abstract bool IsExpired();
        
        /// <summary>
        /// 获取是否解锁状态
        /// </summary>
        public abstract bool IsUnlockState();

        public bool IsValid()
        {
            return IsUnlockState() && !IsExpired();
        }

        public virtual void OnBannerJump(JumpInfo jumpInfo)
        {
            
        }

        public virtual bool OnCheckBannerValid(JumpInfo jumpInfo)
        {
            return IsValid();
        }

        public virtual void OnUpdateBannerContent(Advertisement advertisement, View parentView, Transform bannerContent) { }
        
        /// <summary>
        /// 客户端结束时间统一比服务器早结束1分钟（多长时间待定）
        /// </summary>
        /// <param name="endCountDown"></param>
        protected void OnUpdateCountDown(ulong endCountDown)
        {
            _gameCountDown = endCountDown;
            _lastUpdateGameCountDown = Time.realtimeSinceStartup;
            SetExpireTimeCallback();
        }

        public virtual float GetCountDown()
        {
            return _gameCountDown - (Time.realtimeSinceStartup - _lastUpdateGameCountDown) - 60;
        }
        
        protected virtual void OnExpire()
        {
            _controller.RemoveActivity(ServerActivityType, ActivityID);
            
            EventBus.Dispatch(new EventActivityExpire(activityType, ActivityID));
            
            CleanUp();
        }

        public virtual void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            
        }
        protected T GetSystemData<T>(RepeatedField<AnyStruct> systemContent, string filter) where T : Google.ilruntime.Protobuf.IMessage
        {
            if (systemContent!=null && systemContent.Count>0)
            {
                for (int i = 0; i < systemContent.Count; i++)
                {
                    var extraData = systemContent[i];
                    if (extraData.Type == filter)
                    {
                        return ProtocolUtils.GetAnyStruct<T>(extraData);
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// 每个获取各自记录所需要的数据
        /// </summary>
        /// <param name="inActivityData"></param>
        public virtual void OnRefreshUserData(SGetActivityUserData.Types.ActivityData inActivityData)
        {
            activityData = inActivityData;
            EventBus.Dispatch(new EventActivityServerDataUpdated(activityType, ActivityID));
        }

        /// <summary>
        /// 用于获取该活动的UserData接口
        /// </summary>
        /// <returns></returns>
        public async Task<SGetActivityUserData> RequestCGetActivityUserDataAsync()
        {
            var ids = new List<string>();
            ids.Add(ActivityID);
            return await _controller.RequestCGetActivityUserDataAsync(ids);
        }

        public virtual void OnActivityOpen()
        {
            EventBus.Dispatch(new EventActivityOpen(activityType, ActivityID));
        }

        public virtual Task OnEventLobbyCreated(EventOnLobbyCreated obj)
        {
            return Task.CompletedTask;
        }

        public virtual string GetAssetLabelName()
        {
            return null;
        }

        public virtual void UpdateAssetThemeName(AssetAddressAttribute addressAttribute)
        {
            
        }
    }
}