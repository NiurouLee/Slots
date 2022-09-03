// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/17/19:29
// Ver : 1.0.0
// Description : Model.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public enum ModelType
    {
        TYPE_USPER_PROFILE,
        TYPE_DAILY_BONUS,
        TYPE_VIP,
        TYPE_PAYMENT_INFO,
        TYPE_DAILY_MISSION,
        TYPE_SEASON_PASS,
        TYPE_PIGGY_BANK,
        TYPE_TIME_BONUS_INFO,
        TYPE_BUFF,
        TYPE_LOBBY_BANNER,
        TYPE_NEW_BIE_QUEST,
        TYPE_GUIDE,
        TYPE_SEASON_QUEST,
        TYPE_ADS_CONFIG,
        TYPE_ALBUM_INFO,
        TYPE_AMAZING_HAT_INFO,
        TYPE_INBOX_MODEL,
        TYPE_COIN_DASH,
        TYPE_LEVEL_RUSH,
    }

    /// <summary>
    /// 服务器原始数据存放，只读不修改
    /// </summary>
    public class Model
    {
        protected ModelType type;
        protected float lastTimeUpdateData = 0;
        protected bool isModelDataInitialized = false;
        
        public Model(ModelType modelType)
        {
            type = modelType;
        }

        public float LastTimeUpdateData
        {
            get => lastTimeUpdateData;
            set => lastTimeUpdateData = value;
        }

        public float TimeElapseSinceLastUpdate()
        {
            return Time.realtimeSinceStartup - lastTimeUpdateData;
        }

        public async void FetchModalDataFromServer()
        {
            await FetchModalDataFromServerAsync();
        }
        
        public virtual async Task FetchModalDataFromServerAsync()
        {
            await Task.CompletedTask;
        }

        public bool IsInitialized()
        {
            return isModelDataInitialized;
        }
        
    }

    public class Model<T> : Model
    {
        protected T modelData;
        
        public Model(ModelType modelType) : base(modelType)
        {
            
        }
        
        public virtual void UpdateModelData(T inModelData)
        {
            modelData = inModelData;
            isModelDataInitialized = inModelData != null;
            lastTimeUpdateData = Time.realtimeSinceStartup;
        }

        public T GetModelData()
        {
            return modelData;
        }
    }
}