using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameModule
{
    public class ActivityController : LogicController
    {
        public bool hasActivity { get { return _fxActivities != null && _fxActivities.Count > 0; } }
        public double serverTimeSeconds { get; private set; }
        public double lastUpdateTime { get; private set; }
        private readonly Dictionary<string, string> _idToType = new Dictionary<string, string>();
        private readonly Dictionary<string, Dictionary<string, ActivityBase>> _fxActivities
            = new Dictionary<string, Dictionary<string, ActivityBase>>();

        private Dictionary<string, AssetDependenciesDownloadOperation> downloadingOperations;
        private List<string> needDownloadLabel;

        private RepeatedField<SGetActivitiesOpenTime.Types.ActivityTimeConfig> _activityTimeConfigs;
        private uint activityTimeConfigServerTimeSeconds;
        private float updateActivityTimeConfigGameCountDown;
        private Dictionary<string, CancelableCallback> _idCancelCallbacks;

        public ActivityController(Client client) : base(client)
        {
            
        }

        public Dictionary<string, ActivityBase> GetActivities(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            Dictionary<string, ActivityBase> map;
            _fxActivities.TryGetValue(type, out map);
            return map;
        }

        public ActivityBase GetDefaultActivity(string type)                                               
        {
            var map = GetActivities(type);
            if (map == null || map.Count == 0)
                return null;

            return map[map.First().Key];
            // foreach (var item in map.Values)
            // {
            //     return item;
            // }
            // return null;
        }

        public ActivityBase GetActivity(string type, string id)
        {
            var map = GetActivities(type);
            if (map == null)
                return null;

            ActivityBase result;
            map.TryGetValue(id, out result);
            return result;
        }

        private void AddActivity(string type, string id, ActivityBase activity)
        {
            if (activity == null)
                return;

            Dictionary<string, ActivityBase> map;
            _fxActivities.TryGetValue(type, out map);
            if (map == null)
            {
                map = new Dictionary<string, ActivityBase>();
                _fxActivities.Add(type, map);
            }
            map[id] = activity;
        }

        public void RemoveActivity(string type, string id)
        {
            if (string.IsNullOrWhiteSpace(type))
                return;

            if (string.IsNullOrWhiteSpace(id))
                return;

            Dictionary<string, ActivityBase> map;
            _fxActivities.TryGetValue(type, out map);
            if (map == null)
                return;
            ActivityBase activity;
            map.TryGetValue(id, out activity);
            if (activity != null)
                map.Remove(id);

            if (map.Count == 0)
                _fxActivities.Remove(type);
        }

        private string GetActivityType(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            string result;
            _idToType.TryGetValue(id, out result);
            return result;
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.ActivityLogic);
            SubscribeEvent<EventOnLobbyCreated>(OnLobbyCreated,1);
        }
 
        private async void OnLobbyCreated(Action handleEndCallback, EventOnLobbyCreated evt,
            IEventHandlerScheduler scheduler)
        {
            if (_fxActivities.Count <= 0)
            {
                handleEndCallback.Invoke();
                return;
            }

            var taskList = new List<Task>();
            
            foreach (var map in _fxActivities.Values)
            {
                foreach (var activity in map.Values)
                {
                    var task = activity.OnEventLobbyCreated(evt);
                    taskList.Add(task);
                }
            }
            
            await Task.WhenAll(taskList);
            
            handleEndCallback.Invoke();
        }

        private bool isAdaptor;

        private void SetAdaptor(bool inIsAdaptor)
        {
            isAdaptor = inIsAdaptor;
        }
        public bool IsAdaptor()
        {
            return isAdaptor;
        }
        
        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            if (_fxActivities.Count <= 0)
                return;
            foreach (var map in _fxActivities.Values)
            {
                foreach (var activity in map.Values)
                {
                    activity.OnSpinSystemContentUpdate(evt);
                }
            }
        }

        private async void OnSceneSwitchEnd(
            Action handleEndCallback,
            EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY && eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING)
            {
                if (hasActivity)
                {
                    foreach (var map in _fxActivities.Values)
                    {
                        foreach (var activity in map.Values)
                        {
                            activity.OnEnterLobby();
                        }
                    }
                    var widgetOffset = Mathf.Max(Screen.safeArea.x, Screen.width - Screen.safeArea.width - Screen.safeArea.x);

                    if(widgetOffset > 101) {
                        widgetOffset = 101;
                    }

                    if (Screen.width / (float)Screen.height < 1.79) 
                    {
                        widgetOffset = 0;
                    }

                    widgetOffset = widgetOffset * ViewResolution.referenceResolutionLandscape.x / Screen.width;
                    SetAdaptor(widgetOffset != 0);
                }
            }

            handleEndCallback?.Invoke();
        }

        // public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        // {
        //     if (needDownloadLabel == null)
        //     {
        //         needDownloadLabel = new List<string>();
        //     }
        //     _idToType.Clear();
        //     _fxActivities.Clear();
        //     needDownloadLabel.Clear();
        //
        //     await GetActivityOpenTime();
        //     await CheckNewActivity();
        //     finishCallback?.Invoke();
        // }

        public async Task PrepareActivitiesServerData()
        {
            if (needDownloadLabel == null)
            {
                needDownloadLabel = new List<string>();
            }

            _idToType.Clear();
            _fxActivities.Clear();
            needDownloadLabel.Clear();

           // await GetActivityOpenTime();
            await CheckNewActivity();
        }

        private void SetLeftTimeFinishCallback()
        {
            if (_activityTimeConfigs == null || _activityTimeConfigs.Count <= 0) return;

            if (_idCancelCallbacks == null)
            {
                _idCancelCallbacks = new Dictionary<string, CancelableCallback>();
            }

            foreach (var timeConfig in _activityTimeConfigs)
            {
                if (CheckHasActivity(timeConfig.ActivityId)) continue;
                var leftTime = Mathf.Max(0,
                                   (long)timeConfig.StartTime - activityTimeConfigServerTimeSeconds) -
                               (Time.realtimeSinceStartup - updateActivityTimeConfigGameCountDown);
                
                XDebug.LogWarning("     leftTime:" + leftTime);
                if (leftTime > 0)
                {
                    if (_idCancelCallbacks.ContainsKey(timeConfig.ActivityId))
                    {
                        _idCancelCallbacks[timeConfig.ActivityId].CancelCallback();
                    }
                    var leftTimeCallback = WaitForSeconds(leftTime, async () =>
                    {
                        await CheckNewActivity();
                    });
                    _idCancelCallbacks[timeConfig.ActivityId] = leftTimeCallback;
                }
            }
        }

        private async Task GetActivityOpenTime()
        {
            CGetActivitiesOpenTime cGetActivitiesOpenTime = new CGetActivitiesOpenTime();

            var sGetActivitiesOpenTime =
                await APIManagerGameModule.Instance.SendAsync<CGetActivitiesOpenTime, SGetActivitiesOpenTime>(
                    cGetActivitiesOpenTime);

            if (sGetActivitiesOpenTime.ErrorCode == 0)
            {
                _activityTimeConfigs = sGetActivitiesOpenTime.Response.ActivitiesOpenTimes;
                activityTimeConfigServerTimeSeconds = sGetActivitiesOpenTime.Response.ServerTimeSeconds;
                updateActivityTimeConfigGameCountDown = Time.realtimeSinceStartup;
                SetLeftTimeFinishCallback();
            }
            else
                XDebug.LogError(sGetActivitiesOpenTime.ErrorInfo);
        }

        private async Task CheckNewActivity()
        {
            await GetActivityOpenTime();
            if (_activityTimeConfigs == null || _activityTimeConfigs.Count <= 0) 
                return;

            // var responseData = await RequestCGetActivitiesAsync();
            // if (responseData == null || responseData.Activities == null || responseData.Activities.Count == 0)
            // {
            //     return;
            // }
            // var count = responseData.Activities.Count;
            
            List<string> ids = new List<string>();
            for (int i = 0; i < _activityTimeConfigs.Count; i++)
            {
                var activityTimeConfig = _activityTimeConfigs[i];
                var activityID = activityTimeConfig.ActivityId;
                var serverActivityType = activityTimeConfig.ActivityType;
                // var open = true;//(activityTimeConfigServerTimeSeconds - activity.StartTime) <= 0;
                var leftTime = Mathf.Max(0,
                                   (long)activityTimeConfig.StartTime - activityTimeConfigServerTimeSeconds) -
                               (Time.realtimeSinceStartup - updateActivityTimeConfigGameCountDown);
                XDebug.LogWarning("Check leftTime: " + leftTime);
                if (!CheckHasActivity(activityID) && leftTime <= 0)
                {
                    var activityName =
                        GetTitleCase(serverActivityType.Replace("OPS_EVENT_TYPE_", "").Replace("_", " ").ToLower());
                    var activityType = $"Activity_{activityName}";
                    var classType = Type.GetType($"GameModule.{activityType}");
                    
                    if (classType != null)
                    {
                        var activityInstance = CreateActivity(activityTimeConfig, classType, activityID, serverActivityType);
                        
                        ids.Add(activityID);

                        var activityAssetLabelName = activityName;

                        if (activityInstance != null && activityInstance.GetAssetLabelName() != null)
                        {
                            activityAssetLabelName = activityInstance.GetAssetLabelName();
                        }
                        
                        var activityAssetLabel = $"SALabel_UI{activityAssetLabelName}";
                        
                        if (!needDownloadLabel.Contains(activityAssetLabel))
                        {
                            needDownloadLabel.Add(activityAssetLabel);
                        }
                    }
                }
            }

#if !UNITY_EDITOR
            BeginDownloadActivityAsset();
#endif

            if (ids.Count > 0)
            {
                await RequestCGetActivityUserDataAsync(ids, true);
            }
        }

        private string GetTitleCase(string name)
        {
            var result = "";
            var strings = name.Split(" ".ToCharArray());
            for (int i = 0; i < strings.Length; i++)
            {
                var arr = strings[i].ToCharArray();
                arr[0] = char.ToUpper(arr[0]);
                for (int j = 0; j < arr.Length; j++)
                {
                    result += string.Format($"{arr[j]}");
                }
            }

            return result;
        }

        private bool CheckHasActivity(string activityID)
        {
            return _idToType.ContainsKey(activityID);
        }

        private ActivityBase CreateActivity(SGetActivitiesOpenTime.Types.ActivityTimeConfig activityTimeConfig, Type classType, string activityID, string serverActivityType)
        {
            try
            {
                ActivityBase fxActivity = Activator.CreateInstance(classType) as ActivityBase;
                _idToType[activityID] = serverActivityType;
                AddActivity(serverActivityType, activityID, fxActivity);
                fxActivity?.OnCreate(activityTimeConfig, this);
                return fxActivity;
            }
            catch (Exception e)
            {
                XDebug.LogError(e.Message);
                throw;
            }
        }

        public async Task<SGetActivities> RequestCGetActivitiesAsync()
        {
            var c = new CGetActivities();
            var tcs = new TaskCompletionSource<SGetActivities>();

            APIManagerBridge.Send(c, (response) =>
            {
                var s = response as SGetActivities;
                XDebug.Log("11111111111 Receive SGetActivities:" + s);
                tcs.TrySetResult(s);

            }, (errorCode, message, response) =>
            {
                XDebug.LogError("11111111111 Receive SGetActivities failed:" + message);
                tcs.TrySetResult(null);
            });

            return await tcs.Task;
        }

        public async Task<SGetActivityUserData> RequestCGetActivityUserDataAsync(List<string> ids, bool noticeActivityOpen = false)
        {
            var c = new CGetActivityUserData();
            foreach (var id in ids)
            {
                c.ActivityIds.Add(id);
            }
            var handle = await APIManagerGameModule.Instance.SendAsync<CGetActivityUserData, SGetActivityUserData>(c);
            if (handle != null && handle.ErrorCode == 0)
            {
                var response = handle.Response;
                lastUpdateTime = Time.realtimeSinceStartup;
                serverTimeSeconds = response.ServerTimeSeconds;
                if (response.ActivityDatas != null && response.ActivityDatas.Count > 0)
                {
                    var userDatas = response.ActivityDatas;
                    foreach (var userData in userDatas)
                    {
                        var activityID = userData.Key;
                        var activityType = GetActivityType(activityID);
                        if (string.IsNullOrWhiteSpace(activityID) || string.IsNullOrWhiteSpace(activityType))
                        {
                            continue;
                        }
                        var fxActivity = GetActivity(activityType, activityID);
                        if (fxActivity != null)
                        {
                            fxActivity.OnRefreshUserData(userData.Value);
                          
                            if(noticeActivityOpen)
                                fxActivity.OnActivityOpen();
                        }
                    }
                }
                return response;
            }
            else
            {
                // XDebug.LogError($"11111111111 Receive SGetActivityUserData failed:errorCode={handle.ErrorCode}, errorInfo={handle.ErrorInfo}");
                return null;
            }
        }

        #region ActivityDownload

        /// <summary>
        /// 为Loading提供的下载活动P1接口
        /// </summary>
        public List<string> GetNeedDownloadActivityHighPriorityAssets()
        {
            if (needDownloadLabel != null && needDownloadLabel.Count > 0)
            {
                //下载P1 bundle
                var listLabels = new List<string>();
                var bundlePriority = 1;
                foreach (var label in needDownloadLabel)
                {
                    var assetLabel = $"{label}_P{bundlePriority}";
                    listLabels.Add(assetLabel);
                }
             
                return listLabels;
            }
            return null;
        }

        /// <summary>
        /// 进入大厅开始调用
        /// </summary>
        private void BeginDownloadActivityAsset()
        {
            if (needDownloadLabel != null && needDownloadLabel.Count > 0)
            {
                //下载P2到P4的bundle，P1的bundle已经下载过了
                var bundlePriority = 2;
                foreach (var label in needDownloadLabel)
                {
                    CheckAndDownloadAssets(bundlePriority, label);
                }
            }
        }

        private void CheckAndDownloadAssets(int bundlePriority, string label)
        {
            if (bundlePriority > 4)
                return;

            var assetLabel = $"{label}_P{bundlePriority}";
            AssetHelper.CheckAssetExist(assetLabel, async (isExist) =>
            {
                if (isExist)
                {
                    var downloadSize = await AssetHelper.GetNeedDownloadSize(assetLabel);
                    if (downloadSize > 0)
                    {
                        DownloadActivityAsset(assetLabel, (isSuccess) =>
                        {
                            if (isSuccess)
                            {
                                XDebug.Log($"{assetLabel} download success!!!! begin to download next priority bundle!!!!!");
                                CheckAndDownloadAssets(bundlePriority + 1, label);
                            }
                            else
                            {
                                XDebug.LogWarning($"{assetLabel} download failed!!!! check your group label!!!!!");
                            }
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 下载活动资源
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public AssetDependenciesDownloadOperation DownloadActivityAsset(string labelName, Action<bool> downloadCallback)
        {
            if (downloadingOperations == null)
            {
                downloadingOperations = new Dictionary<string, AssetDependenciesDownloadOperation>();
            }
            if (downloadingOperations.ContainsKey(labelName))
            {
                return downloadingOperations[labelName];
            }
            var downloadingOperation = AssetHelper.DownloadDependencies(labelName, (operation) =>
            {
                if (operation)
                {
                    downloadingOperations.Remove(labelName);
                }
                else
                {
                    downloadingOperations.Remove(labelName);
                }
                downloadCallback?.Invoke(operation);
            }, true);
            downloadingOperations.Add(labelName, downloadingOperation);
            return downloadingOperation;
        }
        
        #endregion

        public override void CleanUp()
        {
            base.CleanUp();
            downloadingOperations?.Clear();
        }
    }
}