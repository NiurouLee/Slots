// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/11:51
// Ver : 1.0.0
// Description : AdConfigModel.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class AdLogicConfigModel:Model<SGetRVAdvertisingConfig>
    {
        public AdLogicConfigModel() 
            :base(ModelType.TYPE_ADS_CONFIG)
        {
            
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetRVAdvertisingConfig cGetRvAdvertisingConfig = new CGetRVAdvertisingConfig();
            cGetRvAdvertisingConfig.UserGroup = AdController.GetUserGroupId();
            
            var request = await APIManagerGameModule.Instance.SendAsync<CGetRVAdvertisingConfig, SGetRVAdvertisingConfig>(cGetRvAdvertisingConfig);
            
            if (request.ErrorCode == ErrorCode.Success)
            {
                UpdateModelData(request.Response);
            }
        }

        public SGetRVAdvertisingConfig.Types.AdTaskInfo GetAdTaskInfo()
        {
            if (modelData != null && modelData.AdTaskInfo != null)
            {
                return modelData.AdTaskInfo;
            }
            
            return null;
        }

        public float GetAdTaskRefreshLeftTime()
        {
            if(modelData != null && modelData.AdTaskInfo != null)
                return (float)modelData.AdTaskInfo.NextRefreshCountDown - TimeElapseSinceLastUpdate();
            return float.MaxValue;
        }

        public SGetRVAdvertisingConfig.Types.MysteryBoxInfo GetMysteryBoxInfo()
        {
            if (modelData != null && modelData.MysteryBoxInfo != null)
                return modelData.MysteryBoxInfo;
            return null;
        }

        // public float GetIsMysteryBoxActive()
        // {
        //     modelData.MysteryGiftInfo.LimitPerDay
        // }
        //
      
    }
}