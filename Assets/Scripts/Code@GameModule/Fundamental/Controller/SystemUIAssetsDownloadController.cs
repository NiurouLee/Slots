// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/10/10:47
// Ver : 1.0.0
// Description : SystemUIAssetsDownloadController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace GameModule
{
    public class SystemUIAssetsDownloadController : LogicController
    {
        private List<List<string>> assetsLabelsDownLoadPriorityList;
        
        public SystemUIAssetsDownloadController(Client client)
            : base(client)
        {
            assetsLabelsDownLoadPriorityList = new List<List<string>>();

            assetsLabelsDownLoadPriorityList.Add(new List<string>()
            {
                "AssetPriorityLevel2"
            });

            assetsLabelsDownLoadPriorityList.Add(new List<string>()
            {
                "AssetPriorityLevel3"
            });
            
            assetsLabelsDownLoadPriorityList.Add(new List<string>()
            {
                "AssetPriorityLevel4"
            });
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.SystemAssets);
        }

        protected async void OnSceneSwitchEnd(Action handleEndCallback,
            EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            handleEndCallback.Invoke();

            if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING &&
                eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
#if !UNITY_EDITOR              
                await WaitNFrame(2);
                for (var i = 0; i < assetsLabelsDownLoadPriorityList.Count; i++)
                {
                    await LoadAssetByLabels(assetsLabelsDownLoadPriorityList[i]);
                }
 #endif               
            }
        }

        public async Task LoadAssetByLabels(List<string> labels)
        {
            List<Task> waitingTask = new List<Task>();
            
            for (var i = 0; i < labels.Count; i++)
            {
                var task = LoadAssetByLabel(labels[i]);
                waitingTask.Add(task);
            }

            await Task.WhenAll(waitingTask);
        }

        public async Task LoadAssetByLabel(string label)
        {
            var waitTask = new TaskCompletionSource<bool>();
            AddWaitTask(waitTask, null);

            AssetHelper.DownloadDependencies(label, assetsRef =>
            {
                RemoveTask(waitTask);
                waitTask.SetResult(true);
            }, true);

            await waitTask.Task;
        }

    }
}